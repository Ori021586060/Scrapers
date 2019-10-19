using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class AirdnaScrapeDataModel
    {
        [JsonProperty("listings_limited")]
        public bool ListingsLimited { get; set; }

        [JsonProperty("request_info")]
        public RequestInfoDto RequestInfo { get; set; }

        [JsonProperty("properties")]
        public List<PropertyDto> Properties { get; set; }

        [JsonProperty("listings_returned")]
        public int ListingsReturned { get; set; }

        [JsonProperty("area_info")]
        public AreaInfoDto AreaInfo { get; set; }
    }
}
