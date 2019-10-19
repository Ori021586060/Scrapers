using Newtonsoft.Json;

namespace ScraperModels.Models.HomeLess
{
    public class PostDetailsDDtoModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("tablocation")]
        public string Tablocation { get; set; }
    }
}