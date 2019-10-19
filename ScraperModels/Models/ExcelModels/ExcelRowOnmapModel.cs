using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Excel
{
    public class ExcelRowOnmapModel
    {
        public string TagId_ { get; set; }
        public string DateCreate { get; set; }
        public string DateUpdate { get; set; }
        public string EnCity { get; set; }
        public string EnHouseNumber { get; set; }
        public string EnNeighborhood { get; set; }
        public string EnStreetName { get; set; }
        public string HeCity { get; set; }
        public string HeHouseNumber { get; set; }
        public string HeNeighborhood { get; set; }
        public string HeStreetName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string AriaBase { get; set; }
        public string Balconies { get; set; }
        public string Bathrooms { get; set; }
        public string Elevators { get; set; }
        public string FloorOn { get; set; }
        public string FloorOf { get; set; }
        public string Rooms { get; set; }
        public string Toilets { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string PropertyType { get; set; }
        public string Section { get; set; }
        public List<ExcelImageModel> Images { get; set; }
        public List<ExcelVideoModel> Videos { get; set; }
    }
}
