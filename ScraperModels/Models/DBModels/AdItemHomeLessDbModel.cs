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
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        [NotMapped]
        public List<ExcelImageModel> Images { get; set; }
        public string Description { get; set; }
        public string Field0 { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string Field6 { get; set; }
        public string Size { get; set; }
        public string Floor { get; set; }
        public string Contact { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string AgencyName { get; set; }
        public string Address { get; set; }
        public string LinkToProfile { get; set; }

        public AdItemHomeLessDbModel FromDomain(AdItemHomeLessDomainModel item)
        {
            AdItemId = item.Id;
            DateUpdated = item.DateUpdated;
            City = item.City;
            Region = item.Region;
            Phone = item.Phone;
            Latitude = item.Latitude;
            Longitude = item.Longitude;
            Images = item.Images;
            Description = item.Description;
            Field0 = item.Field0;
            Field1 = item.Field1;
            Field2 = item.Field2;
            Field3 = item.Field3;
            Field4 = item.Field4;
            Field5 = item.Field5;
            Field6 = item.Field6;
            Size = item.Size;
            Floor = item.Floor;
            Contact = item.Contact;
            Phone1 = item.Phone1;
            Phone2 = item.Phone2;
            AgencyName = item.AgencyName;
            Address = item.Address;
            LinkToProfile = item.LinkToProfile;

            return this;
        }
    }
}
