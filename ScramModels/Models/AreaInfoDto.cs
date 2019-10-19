using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class AreaInfoDto
    {
        [JsonProperty("geom")]
        public GeomDto Geom { get; set; }
    }
}
