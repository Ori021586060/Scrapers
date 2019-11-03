using Newtonsoft.Json;
using ScraperModels.Models.Yad2Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScraperModels.Models.Domain
{
    public class AdItemYad2DomainModel
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

        public AdItemYad2DomainModel FromDto(Phase3ObjectDto item, Phase3ObjectContactsDto itemContacts)
        {
            var dateCreate = item?.date_added;
            var dateUpdate = item?.date_of_entry?.Replace("/", ".");
            var heCity = item?.city_text;
            var heHouseNumber = item?.address_home_number;
            var heNeighborhood = item?.neighborhood;
            var heStreetName = item?.street;
            var ariaBase = item?.square_meters;
            var balconies = item?.balconies;
            var pets = item?.additional_info_items_v2?.Where(x => x.key == "pets" && x.value == "true")
                        .Select(x => x.value).FirstOrDefault()?? "false";
            var elevators = item?.additional_info_items_v2?.Where(x => x.key == "elevator" && x.value == "true")
                        .Select(x => x.value).FirstOrDefault()?? "false";
            var floorOn = item?.info_bar_items?.Where(x => x.key == "floor").Select(x => x.titleWithoutLabel).FirstOrDefault();
            var floorOf = item?.TotalFloor_text;
            var rooms = item?.info_bar_items?.Where(x => x.key == "rooms").Select(x => x.titleWithoutLabel).FirstOrDefault();
            var parking = item?.additional_info_items_v2?.Where(x => x.key == "parking" && x.value == "true")
                        .Select(x => x.value).FirstOrDefault()??"false";
            var contactEmail = itemContacts?.data.email;
            var contactName = itemContacts?.data.contact_name;
            var contactPhone = itemContacts?.data.phone_numbers?.FirstOrDefault()?.title;
            var description = item?.info_text;
            var price = item?.price;
            var propertyType = item?.media?.@params?.AppType;
            var airConditioner = item?.additional_info_items_v2?.Where(x => x.key == "air_conditioner" && x.value == "true")
                        .Select(x => x.value).FirstOrDefault();
            var images = item?.images;

            var coordinates = item.navigation_data;

            if (coordinates != null)
            {
                var json = JsonConvert.SerializeObject(coordinates);
                Phase3NavigationData navigationData = null;
                try
                {
                    navigationData = JsonConvert.DeserializeObject<Phase3NavigationData>(json);

                    this.Latitude = navigationData.coordinates.latitude;
                    this.Longitude = navigationData.coordinates.longitude;
                }
                catch (Exception exception)
                {
                    ;
                }
            }

            Id = item.id;
            DateCreate = dateCreate;
            DateUpdate = dateUpdate;
            HeCity = heCity;
            HeHouseNumber = heHouseNumber;
            HeNeighborhood = heNeighborhood;
            HeStreetName = heStreetName;
            AriaBase = ariaBase;
            Balconies = balconies;
            Pets = pets;
            Elevators = elevators;
            FloorOn = floorOn;
            FloorOf = floorOf;
            Rooms = rooms;
            Parking = parking;
            ContactEmail = contactEmail;
            ContactName = contactName;
            ContactPhone = contactPhone;
            Description = description;
            Price = price;
            PropertyType = propertyType;
            AirConditioner = airConditioner;
            Images = images;

            return this;
        }
    }
}
