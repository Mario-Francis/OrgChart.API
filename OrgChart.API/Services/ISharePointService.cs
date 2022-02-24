using OrgChart.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.Services
{
    public interface ISharePointService
    {
        Task AddApprovalItem(ApprovalItem item);
        Task BatchAddApprovalItem(IEnumerable<ApprovalItem> items);
        Task<ApprovalItem> GetApprovalItem(int itemId);
        Task UpdateApprovalItem(int id, string approvalStatus, string comment = null);
        Task<IEnumerable<ApprovalItem>> GetInitiatedPendingApprovalItems(string requestorEmail);
        Task<IEnumerable<ApprovalItem>> GetApprovalPendingItemLocalADSyncCompleted();        
        Task<IEnumerable<ApprovalItem>> GetApprovalItemsPendingAction(string managerEmail);
        Task<IEnumerable<ApprovalItem>> GetApprovalItemsPendingAcceptance(string toManagerEmail);
        Task<bool> IsEmployeePendingRequestExists(string employeeEmail);
        Task<bool> IsManagerHasMultiplePendingRequestForEmployee(string employeeEmail, string managerEmail);
        Task DeleteApprovalItem(int itemId);

        //=============================== Profile  Approval ==============================
        Task AddProfileApprovalItem(ProfileApprovalItem item);
        Task<ProfileApprovalItem> GetProfileApprovalItem(int itemId);
        Task UpdateProfileApprovalItem(int id, string approvalStatus, string comment = null);
        Task<IEnumerable<ProfileApprovalItem>> GetInitiatedPendingProfileApprovalItems(string employeeEmail);
        Task<IEnumerable<ProfileApprovalItem>> GetProfileApprovalItemsPendingAction(string managerEmail);
        Task<bool> IsEmployeePendingProfileRequestExists(string employeeEmail);
        Task<bool> IsManagerHasMultiplePendingProfileRequestForEmployee(string employeeEmail, string managerEmail);
        Task DeleteProfileApprovalItem(int itemId);

    }
}
