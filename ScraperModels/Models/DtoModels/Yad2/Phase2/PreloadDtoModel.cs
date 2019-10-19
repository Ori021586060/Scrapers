using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Yad2Dto
{
    public class PreloadDtoModel
    {
        [JsonProperty(PropertyName = "feed")]
        public PreloadFeedDtoModel Feed { get; set; }

        [JsonProperty(PropertyName = "pagination")]
        public PreloadPaginationDtoModel Pagination { get; set; }
    }
}
