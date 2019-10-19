using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Yad2Dto
{
    public class PreloadPaginationDtoModel
    {
        [JsonProperty(PropertyName = "last_page")]
        public string LastPage { get; set; }

        [JsonProperty(PropertyName = "total_items")]
        public string TotalItems { get; set; }
    }
}
