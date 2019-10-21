using ScraperModels.Models.Domain;
using ScraperModels.Models.OnmapDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ScraperModels.Models.Db
{
    public class AdItemOnmapDbModel
    {
        [Key]
        public int Id { get; set; }
        public string AdItemId { get; set; }
        public string DateCreate { get; set; }
        public string DateUpdate { get; set; }
        public string EnCity { get; set; }
        public string EnHouseNumber { get; set; }
        public string EnNeighborhood { get; set; }
        public string EnStreetName { get; set; }
        public string HeCity { get; set; }
        public string HeHouseNumber { get; set; }
        public string HeNeighborhood { get; set; }
        public string HeStreetName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string AriaBase { get; set; }
        public string Balconies { get; set; }
        public string Bathrooms { get; set; }
        public string Elevators { get; set; }
        public string FloorOn { get; set; }
        public string FloorOf { get; set; }
        public string Rooms { get; set; }
        public string Toilets { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string PropertyType { get; set; }
        public string Section { get; set; }
        [NotMapped]
        public List<ExcelImageModel> Images { get; set; }
        [NotMapped]
        public List<ExcelVideoModel> Videos { get; set; }

        public AdItemOnmapDbModel FromDomain(AdItemOnmapDomainModel item)
        {
            AdItemId = item.Id;
            DateCreate = item.DateCreate;
            DateUpdate = item.DateUpdate;
            EnCity = item.EnCity;
            EnHouseNumber = item.EnHouseNumber;
            EnNeighborhood = item.EnNeighborhood;
            EnStreetName = item.EnStreetName;
            HeCity = item.HeCity;
            HeHouseNumber = item.HeHouseNumber;
            HeNeighborhood = item.HeNeighborhood;
            HeStreetName = item.HeStreetName;
            Latitude = item.Latitude;
            Longitude = item.Longitude;
            AriaBase = item.AriaBase;
            Balconies = item.Balconies;
            Bathrooms = item.Bathrooms;
            Elevators = item.Elevators;
            FloorOn = item.FloorOn;
            FloorOf = item.FloorOf;
            Rooms = item.Rooms;
            Toilets = item.Toilets;
            ContactEmail = item.ContactEmail;
            ContactName = item.ContactName;
            ContactPhone = item.ContactPhone;
            Description = item.Description;
            Price = item.Price;
            PropertyType = item.PropertyType;
            Section = item.Section;
            Images = item.Images;
            Videos = item.Videos;

            return this;
        }
    }
}
