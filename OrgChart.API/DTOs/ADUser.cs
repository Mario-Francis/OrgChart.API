using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class ADUser
    {
        public string Id { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Department { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BusinessPhone { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string MobilePhone { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string UserPrincipalName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? AccountEnabled { get; set; }
        public string ManagerId { get; set; }
        [JsonIgnore(Condition =JsonIgnoreCondition.WhenWritingNull)]
        public ADUser Manager { get; set; }

        public static ADUser FromUser(User user)
        {
            return new ADUser
            {
                Id = user.Id,
                GivenName = user.GivenName,
                Surname = user.Surname,
                DisplayName = user.DisplayName,
                Email = user.UserPrincipalName?.ToLower(),
                UserPrincipalName=user.UserPrincipalName?.ToLower(),
                JobTitle = user.JobTitle,
                Department = user.Department,
                BusinessPhone = user.BusinessPhones?.FirstOrDefault(),
                MobilePhone = user.MobilePhone,
                AccountEnabled = user.AccountEnabled,
                ManagerId = user.Manager?.Id,
                Manager = user.Manager==null?null: ADUser.FromUser(user?.Manager as User)
            };
        }

        public ADUser Clone()
        {
            var copy =  (ADUser)this.MemberwiseClone();
            copy.ManagerId = Manager?.Id;
            copy.Email = copy.UserPrincipalName.ToLower();
            return copy;
        }
    }
}
