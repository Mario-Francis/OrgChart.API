using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class ODataResponse
    {
        [JsonPropertyName("@odata.context")]
        public string Context { get; set; }
        [JsonPropertyName("@odata.count")]
        public int Count { get; set; }
        public IEnumerable<User> Value { get; set; }
    }
}
