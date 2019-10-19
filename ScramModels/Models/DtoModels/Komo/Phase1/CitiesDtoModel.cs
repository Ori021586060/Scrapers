using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.KomoDto
{
    public class CitiesDtoModel
    {
        public string status { get; set; }
        public string response { get; set; }
        public List<CitiesDataDtoModel> data { get; set; }
    }
}
