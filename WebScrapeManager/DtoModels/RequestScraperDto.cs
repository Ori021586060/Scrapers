using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraperManager.DtoModels
{
    public class RequestScraperDto
    {
        public List<ScraperDto> UseScrapers { get; set; }
        public int CityId { get; set; }
    }
}
