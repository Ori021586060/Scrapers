using System.Collections.Generic;

namespace ScraperModels.Models.KomoDto
{
    public class DataCoordinatesResultDtoModel
    {
        public List<DataCoordinatesResultAddressComponentDtoModel> address_components { get; set; }
        public string formatted_address { get; set; }
        public DataCoordinatesResultGeometryDtoModel geometry { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }
}