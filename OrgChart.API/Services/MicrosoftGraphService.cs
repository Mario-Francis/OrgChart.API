using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using OrgChart.API.DTOs;
using OrgChart.API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using IHttpClientFactory = System.Net.Http.IHttpClientFactory;

namespace OrgChart.API.Services
{
    public class MicrosoftGraphService : IMicrosoftGraphService
    {
        private readonly AzureADSettings azureADSettings;
        private readonly IOptions<AzureADSettings> azureADSettings1;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<MicrosoftGraphService> logger;
        private readonly IConfiguration config;

        public MicrosoftGraphService(IOptions<AzureADSettings> azureADSettings,
            IHttpClientFactory clientFactory,
            ILogger<MicrosoftGraphService> logger,
            IConfiguration config)
        {
            this.azureADSettings = azureADSettings.Value;
            azureADSettings1 = azureADSettings;
            this.clientFactory = clientFactory;
            this.logger = logger;
            this.config = config;
        }

        private async Task<GraphServiceClient> GetGraphServiceClient()
        {
            // Get Access Token and Microsoft Graph Client using access token and microsoft graph v1.0 endpoint
            var delegateAuthProvider = await GetAuthProvider();
            // Initializing the GraphServiceClient
            var graphClient = new GraphServiceClient(azureADSettings.GraphAPIEndPoint, delegateAuthProvider);

            return graphClient;
        }


        private async Task<IAuthenticationProvider> GetAuthProvider()
        {
            AuthenticationContext authenticationContext = new AuthenticationContext(azureADSettings.Authority);
            ClientCredential clientCred = new ClientCredential(azureADSettings.ClientId, azureADSettings.ClientSecret);

            // ADAL includes an in memory cache, so this call will only send a message to the server if the cached token is expired.
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(azureADSettings.GraphResource, clientCred);
            var token = authenticationResult.AccessToken;

            var delegateAuthProvider = new DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token.ToString());
                return Task.FromResult(0);
            });

            return delegateAuthProvider;
        }

        // public methods
        public async Task<ADUser> GetUser(string userId)
        {
            var client = await GetGraphServiceClient();

            var user = await client.Users[userId].Request()
                 //.Expand(u => u.Manager)
                 .Expand("manager($select=id,displayName)")
                 .Select(u => new
                 {
                     u.Id,
                     u.GivenName,
                     u.Surname,
                     u.DisplayName,
                     u.Mail,
                     u.JobTitle,
                     u.Department,
                     u.BusinessPhones,
                     u.MobilePhone,
                     u.AccountEnabled,
                     u.UserPrincipalName,
                     u.Manager
                 })
                .GetAsync();

            return ADUser.FromUser(user);
        }

        public async Task<IEnumerable<ADUser>> GetUsers()
        {
            var client = await GetGraphServiceClient();
            var users = await client.Users.Request()
                // .Expand(u=>u.Manager)
                .Expand("manager($select=id,displayName)")
                .Filter("accountEnabled eq true")
                .Select(u => new
                {
                    u.Id,
                    u.GivenName,
                    u.Surname,
                    u.DisplayName,
                    u.Mail,
                    u.JobTitle,
                    u.Department,
                    u.BusinessPhones,
                    u.MobilePhone,
                    u.AccountEnabled,
                    u.UserPrincipalName,
                    u.Manager
                })
                .GetAsync();

            return users.Select(u => ADUser.FromUser(u));
        }


        public async Task<IEnumerable<ADUser>> GetUserDirectReports(string userId)
        {
            var user = await GetUser(userId);
            var client = await GetGraphServiceClient();
            var directReports = await client.Users[userId].DirectReports.Request()
                //.Select("id,displayName,jobTitle,mail,surname,givenName,mobilePhone,businessPhones,department,accountEnabled")
                .GetAsync();

            return directReports.Select(u =>
            {
                var _u = ADUser.FromUser(u as User);
                _u.ManagerId = user.Id;
                return _u;
            });
        }

        public async Task<IEnumerable<ADUser>> GetUserManagers(string userId, bool includeUser = false)
        {
            AuthenticationContext authenticationContext = new AuthenticationContext(azureADSettings.Authority);
            ClientCredential clientCred = new ClientCredential(azureADSettings.ClientId, azureADSettings.ClientSecret);

            // ADAL includes an in memory cache, so this call will only send a message to the server if the cached token is expired.
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(azureADSettings.GraphResource, clientCred);
            var token = authenticationResult.AccessToken;

            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User ID is required");
            }
            var url = $"https://graph.microsoft.com/v1.0/users/{userId}?$expand=manager($levels=max;$select=id,displayName,userPrincipalName,jobTitle,mail,surname,givenName,mobilePhone,businessPhones,department,accountEnabled)&$select=id,displayName,jobTitle,mail,surname,userPrincipalName,givenName,mobilePhone,businessPhones,department,accountEnabled&$count=true&ConsistencyLevel=eventual";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token.ToString());
            var client = clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            var resContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var managerChain = JsonSerializer.Deserialize<ADUser>(resContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (!includeUser)
                {
                    managerChain = managerChain.Manager;
                }
                var managers = new List<ADUser>();
                while (managerChain != null)
                {
                    var _manager = managerChain.Clone();
                    _manager.Manager = null;
                    managers.Add(_manager);
                    managerChain = managerChain.Manager;
                }
                return managers;
            }
            else
            {
                throw new Exception(resContent);
            }
        }

        public async Task<IEnumerable<ADUser>> GetUserOrgChart(string userId)
        {
            var managers = await GetUserManagers(userId, true);
            var directReports = await GetUserDirectReports(userId);
            IEnumerable<ADUser> siblings = new List<ADUser>();
            if (directReports.Count() == 0)
            {
                if (managers.Count() > 1)
                {
                    siblings = (await GetUserDirectReports(managers.ElementAt(1).Id)).Where(e => e.Id != userId);
                }
            }

            if (directReports.Count() > 0)
            {
                return managers.Concat(directReports);
            }
            else
            {
                return managers.Concat(siblings);
            }
        }

        public async Task<IEnumerable<ADUser>> GetUsersWithoutManagers()
        {
            var usersWithoutManagers = (await GetUsers()).Where(u => u.Manager == null);
            var groupId = config["ManagersGroupId"];
            var client = await GetGraphServiceClient();
            var users = await client.Users.Request()
                //.Expand("manager($select=id,displayName)")
                .Expand("memberOf($select=id)")
                .Filter("accountEnabled eq true")
                .Select(u => new
                {
                    u.Id,
                    //u.GivenName,
                    //u.Surname,
                    u.DisplayName,
                    //u.Mail,
                    //u.JobTitle,
                    u.Department,
                    //u.BusinessPhones,
                    //u.MobilePhone,
                    //u.AccountEnabled,
                    u.UserPrincipalName,
                    u.Manager
                })
                .GetAsync();

           var _users = usersWithoutManagers;
            if (!string.IsNullOrEmpty(groupId))
            {
                var __users = users.Where(u => !u.MemberOf.Any(g => g.Id == groupId));
                _users = _users.Where(u => __users.Any(x => x.Id == u.Id));
            }

            return _users;
        }

        public async Task<IEnumerable<ADUser>> GetUsersWithManagers()
        {
            var client = await GetGraphServiceClient();
            var users = await client.Users.Request()
                .Expand("manager($select=id,displayName)")
                .Filter("accountEnabled eq true")
                .Select(u => new
                {
                    u.Id,
                    //u.GivenName,
                    //u.Surname,
                    u.DisplayName,
                    //u.Mail,
                    //u.JobTitle,
                    u.Department,
                    //u.BusinessPhones,
                    //u.MobilePhone,
                    //u.AccountEnabled,
                    u.UserPrincipalName,
                    u.Manager
                })
                .GetAsync();
            var _users = users.Where(u => u.Manager != null);

            return _users.Select(u =>
            {
                var _u = ADUser.FromUser(u);
                _u.Manager = null;
                return _u;
            });
        }

        public async Task AssignUserManager(string userId, string managerId)
        {
            var user = await GetUser(userId);
            if (user.ManagerId == null)
            {
                var client = await GetGraphServiceClient();
                await client.Users[userId].Manager.Reference.Request().PutAsync(managerId);
            }
            else
            {
                throw new Exception("User has already been claimed");
            }
        }

        public async Task UnassignUserManager(string userId)
        {
            var client = await GetGraphServiceClient();
            await client.Users[userId].Manager.Reference.Request().DeleteAsync();
        }

        public async Task AssignUsersManager(IEnumerable<string> userIds, string managerId)
        {

            await userIds.ParallelForEachAsync(async (userId) =>
            {
                var user = await GetUser(userId);
                if (user.ManagerId == null)
                {
                    var client = await GetGraphServiceClient();
                    await client.Users[userId].Manager.Reference.Request().PutAsync(managerId);
                }
            }, Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)));
        }

        public async Task UnassignUsersManager(IEnumerable<string> userIds)
        {
            await userIds.ParallelForEachAsync(async (userId) =>
            {
                var client = await GetGraphServiceClient();
                await client.Users[userId].Manager.Reference.Request().DeleteAsync();
            }, Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)));
        }

        public async Task<bool> UserExistsInGroup(string userId, string groupId)
        {
            var client = await GetGraphServiceClient();
            var groupIds = new List<String>()
            {
                groupId
            };
            var groups = await client.Users[userId].CheckMemberGroups(groupIds).Request().PostAsync();
            if (groups.Contains(groupId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
