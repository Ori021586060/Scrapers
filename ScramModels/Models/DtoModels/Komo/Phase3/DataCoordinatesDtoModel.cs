using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.KomoDto
{
    public class DataCoordinatesDtoModel
    {
        public List<DataCoordinatesResultDtoModel> results { get; set; }
        public string status { get; set; }
    }
}
