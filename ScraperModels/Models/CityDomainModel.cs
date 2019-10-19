using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CityDomainModel
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string CityOriginalName { get; set; }

        [JsonProperty]
        public AirdnaModel Airdna { get; set; }
    }
}
