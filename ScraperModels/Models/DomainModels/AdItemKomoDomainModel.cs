using ScraperModels.Models.Excel;
using ScraperModels.Models.KomoDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScraperModels.Models.Domain
{
    public class AdItemKomoDomainModel
    {
        public string Id { get; set; }
        public string Updated { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ContactName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string PropertyType { get; set; }
        public string Rooms { get; set; }
        public string Floor { get; set; }
        public string Square { get; set; }
        public string CheckHour { get; set; }
        public string Extras { get; set; }
        public List<ExcelImageModel> Images { get; set; }

        public AdItemKomoDomainModel FromDto(ItemKomoDtoModel itemDto)
        {
            var location = itemDto?.DataCoordinates?.results?.FirstOrDefault()?.geometry?.location;

            Id = itemDto.Id;
            Updated = itemDto?.DataPage?.Updated.ClearSymbols().ClearFullTrim();
            Latitude = location?.lat;
            Longitude = location?.lng;
            ContactName = itemDto.DataPage.ContactName;
            Phone1 = $"{itemDto?.DataContacts?.data?.phone1_pre} - {itemDto?.DataContacts?.data?.phone1}";
            Phone2 = $"{itemDto?.DataContacts?.data?.phone2_pre} - {itemDto?.DataContacts?.data?.phone2}";
            Description = itemDto?.DataPage?.Description.ClearSymbols();
            Price = itemDto?.DataPage?.Price?.Replace("&nbsp;","").Replace("&#8362;","");
            PropertyType = itemDto.DataPage.Minisite;
            Rooms = itemDto?.DataPage?.Rooms.ClearSymbols().RemoveNotDigits();
            Floor = itemDto?.DataPage?.Floor?.ClearFullTrim();
            Square = itemDto?.DataPage?.Square.ClearSymbols();
            CheckHour = itemDto?.DataPage?.CheckHour;
            Extras = itemDto.DataPage?.ExtDescription?.Aggregate((a,b)=> a+","+b).ClearSymbols();
            Images = itemDto.DataPage?.Images?.Select(x => new ExcelImageModel() { Full = x }).ToList() ?? new List<ExcelImageModel>(); 

            return this;
        }
    }
}
