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
                BusinessPhones = new List<string> {
                BusinessPhone
                },
                MobilePhone = MobilePhone,
                OfficeLocation = Office,
                StreetAddress = Street,
                PostalCode = PostalCode,
                City = City,
                State = State,
                Country = Country
            };
        }

        public User ToAboutMe()
        {
            return new User
            {
                AboutMe = AboutMe
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
