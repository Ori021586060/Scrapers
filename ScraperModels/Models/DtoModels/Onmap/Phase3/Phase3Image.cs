using ScraperModels.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.OnmapDto
{
    public class Phase3Image: IImageDto
    {
        public string description { get; set; }
        public string full { get; set; }
        public string thumbnail { get; set; }
    }
}

