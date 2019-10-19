using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperServices.Models.Komo
{
    public class ScraperKomoStatusModel: BaseStatusModel, IStatus
    {
        public int AmountCities { get; set; }
        public int AmountItemsFromCitiesStreets { get; set; }
    }
}
