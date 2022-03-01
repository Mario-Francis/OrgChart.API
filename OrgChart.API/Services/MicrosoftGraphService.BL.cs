using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using OrgChart.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrgChart.API.Services
{
    public partial class MicrosoftGraphService
    {
        // get profile photo
        private async Task<string> GetUserPhoto(string userId)
        {
            try
            {
                var client = await GetGraphServiceClient();

                var photoStream = await client.Users[userId]
                    .Photos["240x240"]
                    .Content
                    .Request()
                    .GetAsync();
                var photoBytes = new byte[photoStream.Length];
                await photoStream.ReadAsync(photoBytes);
                return Convert.ToBase64String(photoBytes);
            }
            catch (ServiceException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }


        // update profile photo
        private async Task UpdateUserPhoto(string userId, string base64Photo)
        {
            var photoBytes = Convert.FromBase64String(base64Photo);

            var client = await GetGraphServiceClient();

            using var stream = new System.IO.MemoryStream(photoBytes);

            await client.Users[userId].Photo.Content
                .Request()
                .PutAsync(stream);

        }


        // get user profile
        private async Task<UserProfile> GetUserProfile(string userId)
        {
            var client = await GetGraphServiceClient();

            var user = await client.Users[userId].Request()
                 .Select(u => new
                 {
                     u.Id,
                     u.UserPrincipalName,
                     u.AboutMe,
                     u.BusinessPhones,
                     u.City,
                     u.Country,
                     u.MobilePhone,
                     u.OfficeLocation,
                     u.PostalCode,
                     u.State,
                     u.StreetAddress
                 })
                .GetAsync();

            return new UserProfile
            {
                Profile = Profile.FromUser(user),
                Email = user.UserPrincipalName,
                Id = user.Id
            };
        }


        // update user profile
        private async Task UpdateUserProfile(string userId, Profile profile)
        {
            var client = await GetGraphServiceClient();

            var user = profile.ToUser();

            await client.Users[userId]
                .Request()
                .UpdateAsync(user);
        }

        private async Task HttpUpdateUserProfile(string userId, Profile profile)
        {
            AuthenticationContext authenticationContext = new AuthenticationContext(azureADSettingsDelegate.Value.Authority);
            ClientCredential clientCred = new ClientCredential(azureADSettingsDelegate.Value.ClientId, azureADSettingsDelegate.Value.ClientSecret);

            // ADAL includes an in memory cache, so this call will only send a message to the server if the cached token is expired.
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(azureADSettingsDelegate.Value.GraphResource, clientCred);
            var token = authenticationResult.AccessToken;

            var url = $"https://graph.microsoft.com/v1.0/users/{userId}";
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token.ToString());
            var body = JsonSerializer.Serialize(new
            {
                businessPhones = new List<string> { profile.BusinessPhone },
                mobilePhone = profile.MobilePhone,
                officeLocation = profile.Office,
                streetAddress = profile.Street,
                postalCode = profile.PostalCode,
                city = profile.City,
                state = profile.State,
                country = profile.Country
            });
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

           var client = clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            
            if (!response.IsSuccessStatusCode)
            {
                var resContent = await response.Content.ReadAsStringAsync();
                throw new Exception(resContent);
            }

        }

        // update user about me
        private async Task UpdateUserAboutMe(string userId, Profile profile)
        {
            var client = await GetGraphServiceClient();

            var user = profile.ToAboutMe();

            await client.Users[userId]
                .Request()
                .UpdateAsync(user);

        }

        private async Task HttpUpdateUserAboutMe(string userId, Profile profile)
        {
            AuthenticationContext authenticationContext = new AuthenticationContext(azureADSettingsDelegate.Value.Authority);
            ClientCredential clientCred = new ClientCredential(azureADSettingsDelegate.Value.ClientId, azureADSettingsDelegate.Value.ClientSecret);

            // ADAL includes an in memory cache, so this call will only send a message to the server if the cached token is expired.
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(azureADSettingsDelegate.Value.GraphResource, clientCred);
            var token = authenticationResult.AccessToken;

            var url = $"https://graph.microsoft.com/v1.0/users/{userId}";
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token.ToString());
            var body = JsonSerializer.Serialize(new
            {
                aboutMe = profile.AboutMe
            });
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var client = clientFactory.CreateClient();
            var response = await client.SendAsync(request);


            if (!response.IsSuccessStatusCode)
            {
                var resContent = await response.Content.ReadAsStringAsync();
                throw new Exception(resContent);
            }

        }




    }
}
