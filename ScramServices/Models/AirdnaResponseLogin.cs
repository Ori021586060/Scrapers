using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperServices.Models
{
    public class ResponseLogin
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("expires")]
        public string Expires { get; set; }
    }
}
