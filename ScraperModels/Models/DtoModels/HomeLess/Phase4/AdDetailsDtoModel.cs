using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.HomeLess
{
    public class AdDetailsDtoModel
    {
        public string ID { get; set; }
        public string DateUpdated { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        //public AdDetailsImagesDtoModel Images { get; set; }
        public List<string> Images { get; set; }
        public object VideoUrl { get; set; }
        public string AgencyName { get; set; }
        //public string SEODescription { get; set; }
        //public string SEOTItle { get; set; }
        //public AdDetailsBoolValuesDtoModel BoolValues { get; set; } 
        public List<AdDetailsBoolValuesAdBoolValuesDtoModel> BoolValues { get; set; } 
        public string FullAddress { get; set; }
        //public AdDetailsExtraValuesDtoModel ExtraValues { get; set; } 
        public List<AdDetailsExtraValuesAdExtraDetailsRowDtoModel> ExtraValues { get; set; } 
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}
