using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.HomeLess
{
    public class DetailsItemDtoModel
    {
        public List<ScraperModels.Models.CoordinateDtoModel> Coordinates { get; set; }
        public string Phones { get; set; }
        public List<PostDetailsDDtoModel> Details { get; set; }
        public AdDetailsDtoModel AdDetails {get;set;}
        public AdDtoModel RowDataFromPage { get; set; }
    }
}
