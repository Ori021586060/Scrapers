using ScraperModels.Models.OnmapDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScraperModels.Models.Domain
{
    public class AdItemAirdnaDomainModel
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
        public bool IsValidForDbModel { get => !string.IsNullOrWhiteSpace(Latitude) && !string.IsNullOrWhiteSpace(Longitude); }

        public AdItemAirdnaDomainModel FromDto(PropertyDto dto)
        {
            try
            {
                Id = dto.Id;
                Title = dto.Title;
                //Location = dto.Location;
                Longitude = dto.Longitude;
                Latitude = dto.Latitude;
                Adr = dto.Adr;
                Rating = dto.Rating;
                Bathrooms = dto.Bathrooms;
                Bedrooms = dto.Bedrooms;
                Accommodates = dto.Accommodates;
                Revenue = dto.Revenue;
                PropertyType = dto.PropertyType;
                Occ = dto.Occ;
                Reviews = dto.Reviews;
                RoomType = dto.RoomType;

                var url = $"";
                if (dto.HomeawayPropertyId is null)
                {
                    url = $"https://www.airbnb.com/rooms/{dto.AairbnbPropertyId}";
                }
                else
                {
                    url = $"https://www.homeaway.com/vacation-rental/p{dto.HomeawayPropertyId}";
                }

                LinkToProfile = url;
            }
            catch(Exception exeption)
            {
                ;
            }

            return this;
        }
    }
}
