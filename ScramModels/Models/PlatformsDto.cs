using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class PlatformsDto
    {
        [JsonProperty("airbnb_property_id")]
        public string AirbnbPropertyId { get; set; }

        [JsonProperty("homeaway_property_id")]
        public string HomeawayPropertyId { get; set; }
    }
}
