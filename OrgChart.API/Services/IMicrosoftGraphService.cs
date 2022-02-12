using Microsoft.Graph;
using OrgChart.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrgChart.API.Services
{
    public interface IMicrosoftGraphService
    {
        Task<ADUser> GetUser(string userId);
        Task<IEnumerable<ADUser>> GetUsers();
        Task<IEnumerable<ADUser>> GetUserDirectReports(string userId);
        Task<IEnumerable<ADUser>> GetUserManagers(string userId, bool includeUser = false);
        Task<IEnumerable<ADUser>> SearchUsers(string query, string userId, bool includeUser = false);
        Task<IEnumerable<ADUser>> SearchManagers(string query, string userId = null, bool includeUser = false);
        Task<IEnumerable<ADUser>> GetUserOrgChart(string userId);
        Task<IEnumerable<ADUser>> GetUsersWithoutManagers();
        Task<IEnumerable<ADUser>> GetUsersWithManagers();
        Task AssignUserManager(string userId, string managerId, bool forceAssign = false);
        Task UnassignUserManager(string userId);
        Task AssignUsersManager(IEnumerable<string> userIds, string managerId, bool forceAssign = false);
        Task UnassignUsersManager(IEnumerable<string> userIds);
        Task<bool> UserExistsInGroup(string userId, string groupId);

        Task<UserProfile> GetProfile(string userId);
        Task UpdateProfile(string userId, Profile profile);

    }
}
