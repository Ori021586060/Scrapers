using Newtonsoft.Json;
using ScraperModels.Models.Domain;
using ScraperModels.Models.Yad2Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ScraperModels.Models.Db
{
    [Table("DataYad2")]
    public class AdItemYad2DbModel : BaseDbModel
    {
        [Required]
        [Key]
        public int id { get; set; }
        public string AdItemId { get; set; }
        public string DateCreate { get; set; }
        public string DateUpdate { get; set; }
        public string HeCity { get; set; }
        public int? HouseNumber { get; set; }
        public string Neighborhood { get; set; }
        public string StreetName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? AriaBase { get; set; }
        public int? Balconies { get; set; }
        public string Pets { get; set; }
        public string Elevators { get; set; }
        public int? FloorOn { get; set; }
        public int? FloorOf { get; set; }
        public float? Rooms { get; set; }
        public string Parking { get; set; }
        //public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Description { get; set; }
        public double? Price { get; set; }
        public string PropertyType { get; set; }
        public string AirConditioner { get; set; }
        public List<string> Images { get; set; }

        public AdItemYad2DbModel FromDomain(AdItemYad2DomainModel item)
        {
            AdItemId = item.Id;
            DateCreate = item.DateCreate;
            DateUpdate = item.DateUpdate;
            HeCity = item.HeCity;
            HouseNumber = item.HeHouseNumber.ConvertToInt();
            Neighborhood = item.HeNeighborhood;
            StreetName = item.HeStreetName;
            Latitude = item.Latitude.ConvertToDouble();
            Longitude = item.Longitude.ConvertToDouble();
            AriaBase = item.AriaBase.ConvertToInt();
            Balconies = item.Balconies.ConvertToInt();
            Pets = item.Pets;
            Elevators = item.Elevators;
            FloorOn = item.FloorOn.ConvertToInt();
            FloorOf = item.FloorOf.ConvertToInt();
            Rooms = item.Rooms.ConvertToFloat();
            Parking = item.Parking;
            //ContactEmail = item.ContactEmail;
            ContactName = item.ContactName;
            ContactPhone = item.ContactPhone;
            Description = item.Description;
            Price = item.Price.ClearDigits().ConvertToFloat();
            PropertyType = item.PropertyType;
            AirConditioner = item.AirConditioner;
            Images = item.Images;

            return this;
        }
    }
}
