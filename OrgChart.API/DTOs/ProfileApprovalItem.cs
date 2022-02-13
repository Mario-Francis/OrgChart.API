using SharepointCSOMLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrgChart.API.DTOs
{
    public class ProfileApprovalItem
    {
        public int Id { get; set; }
        public string StreetAddress { get; set; }
        public string StateProvince { get; set; }
        public string CountryRegion { get; set; }
        public string Office { get; set; }
        public string City { get; set; }
        public string ZipPostalCode { get; set; }
        public string OfficePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Photo { get; set; }
        public string AboutMe { get; set; }
        public string ApprovalStatus { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerName { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeeName { get; set; }
        public string Comment { get; set; }
        public string ReviewDate { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("streetAddress", StreetAddress),
                new KeyValuePair<string, string>("stateProvince", StateProvince),
                new KeyValuePair<string, string>("countryRegion", CountryRegion),
                new KeyValuePair<string, string>("office", Office),
                new KeyValuePair<string, string>("city", City),
                new KeyValuePair<string, string>("zipPostalCode", ZipPostalCode),
                new KeyValuePair<string, string>("officePhone", OfficePhone),
                new KeyValuePair<string, string>("mobilePhone", MobilePhone),
                new KeyValuePair<string, string>("photo", Photo),
                new KeyValuePair<string, string>("aboutMe", AboutMe),
                new KeyValuePair<string, string>("approvalStatus", ApprovalStatus),
                new KeyValuePair<string, string>("managerEmail", ManagerEmail),
                 new KeyValuePair<string, string>("managerName", ManagerName),
                new KeyValuePair<string, string>("employeeEmail", EmployeeEmail),
                 new KeyValuePair<string, string>("employeeName", EmployeeName),
                //new KeyValuePair<string, string>("comment", Comment),
                //new KeyValuePair<string, string>("reviewDate", ReviewDate),
            };
        }

        public static ProfileApprovalItem FromSPListItem(SPListItem item)
        {
            return new ProfileApprovalItem
            {
                Id = item.Id,
                StreetAddress = item.FieldValues.FirstOrDefault(x => x.Key == "streetAddress").Value,
                StateProvince = item.FieldValues.FirstOrDefault(x => x.Key == "stateProvince").Value,
                CountryRegion = item.FieldValues.FirstOrDefault(x => x.Key == "countryRegion").Value,
                Office = item.FieldValues.FirstOrDefault(x => x.Key == "office").Value,
                City = item.FieldValues.FirstOrDefault(x => x.Key == "city").Value,
                ZipPostalCode = item.FieldValues.FirstOrDefault(x => x.Key == "zipPostalCode").Value,
                OfficePhone = item.FieldValues.FirstOrDefault(x => x.Key == "officePhone").Value,
                MobilePhone = item.FieldValues.FirstOrDefault(x => x.Key == "mobilePhone").Value,
                Photo = item.FieldValues.FirstOrDefault(x => x.Key == "photo").Value,
                AboutMe = item.FieldValues.FirstOrDefault(x => x.Key == "aboutMe").Value,
                ApprovalStatus = item.FieldValues.FirstOrDefault(x => x.Key == "approvalStatus").Value,
                ManagerEmail = item.FieldValues.FirstOrDefault(x => x.Key == "managerEmail").Value,
                ManagerName = item.FieldValues.FirstOrDefault(x => x.Key == "managerName").Value,
                EmployeeEmail = item.FieldValues.FirstOrDefault(x => x.Key == "employeeEmail").Value,
                EmployeeName = item.FieldValues.FirstOrDefault(x => x.Key == "employeeName").Value,
                Comment = item.FieldValues.FirstOrDefault(x => x.Key == "comment").Value,
                ReviewDate = item.FieldValues.FirstOrDefault(x => x.Key == "reviewDate").Value,
                Created = DateTimeOffset.Parse(item.FieldValues.FirstOrDefault(x => x.Key == "Created").Value).ToString("MM-dd-yyyy hh:mm tt"),
                Modified = DateTimeOffset.Parse(item.FieldValues.FirstOrDefault(x => x.Key == "Modified").Value).ToString("MM-dd-yyyy hh:mm tt"),
            };
        }

        public Profile ToProfile()
        {
            return new Profile
            {
                AboutMe = AboutMe,
                Base64Photo = Photo,
                BusinessPhone = OfficePhone,
                City = City,
                Country = CountryRegion,
                MobilePhone = MobilePhone,
                Office = Office,
                PostalCode = ZipPostalCode,
                State = StateProvince,
                Street = StreetAddress
            };
        }

        public static ProfileApprovalItem FromProfile(Profile profile)
        {
            return new ProfileApprovalItem
            {
                AboutMe = profile.AboutMe,
                Photo = profile.Base64Photo,
                OfficePhone = profile.BusinessPhone,
                City = profile.City,
                CountryRegion = profile.Country,
                MobilePhone = profile.MobilePhone,
                Office = profile.Office,
                ZipPostalCode = profile.PostalCode,
                StateProvince = profile.State,
                StreetAddress = profile.Street,
                ApprovalStatus = API.ApprovalStatus.PENDING.ToString()
            };
        }
    }
}
