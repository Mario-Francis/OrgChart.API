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
        Task<IEnumerable<ADUser>> GetUserOrgChart(string userId);
        Task<IEnumerable<ADUser>> GetUsersWithoutManagers();
        Task<IEnumerable<ADUser>> GetUsersWithManagers();
        Task AssignUserManager(string userId, string managerId);
        Task UnassignUserManager(string userId);
        Task AssignUsersManager(IEnumerable<string> userIds, string managerId);
        Task UnassignUsersManager(IEnumerable<string> userIds);
        Task<bool> UserExistsInGroup(string userId, string groupId);
    }
}
