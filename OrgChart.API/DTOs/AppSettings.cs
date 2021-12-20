using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class AppSettings
    {
        public string ManagersGroupId { get; set; }
        public string ManagersGroupMail { get; set; }
        public string SearchFilterSuffix { get; set; }
    }
}
