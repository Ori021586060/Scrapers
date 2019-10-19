using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.HomeLess
{
    public class PostDetailsDtoModel
    {
        [JsonProperty("d")]
        public List<PostDetailsDDtoModel> D { get; set; }
    }
}
