﻿using Newtonsoft.Json;
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
    public class AdItemYad2DbModel
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string AdItemId { get; set; }
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

        public AdItemYad2DbModel FromDomain(AdItemYad2DomainModel item)
        {
            AdItemId = item.Id;
            DateCreate = item.DateCreate;
            DateUpdate = item.DateUpdate;
            HeCity = item.HeCity;
            HeHouseNumber = item.HeHouseNumber;
            HeNeighborhood = item.HeNeighborhood;
            HeStreetName = item.HeStreetName;
            Latitude = item.Latitude;
            Longitude = item.Longitude;
            AriaBase = item.AriaBase;
            Balconies = item.Balconies;
            Pets = item.Pets;
            Elevators = item.Elevators;
            FloorOn = item.FloorOn;
            FloorOf = item.FloorOf;
            Rooms = item.Rooms;
            Parking = item.Parking;
            ContactEmail = item.ContactEmail;
            ContactName = item.ContactName;
            ContactPhone = item.ContactPhone;
            Description = item.Description;
            Price = item.Price;
            PropertyType = item.PropertyType;
            AirConditioner = item.AirConditioner;
            Images = item.Images;

            return this;
        }
    }
}