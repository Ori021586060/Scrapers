using ScraperModels.Models.Excel;
using ScraperModels.Models.WinWinDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScraperModels.Models.Domain
{
    public class AdItemWinWinDomainModel
    {
        public string TagId_ { get; set; }
        public string DateUpdate { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string StreetAddress { get; set; }
        public string Rooms { get; set; }
        public string Floor { get; set; }
        public string State { get; set; }
        public string DateEnter { get; set; }
        public string Square { get; set; }
        public string IsPartners { get; set; }
        public string AmountPayment { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string IsAgent { get; set; }
        public string ContactName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public List<ExcelImageModel> Images { get; set; }

        public AdItemWinWinDomainModel()
        {
        }

        public AdItemWinWinDomainModel FromDto(AdItemWinWinDtoModel itemDto)
        {
            TagId_ = itemDto.ItemId;
            DateUpdate = itemDto.DateUpdate;
            Longitude = itemDto.Longitude;
            Latitude = itemDto.Latitude;
            City = itemDto.City;
            Area = itemDto.Area;
            StreetAddress = itemDto?.StreetAddress?.ClearSymbols();
            Rooms = itemDto.Rooms;
            Floor = itemDto.Floor;
            State = itemDto.State;
            DateEnter = itemDto.DateEnter;
            Square = itemDto?.Square?.ClearSymbols();
            IsPartners = itemDto.IsPartners;
            AmountPayment = itemDto.AmountPayment;
            Description = itemDto.Description?.ClearSymbols();
            Price = itemDto.Price;
            IsAgent = itemDto.IsAgent;
            ContactName = itemDto.ContactName;
            Phone1 = itemDto.Phone1;
            Phone2 = itemDto.Phone2;
            Images = itemDto.Images?.Select(x => new ExcelImageModel() { Full = x }).ToList() ?? new List<ExcelImageModel>();

            return this;
        }
    }
}
