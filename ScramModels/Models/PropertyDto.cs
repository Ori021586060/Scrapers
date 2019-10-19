using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class PropertyDto
    {
        [JsonProperty("airbnb_property_id")]
        public int AairbnbPropertyId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("bathrooms")]
        public string Bathrooms { get; set; }

        [JsonProperty("occ")]
        public string Occ { get; set; }

        [JsonProperty("bedrooms")]
        public string Bedrooms { get; set; }

        [JsonProperty("accommodates")]
        public string Accommodates { get; set; }

        [JsonProperty("adr")]
        public string Adr { get; set; }

        [JsonProperty("revenue")]
        public string Revenue { get; set; }

        [JsonProperty("reviews")]
        public string Reviews { get; set; }

        [JsonProperty("homeaway_property_id")]
        public string HomeawayPropertyId { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("m_homeaway_property_id")]
        public string MHomeawayPropertyId { get; set; }

        [JsonProperty("property_type")]
        public string PropertyType { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("room_type")]
        public string RoomType { get; set; }

        [JsonProperty("platforms")]
        public PlatformsDto Platforms { get; set; }
    }
}
