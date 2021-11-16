using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class ADManager
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public ADManager Manager { get; set; }

        public ADManagerItem ToManagerItem()
        {
            return new ADManagerItem
            {
                Id = Id,
                DisplayName = DisplayName
            };
        }
    }

    public class ADManagerItem
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
    }
}
