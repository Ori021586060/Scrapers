using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.KomoDto
{
    public class ItemKomoDtoModel
    {
        public string Id { get; set; }
        public DataPageDtoModel DataPage { get; set; }
        public DataContactsDtoModel DataContacts { get; set; }
        public DataCoordinatesDtoModel DataCoordinates { get; set; }
    }
}
