using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class SharePointSettings
    {
        public string SiteUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Tenant { get; set; }
        public string Resource { get; set; }
        public string GrantType { get; set; }
        public string ApprovalList { get; set; }

    }
}
