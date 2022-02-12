using Microsoft.Graph;
using OrgChart.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.Services
{
    public partial class MicrosoftGraphService
    {
        // get profile photo
        private async Task<string> GetUserPhoto(string userId)
        {
            try
            {
                var client = await GetGraphServiceClient();

                var photoStream = await client.Users[userId]
                    .Photos["240x240"]
                    .Content
                    .Request()
                    .GetAsync();
                var photoBytes = new byte[photoStream.Length];
                await photoStream.ReadAsync(photoBytes);
                return Convert.ToBase64String(photoBytes);
            }
            catch (ServiceException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }


        // update profile photo
        private async Task UpdateUserPhoto(string userId, string base64Photo)
        {
            var photoBytes = Convert.FromBase64String(base64Photo);

            var client = await GetGraphServiceClient();

            using var stream = new System.IO.MemoryStream(photoBytes);

            await client.Users[userId].Photo.Content
                .Request()
                .PutAsync(stream);

        }


        // get user profile
        private async Task<UserProfile> GetUserProfile(string userId)
        {
            var client = await GetGraphServiceClient();

            var user = await client.Users[userId].Request()
                 .Select(u => new
                 {
                     u.Id,
                     u.UserPrincipalName,
                     u.AboutMe,
                     u.BusinessPhones,
                     u.City,
                     u.Country,
                     u.MobilePhone,
                     u.OfficeLocation,
                     u.PostalCode,
                     u.State,
                     u.StreetAddress
                 })
                .GetAsync();

            return new UserProfile
            {
                Profile = Profile.FromUser(user),
                Email = user.UserPrincipalName,
                Id = user.Id
            };
        }


        // update user profile
        private async Task UpdateUserProfile(string userId, Profile profile)
        {
            var client = await GetGraphServiceClient();

            var user = profile.ToUser();

            await client.Users[userId]
                .Request()
                .UpdateAsync(user);

        }

        // update user about me
        private async Task UpdateUserAboutMe(string userId, Profile profile)
        {
            var client = await GetGraphServiceClient();

            var user = profile.ToAboutMe();

            await client.Users[userId]
                .Request()
                .UpdateAsync(user);

        }

       

    }
}
