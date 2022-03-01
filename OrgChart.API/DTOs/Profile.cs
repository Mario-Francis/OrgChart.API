using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class Profile
    {
        public string AboutMe { get; set; }
        public string BusinessPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Office { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Base64Photo { get; set; }

        public User ToUser()
        {
            return new User
            {
                BusinessPhones = string.IsNullOrEmpty(BusinessPhone?.Trim())?new List<string>()
                :new List<string> {
                    BusinessPhone
                },
                MobilePhone = string.IsNullOrEmpty(MobilePhone?.Trim()) ? " " : MobilePhone,
                OfficeLocation = string.IsNullOrEmpty(Office?.Trim()) ? " " : Office,
                StreetAddress = string.IsNullOrEmpty(Street?.Trim()) ? " " : Street,
                PostalCode = string.IsNullOrEmpty(PostalCode?.Trim()) ? " " : PostalCode,
                City = string.IsNullOrEmpty(City?.Trim()) ? " " : City,
                State = string.IsNullOrEmpty(State?.Trim()) ? " " : State,
                Country = string.IsNullOrEmpty(Country?.Trim()) ? " " : Country
            };
        }

        public User ToAboutMe()
        {
            return new User
            {
                AboutMe = string.IsNullOrEmpty(AboutMe?.Trim()) ? " " : AboutMe
            };
        }

        public static Profile FromUser(User user)
        {
            return new Profile
            {
                AboutMe = user.AboutMe,
                BusinessPhone = user?.BusinessPhones?.FirstOrDefault(),
                City = user.City,
                Country = user.Country,
                MobilePhone = user.MobilePhone,
                Office = user.OfficeLocation,
                PostalCode = user.PostalCode,
                State = user.State,
                Street = user.StreetAddress
            };
        } 

    }
}
