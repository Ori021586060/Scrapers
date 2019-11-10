using ScraperModels.Models.Domain;
using ScraperModels.Models.OnmapDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScraperModels.Models.Excel
{
    public class AdItemAirdnaExcelModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Adr { get; set; }
        public string Rating { get; set; }
        public string Bathrooms { get; set; }
        public string Bedrooms { get; set; }
        public string Accommodates { get; set; }
        public string Revenue { get; set; }
        public string PropertyType { get; set; }
        public string Occ { get; set; }
        public string Reviews { get; set; }
        public string RoomType { get; set; }
        public string LinkToProfile { get; set; }

        public AdItemAirdnaExcelModel FromDomain(AdItemAirdnaDomainModel item)
        {
            Id = item.Id;
            Title = item.Title;
            Location = item.Location;
            Longitude = item.Longitude;
            Latitude = item.Latitude;
            Adr = item.Adr;
            Rating = item.Rating;
            Bathrooms = item.Bathrooms;
            Bedrooms = item.Bedrooms;
            Accommodates = item.Accommodates;
            Revenue = item.Revenue;
            PropertyType = item.PropertyType;
            Occ = item.Occ;
            Reviews = item.Reviews;
            RoomType = item.RoomType;
            LinkToProfile = item.LinkToProfile;

            return this;
        }
    }
}
