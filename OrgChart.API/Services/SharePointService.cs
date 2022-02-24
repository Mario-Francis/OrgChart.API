using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OrgChart.API.DTOs;
using SharepointCSOMLib;
using SharepointCSOMLib.Models;

namespace OrgChart.API.Services
{
    public class SharePointService:ISharePointService
    {
        private readonly IListManager listMgr;
        private readonly IOptionsSnapshot<SharePointSettings> spSettingsDelegate;

        public SharePointService(IListManager listMgr, IOptionsSnapshot<SharePointSettings> spSettingsDelegate)
        {
            this.listMgr = listMgr;
            this.spSettingsDelegate = spSettingsDelegate;
        }

        public async Task AddApprovalItem(ApprovalItem item)
        {
            var req = new AddItemRequest
            {
                ListName = spSettingsDelegate.Value.ApprovalList,
                FieldValues = item.ToKeyValuePairs()
            };
            await listMgr.AddItem(req);
        }

        public async Task BatchAddApprovalItem(IEnumerable<ApprovalItem> items)
        {
            var req = new BatchAddItemsRequest
            {
                ListName = spSettingsDelegate.Value.ApprovalList,
                Items = items.Select(i => i.ToKeyValuePairs())
            };
            await listMgr.BatchAddItems(req);
        }

        public async Task<ApprovalItem> GetApprovalItem(int itemId)
        {
            var result = await listMgr.GetItem(itemId, spSettingsDelegate.Value.ApprovalList);
            var item = ApprovalItem.FromSPListItem(result);

            return item;
        }

        public async Task UpdateApprovalItem(int id, string approvalStatus, string comment = null)
        {
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("approvalStatus", approvalStatus));
            values.Add(new KeyValuePair<string, string>("reviewDate", DateTime.Now.ToString("MM-dd-yyyy")));
            if (!string.IsNullOrEmpty(comment))
            {
                values.Add(new KeyValuePair<string, string>("comment", comment));
            }
            var req = new UpdateItemRequest
            {
                Id = id,
                ListName = spSettingsDelegate.Value.ApprovalList,
                FieldValues = values
            };
            await listMgr.UpdateItem(req);
        }

        public async Task<IEnumerable<ApprovalItem>> GetInitiatedPendingApprovalItems(string requestorEmail)
        {
            var req = new SearchListByFieldValuesRequest
            {
                ListName = spSettingsDelegate.Value.ApprovalList,
                SearchParams = new List<KeyValuePair<string, string>>
               {
                   new KeyValuePair<string, string>("approvalType", ApprovalTypes.Approval.ToString()),
                   new KeyValuePair<string, string>("requestorEmail", requestorEmail),
                   new KeyValuePair<string, string>("approvalStatus", ApprovalStatus.PENDING.ToString())
               },
                Options = new PagingOptions { Length = 1000, SortByDateCreated = true, SortByDateCreatedDir = "DESC", StartIndex = 0 }
            };
            var result = await listMgr.SearchListByFieldValues(req, 1000);
            var items = result.ListItems.Select(i => ApprovalItem.FromSPListItem(i));

            return items;
        }

        public async Task<IEnumerable<ApprovalItem>> GetApprovalItemsPendingAction(string managerEmail)
        {
            var req = new SearchListByFieldValuesRequest
            {
                ListName = spSettingsDelegate.Value.ApprovalList,
                SearchParams = new List<KeyValuePair<string, string>>
               {
                   new KeyValuePair<string, string>("approvalType", ApprovalTypes.Approval.ToString()),
                   new KeyValuePair<string, string>("managerEmail", managerEmail),
                   new KeyValuePair<string, string>("approvalStatus", ApprovalStatus.PENDING.ToString())
               },
                Options = new PagingOptions { Length = 1000, SortByDateCreated = true, SortByDateCreatedDir = "DESC", StartIndex = 0 }
            };
            var result = await listMgr.SearchListByFieldValues(req, 1000);
            var items = result.ListItems.Select(i => ApprovalItem.FromSPListItem(i));

            return items;
        }

        public async Task<IEnumerable<ApprovalItem>> GetApprovalPendingItemLocalADSyncCompleted()
        {
            var req = new SearchListByFieldValuesRequest
            {
                ListName = spSettingsDelegate.Value.ApprovalList,
                SearchParams = new List<KeyValuePair<string, string>>
               {
                   new KeyValuePair<string, string>("approvalType", ApprovalTypes.Approval.ToString()),
                   new KeyValuePair<string, string>("LocalADSyncStatus", "Completed"),
                   new KeyValuePair<string, string>("approvalStatus", ApprovalStatus.PENDING.ToString())
               },
                Options = new PagingOptions { Length = 1000, SortByDateCreated = true, SortByDateCreatedDir = "DESC", StartIndex = 0 }
            };
            var result = await listMgr.SearchListByFieldValues(req, 1000);

            var items = result.ListItems.Select(i => ApprovalItem.FromSPListItem(i));

            return items;
        }

        public async Task<IEnumerable<ApprovalItem>> GetApprovalItemsPendingAcceptance(string toManagerEmail)
        {
            var req = new SearchListByFieldValuesRequest
            {
                ListName = spSettingsDelegate.Value.ApprovalList,
                SearchParams = new List<KeyValuePair<string, string>>
               {
                   new KeyValuePair<string, string>("approvalType", ApprovalTypes.Acceptance.ToString()),
                   new KeyValuePair<string, string>("toManager", toManagerEmail),
                   new KeyValuePair<string, string>("approvalStatus", ApprovalStatus.PENDING.ToString())
               },
                Options = new PagingOptions { Length = 1000, SortByDateCreated = true, SortByDateCreatedDir = "DESC", StartIndex = 0 }
            };
            var result = await listMgr.SearchListByFieldValues(req, 1000);
            var items = result.ListItems.Select(i => ApprovalItem.FromSPListItem(i));

            return items;
        }

        public async Task<bool> IsEmployeePendingRequestExists(string employeeEmail)
        {
            var req = new SearchListByFieldValuesRequest
            {
                ListName = spSettingsDelegate.Value.ApprovalList,
                SearchParams = new List<KeyValuePair<string, string>>
               {
                   new KeyValuePair<string, string>("employeeEmail", employeeEmail),
                   new KeyValuePair<string, string>("approvalStatus", ApprovalStatus.PENDING.ToString())
               },
                Options = new PagingOptions { Length = 1000, SortByDateCreated = true, SortByDateCreatedDir = "DESC", StartIndex = 0 }
            };
            var result = await listMgr.SearchListByFieldValues(req, 1000);
            //var items = result.ListItems.Select(i => ApprovalItem.FromSPListItem(i));
            return result.TotalResultCount > 0;
        }

        public async Task<bool> IsManagerHasMultiplePendingRequestForEmployee(string employeeEmail, string managerEmail)
        {
            var req = new SearchListByFieldValuesRequest
            {
                ListName = spSettingsDelegate.Value.ApprovalList,
                SearchParams = new List<KeyValuePair<string, string>>
               {
                   new KeyValuePair<string, string>("employeeEmail", employeeEmail),
                   new KeyValuePair<string, string>("managerEmail", managerEmail),
                   new KeyValuePair<string, string>("approvalStatus", ApprovalStatus.PENDING.ToString())
               },
                Options = new PagingOptions { Length = 1000, SortByDateCreated = true, SortByDateCreatedDir = "DESC", StartIndex = 0 }
            };
            var result = await listMgr.SearchListByFieldValues(req, 1000);
            //var items = result.ListItems.Select(i => ApprovalItem.FromSPListItem(i));
            return result.TotalResultCount > 1;
        }
        public async Task DeleteApprovalItem(int itemId)
        {
            await listMgr.DeleteItem(itemId, spSettingsDelegate.Value.ApprovalList);
        }

        //=============================== Profile  Approval ==============================
        public async Task AddProfileApprovalItem(ProfileApprovalItem item)
        {
            var req = new AddItemRequest
            {
                ListName = spSettingsDelegate.Value.ProfileApprovalList,
                FieldValues = item.ToKeyValuePairs()
            };
            await listMgr.AddItem(req);
        }


        public async Task<ProfileApprovalItem> GetProfileApprovalItem(int itemId)
        {
            var result = await listMgr.GetItem(itemId, spSettingsDelegate.Value.ProfileApprovalList);
            var item = ProfileApprovalItem.FromSPListItem(result);

            return item;
        }

        public async Task UpdateProfileApprovalItem(int id, string approvalStatus, string comment = null)
        {
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("approvalStatus", approvalStatus));
            values.Add(new KeyValuePair<string, string>("reviewDate", DateTime.Now.ToString("MM-dd-yyyy")));
            if (!string.IsNullOrEmpty(comment))
            {
                values.Add(new KeyValuePair<string, string>("comment", comment));
            }
            var req = new UpdateItemRequest
            {
                Id = id,
                ListName = spSettingsDelegate.Value.ProfileApprovalList,
                FieldValues = values
            };
            await listMgr.UpdateItem(req);
        }

        public async Task<IEnumerable<ProfileApprovalItem>> GetInitiatedPendingProfileApprovalItems(string employeeEmail)
        {
            var req = new SearchListByFieldValuesRequest
            {
                ListName = spSettingsDelegate.Value.ProfileApprovalList,
                SearchParams = new List<KeyValuePair<string, string>>
               {
                   new KeyValuePair<string, string>("employeeEmail", employeeEmail),
                   new KeyValuePair<string, string>("approvalStatus", ApprovalStatus.PENDING.ToString())
               },
                Options = new PagingOptions { Length = 1000, SortByDateCreated = true, SortByDateCreatedDir = "DESC", StartIndex = 0 }
            };
            var result = await listMgr.SearchListByFieldValues(req, 1000);
            var items = result.ListItems.Select(i => ProfileApprovalItem.FromSPListItem(i));

            return items;
        }

        public async Task<IEnumerable<ProfileApprovalItem>> GetProfileApprovalItemsPendingAction(string managerEmail)
        {
            var req = new SearchListByFieldValuesRequest
            {
                ListName = spSettingsDelegate.Value.ProfileApprovalList,
                SearchParams = new List<KeyValuePair<string, string>>
               {
                   new KeyValuePair<string, string>("managerEmail", managerEmail),
                   new KeyValuePair<string, string>("approvalStatus", ApprovalStatus.PENDING.ToString())
               },
                Options = new PagingOptions { Length = 1000, SortByDateCreated = true, SortByDateCreatedDir = "DESC", StartIndex = 0 }
            };
            var result = await listMgr.SearchListByFieldValues(req, 1000);
            var items = result.ListItems.Select(i => ProfileApprovalItem.FromSPListItem(i));

            return items;
        }

        public async Task<bool> IsEmployeePendingProfileRequestExists(string employeeEmail)
        {
            var req = new SearchListByFieldValuesRequest
            {
                ListName = spSettingsDelegate.Value.ProfileApprovalList,
                SearchParams = new List<KeyValuePair<string, string>>
               {
                   new KeyValuePair<string, string>("employeeEmail", employeeEmail),
                   new KeyValuePair<string, string>("approvalStatus", ApprovalStatus.PENDING.ToString())
               },
                Options = new PagingOptions { Length = 1000, SortByDateCreated = true, SortByDateCreatedDir = "DESC", StartIndex = 0 }
            };
            var result = await listMgr.SearchListByFieldValues(req, 1000);
            return result.TotalResultCount > 0;
        }

        public async Task<bool> IsManagerHasMultiplePendingProfileRequestForEmployee(string employeeEmail, string managerEmail)
        {
            var req = new SearchListByFieldValuesRequest
            {
                ListName = spSettingsDelegate.Value.ProfileApprovalList,
                SearchParams = new List<KeyValuePair<string, string>>
               {
                   new KeyValuePair<string, string>("employeeEmail", employeeEmail),
                   new KeyValuePair<string, string>("managerEmail", managerEmail),
                   new KeyValuePair<string, string>("approvalStatus", ApprovalStatus.PENDING.ToString())
               },
                Options = new PagingOptions { Length = 1000, SortByDateCreated = true, SortByDateCreatedDir = "DESC", StartIndex = 0 }
            };
            var result = await listMgr.SearchListByFieldValues(req, 1000);
            return result.TotalResultCount > 1;
        }
        public async Task DeleteProfileApprovalItem(int itemId)
        {
            await listMgr.DeleteItem(itemId, spSettingsDelegate.Value.ProfileApprovalList);
        }
    }
}
