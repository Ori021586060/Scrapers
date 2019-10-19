using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class GeomDto
    {
        [JsonProperty("code")]
        public GeomCodeDto Code { get; set; }

        [JsonProperty("name")]
        public GeomNameDto Name { get; set; }

        [JsonProperty("id")]
        public GeomIdDto Id {get;set;}
    }
}
