using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class AirdnaParseModel
    {
        public string CityId { get; set; }
        public string CityOriginalName { get; set; }
        public string CityName { get; set; }
        public static explicit operator AirdnaModel(AirdnaParseModel model)
        {
            return new AirdnaModel()
            {
                CityId = model.CityId,
                CityOriginalName = model.CityOriginalName,
                CityName = model.CityName,
            };
        }
    }
}
