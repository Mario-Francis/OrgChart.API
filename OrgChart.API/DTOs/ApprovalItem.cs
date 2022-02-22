using SharepointCSOMLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class ApprovalItem
    {
        public int Id { get; set; }
        public string ApprovalType { get; set; } = ApprovalTypes.Approval.ToString();
        public string EmployeeEmail { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeJobTitle { get; set; }
        public string EmployeeDepartment { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerName { get; set; }
        public string ToManagerEmail { get; set; }
        public string ToManagerName { get; set; }
        public string RequestorEmail { get; set; }
        public string RequestorName { get; set; }
        public string ApprovalStatus { get; set; }
        public string LocalADSyncStatus { get; set; }
        public string Comment { get; set; }
        public string ReviewDate { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("approvalType", ApprovalType),
                new KeyValuePair<string, string>("employeeEmail", EmployeeEmail),
                new KeyValuePair<string, string>("employeeName", EmployeeName),
                new KeyValuePair<string, string>("employeeJobTitle", EmployeeJobTitle),
                new KeyValuePair<string, string>("employeeDepartment", EmployeeDepartment),
                new KeyValuePair<string, string>("managerEmail", ManagerEmail),
                new KeyValuePair<string, string>("managerName", ManagerName),
                new KeyValuePair<string, string>("requestorEmail", RequestorEmail),
                new KeyValuePair<string, string>("requestorName", RequestorName),
                new KeyValuePair<string, string>("toManager", ToManagerEmail),
                new KeyValuePair<string, string>("toManagerName", ToManagerName),
                //new KeyValuePair<string, string>("reviewDate", ReviewDate),
                new KeyValuePair<string, string>("approvalStatus", ApprovalStatus),
                new KeyValuePair<string, string>("localADSyncStatus", LocalADSyncStatus),
                //new KeyValuePair<string, string>("comment", Comment)
            };
        }

        public static ApprovalItem FromSPListItem(SPListItem item)
        {
            return new ApprovalItem
            {
                Id = item.Id,
                ApprovalType= item.FieldValues.FirstOrDefault(x => x.Key == "approvalType").Value,
                EmployeeEmail = item.FieldValues.FirstOrDefault(x => x.Key == "employeeEmail").Value,
                EmployeeName = item.FieldValues.FirstOrDefault(x => x.Key == "employeeName").Value,
                EmployeeJobTitle = item.FieldValues.FirstOrDefault(x => x.Key == "employeeJobTitle").Value,
                EmployeeDepartment = item.FieldValues.FirstOrDefault(x => x.Key == "employeeDepartment").Value,
                ManagerEmail = item.FieldValues.FirstOrDefault(x => x.Key == "managerEmail").Value,
                ManagerName = item.FieldValues.FirstOrDefault(x => x.Key == "managerName").Value,
                ToManagerEmail = item.FieldValues.FirstOrDefault(x => x.Key == "toManager").Value,
                ToManagerName= item.FieldValues.FirstOrDefault(x => x.Key == "toManagerName").Value,
                RequestorEmail = item.FieldValues.FirstOrDefault(x => x.Key == "requestorEmail").Value,
                RequestorName = item.FieldValues.FirstOrDefault(x => x.Key == "requestorName").Value,
                ApprovalStatus = item.FieldValues.FirstOrDefault(x => x.Key == "approvalStatus").Value,
                Comment = item.FieldValues.FirstOrDefault(x => x.Key == "comment").Value,
                ReviewDate = item.FieldValues.FirstOrDefault(x => x.Key == "reviewDate").Value,
                Created = DateTimeOffset.Parse(item.FieldValues.FirstOrDefault(x => x.Key == "Created").Value).ToString("MM-dd-yyyy hh:mm tt"),
                Modified = DateTimeOffset.Parse(item.FieldValues.FirstOrDefault(x => x.Key == "Modified").Value).ToString("MM-dd-yyyy hh:mm tt"),
                LocalADSyncStatus = item.FieldValues.FirstOrDefault(x => x.Key == "localADSyncStatus").Value
            };
        }
    }
}
