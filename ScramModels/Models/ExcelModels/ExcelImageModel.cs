using ScraperModels.Model;
using ScraperModels.Models.OnmapDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{ 
    public class ExcelImageModel
    {
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public string Full { get; set; }
        public ExcelImageModel() { }
        public ExcelImageModel(IImageDto image)
        {
            Description = image?.description;
            Thumbnail = image?.thumbnail;
            Full = image?.full;
        }
    }
}
