using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class ManagerUpdateRequest
    {
        public IEnumerable<string> userIds { get; set; }
        public string managerId { get; set; }
        public string userId { get; set; }
    }
}
