using ScraperModels.Models.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Excel
{
    public class AdItemYad2ExcelModel
    {
        public string Id { get; set; }
        public string DateCreate { get; set; }
        public string DateUpdate { get; set; }
        public string HeCity { get; set; }
        public string HeHouseNumber { get; set; }
        public string HeNeighborhood { get; set; }
        public string HeStreetName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string AriaBase { get; set; }
        public string Balconies { get; set; }
        public string Pets { get; set; }
        public string Elevators { get; set; }
        public string FloorOn { get; set; }
        public string FloorOf { get; set; }
        public string Rooms { get; set; }
        public string Parking { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string PropertyType { get; set; }
        public string AirConditioner { get; set; }
        public List<string> Images { get; set; }

        public AdItemYad2ExcelModel FromDomain(AdItemYad2DomainModel itemDomain) {
            Id = itemDomain.Id;
            DateCreate = itemDomain.DateCreate;
            DateUpdate = itemDomain.DateUpdate;
            HeCity = itemDomain.HeCity;
            HeHouseNumber = itemDomain.HeHouseNumber;
            HeNeighborhood = itemDomain.HeNeighborhood;
            HeStreetName = itemDomain.HeStreetName;
            Latitude = itemDomain.Latitude;
            Longitude = itemDomain.Longitude;
            AriaBase = itemDomain.AriaBase;
            Balconies = itemDomain.Balconies;
            Pets = itemDomain.Pets;
            Elevators = itemDomain.Elevators;
            FloorOn = itemDomain.FloorOn;
            FloorOf = itemDomain.FloorOf;
            Rooms = itemDomain.Rooms;
            Parking = itemDomain.Parking;
            ContactEmail = itemDomain.ContactEmail;
            ContactName = itemDomain.ContactName;
            ContactPhone = itemDomain.ContactPhone;
            Description = itemDomain.Description;
            Price = itemDomain.Price;
            PropertyType = itemDomain.PropertyType;
            AirConditioner = itemDomain.AirConditioner;
            Images = itemDomain.Images;
            return this;
        }
    }
}
