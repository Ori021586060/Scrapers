using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class GeomIdDto
    {
        [JsonProperty("country")]
        public int? Country { get; set; }

        [JsonProperty("state")]
        public int? State { get; set; }

        [JsonProperty("city")]
        public int? City { get; set; }
    }
}
