using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrgChart.API.DTOs
{
    public class GetUserIdResponse:ResponseBase
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}
