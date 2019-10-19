using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AirdnaModel:ICloneable
    {
        [JsonProperty]
        public string CityId { get; set; }

        public string CityOriginalName { get; set; }

        [JsonProperty]
        public string CityName { get; set; }
        public object Clone()
        {
            return new AirdnaModel()
            {
                CityId = this.CityId,
                CityOriginalName = this.CityOriginalName,
                CityName = this.CityName
            };
        }
    }
}
