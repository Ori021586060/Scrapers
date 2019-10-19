using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class RequestInfoDto
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
