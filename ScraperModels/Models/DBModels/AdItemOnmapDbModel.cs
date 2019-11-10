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
    public class AdItemOnmapDbModel : BaseDbModel
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
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float? AriaBase { get; set; }
        public int? Balconies { get; set; }
        public string Bathrooms { get; set; }
        public string Elevators { get; set; }
        public int? FloorOn { get; set; }
        public int? FloorOf { get; set; }
        public int? Rooms { get; set; }
        public string Toilets { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Description { get; set; }
        public float? Price { get; set; }
        public string PropertyType { get; set; }
        public string Section { get; set; }
        public List<string> Images { get; set; }
        //public List<string> Videos { get; set; }

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
            Latitude = (double)item.Latitude.ConvertToDouble();
            Longitude = (double)item.Longitude.ConvertToDouble();
            AriaBase = item.AriaBase.ConvertToFloat();
            Balconies = item.Balconies.ConvertToInt();
            Bathrooms = item.Bathrooms;
            Elevators = item.Elevators;
            FloorOn = item.FloorOn.ConvertToInt();
            FloorOf = item.FloorOf.ConvertToInt();
            Rooms = item.Rooms.ConvertToInt();
            Toilets = item.Toilets;
            ContactEmail = item.ContactEmail;
            ContactName = item.ContactName;
            ContactPhone = item.ContactPhone;
            Description = item.Description;
            Price = item.Price.ConvertToFloat();
            PropertyType = item.PropertyType;
            Section = item.Section;
            Images = item.Images.Select(x=>x.Full).ToList();
            //Videos = item.Videos.Select(x => x.Source).ToList();

            return this;
        }
    }
}
