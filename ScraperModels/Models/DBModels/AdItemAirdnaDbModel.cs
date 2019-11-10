using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using ScraperModels.Models.Domain;
using ScraperModels.Models.HomeLess;

namespace ScraperModels.Models.Db
{
    public class AdItemAirdnaDbModel: BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public string AdItemId { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double? Adr { get; set; }
        public double? Rating { get; set; }
        public float? Bathrooms { get; set; }
        public float? Bedrooms { get; set; }
        public string Accommodates { get; set; }
        public string Revenue { get; set; }
        public string PropertyType { get; set; }
        public string Occ { get; set; }
        public float? Reviews { get; set; }
        public string RoomType { get; set; }
        public string LinkToProfile { get; set; }

        public AdItemAirdnaDbModel FromDomain(AdItemAirdnaDomainModel item)
        {
            AdItemId = item.Id;
            Title = item.Title;
            Location = item.Location;
            Longitude = (double)item.Longitude.ConvertToDouble(); 
            Latitude = (double)item.Latitude.ConvertToDouble();
            Adr = item.Adr.ConvertToFloat();
            Rating = item.Rating.ConvertToFloat();
            Bathrooms = item.Bathrooms.ConvertToFloat();
            Bedrooms = item.Bedrooms.ConvertToFloat();
            Accommodates = item.Accommodates;
            Revenue = item.Revenue;
            PropertyType = item.PropertyType;
            Occ = item.Occ;
            Reviews = item.Reviews.ConvertToFloat();
            RoomType = item.RoomType;
            LinkToProfile = item.LinkToProfile; 

            return this;
        }
    }
}
