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
    public class AdItemHomeLessDbModel
    {
        [Key]
        public int Id { get; set; }
        public string AdItemId { get; set; }
        public string DateUpdated { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<string> Images { get; set; }
        public string Description { get; set; }
        public string WindowBars { get; set; }      // Field0
        public string RoomMatesAllow { get; set; }  // Field1
        public string Furnitures { get; set; }      // Field2
        public string Elevator { get; set; }        // Field3
        public string Balcony { get; set; }         // Field4
        public string Parking { get; set; }         // Field5
        public string Conditioner { get; set; }     // Field6
        public float? Size { get; set; }
        public int? Floor { get; set; }
        public string Contact { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string AgencyName { get; set; }
        public string Address { get; set; }
        public float? Price { get; set; }
        public string LinkToProfile { get; set; }

        public AdItemHomeLessDbModel FromDomain(AdItemHomeLessDomainModel item)
        {
            AdItemId = item.Id;
            DateUpdated = item.DateUpdated;
            City = item.City;
            Region = item.Region;
            Phone = item.Phone;
            Latitude = (double)item.Latitude.ConvertToDouble();
            Longitude = (double)item.Longitude.ConvertToDouble();
            Images = item.Images.Select(x=>x.Full).ToList();
            Description = item.Description;
            WindowBars = item.Field0;
            RoomMatesAllow = item.Field1;
            Furnitures = item.Field2;
            Elevator = item.Field3;
            Balcony = item.Field4;
            Parking = item.Field5;
            Conditioner = item.Field6;
            Size = item.Size.ConvertToFloat();
            Floor = item.Floor.GetFirstDigit().ConvertToInt();
            Contact = item.Contact;
            Phone1 = item.Phone1;
            Phone2 = item.Phone2;
            AgencyName = item.AgencyName;
            Address = item.Address;
            Price = 1;
            LinkToProfile = item.LinkToProfile;

            return this;
        }
    }
}
