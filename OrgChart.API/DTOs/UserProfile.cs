using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public Profile Profile { get; set; }

    }
}
