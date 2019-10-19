using ScraperModels.Models.OnmapDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class ExcelVideoModel
    {
        public string Description { get; set; }
        public string Id { get; set; }
        public string Source { get; set; }
        public ExcelVideoModel(Phase3Video video)
        {
            Description = video?.description;
            Id = video?.id;
            Source = video?.source;
        }
    }
}
