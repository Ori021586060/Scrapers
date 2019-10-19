using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.OnmapDto
{
    public class AdditionalInfoModel
    {
        public string rooms { get; set; }
        public AdditionalInfoFloorModel floor { get; set; }
        public AdditionalInfoAreaModel area { get; set; }
        public AdditionalInfoParkingModel parking { get; set; }
        public string bathrooms { get; set; }
    }
}
