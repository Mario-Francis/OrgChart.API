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
        public string EmployeeEmail { get; set; }
        public string ManagerEmail { get; set; }
        public string ToManagerEmail { get; set; }
        public string RequestorEmail { get; set; }
        public string ApprovalStatus { get; set; }
        public string Comment { get; set; }
        public string ReviewDate { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("employeeEmail", EmployeeEmail),
                new KeyValuePair<string, string>("managerEmail", ManagerEmail),
                new KeyValuePair<string, string>("requestorEmail", RequestorEmail),
                new KeyValuePair<string, string>("toManager", ToManagerEmail),
                //new KeyValuePair<string, string>("reviewDate", ReviewDate),
                new KeyValuePair<string, string>("approvalStatus", ApprovalStatus),
                //new KeyValuePair<string, string>("comment", Comment)
            };
        }

        public static ApprovalItem FromSPListItem(SPListItem item)
        {
            return new ApprovalItem
            {
                Id = item.Id,
                EmployeeEmail = item.FieldValues.FirstOrDefault(x => x.Key == "employeeEmail").Value,
                ManagerEmail = item.FieldValues.FirstOrDefault(x => x.Key == "managerEmail").Value,
                ToManagerEmail = item.FieldValues.FirstOrDefault(x => x.Key == "toManager").Value,
                RequestorEmail = item.FieldValues.FirstOrDefault(x => x.Key == "requestorEmail").Value,
                ApprovalStatus = item.FieldValues.FirstOrDefault(x => x.Key == "approvalStatus").Value,
                Comment = item.FieldValues.FirstOrDefault(x => x.Key == "comment").Value,
                ReviewDate = item.FieldValues.FirstOrDefault(x => x.Key == "reviewDate").Value,
                Created = DateTime.Parse(item.FieldValues.FirstOrDefault(x => x.Key == "Created").Value).ToString("MM-dd-yyyy"),
                Modified = DateTime.Parse(item.FieldValues.FirstOrDefault(x => x.Key == "Modified").Value).ToString("MM-dd-yyyy"),
            };
        }
    }
}
