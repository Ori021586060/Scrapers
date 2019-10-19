using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.KomoDto
{
    public class StreetsDtoModel
    {
        public string status { get; set; }
        public string response { get; set; }
        public List<StreetsDataDtoModel> data { get; set; }
    }
}
