using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class ManagerUpdateRequest
    {
        public IEnumerable<string> UserIds { get; set; }
        public string ManagerId { get; set; }
        public string UserId { get; set; }
    }
}
