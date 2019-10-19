namespace ScraperModels.Models.KomoDto
{
    public class DataCoordinatesResultGeometryDtoModel
    {
        public DataCoordinatesResultGeometryBoundsDtoModel bounds { get; set; }
        public DataCoordinatesLatLng location {get;set;} 
        public string location_type { get; set; }
        public DataCoordinatesResultGeometryBoundsDtoModel viewport { get; set; }
    }
}