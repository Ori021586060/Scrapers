using ScraperModels.Models.OnmapDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScraperModels.Models.Excel
{
    public class ExcelRowOnmapModel
    {
        public string TagId_ { get; set; }
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
        public List<ExcelImageModel> Images { get; set; }
        public List<ExcelVideoModel> Videos { get; set; }

        public ExcelRowOnmapModel FromDto(Phase3ObjectDto rowObj)
        {
            TagId_ = rowObj.id;
            DateCreate = rowObj?.created_at;
            DateUpdate = rowObj?.updated_at;
            EnCity = rowObj?.address.en?.city_name;
            EnHouseNumber = rowObj?.address.en?.house_number;
            EnNeighborhood = rowObj?.address.en?.neighborhood;
            EnStreetName = rowObj?.address.en?.street_name;
            HeCity = rowObj?.address.he?.city_name;
            HeHouseNumber = rowObj?.address.he?.house_number;
            HeNeighborhood = rowObj?.address.he?.neighborhood;
            HeStreetName = rowObj?.address.he?.street_name;
            Latitude = rowObj?.address.location?.lat;
            Longitude = rowObj?.address.location?.lon;
            AriaBase = rowObj?.additional_info.area.@base;
            Balconies = rowObj?.additional_info.balconies;
            Bathrooms = rowObj?.additional_info.bathrooms;
            Elevators = rowObj?.additional_info.elevators;
            FloorOn = rowObj?.additional_info.floor.on_the;
            FloorOf = rowObj?.additional_info.floor.out_of;
            Rooms = rowObj?.additional_info.rooms;
            Toilets = rowObj?.additional_info.toilets;
            ContactEmail = rowObj?.contacts.primary.email;
            ContactName = rowObj?.contacts.primary.name;
            ContactPhone = rowObj?.contacts.primary.phone;
            Description = rowObj?.description;
            Price = rowObj?.price;
            PropertyType = rowObj?.property_type;
            Section = rowObj?.section;
            Images = rowObj?.images?.Select(x => new ExcelImageModel(x)).ToList();
            Videos = rowObj?.videos?.Select(x => new ExcelVideoModel(x)).ToList();

            return this;
        }
    }
}
