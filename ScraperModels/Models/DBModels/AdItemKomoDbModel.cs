using ScraperModels.Models.Domain;
using ScraperModels.Models.KomoDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ScraperModels.Models.Db
{
    public class AdItemKomoDbModel : BaseDbModel
    {
        [Key]
        public int Id { get; set; }
        public string AdItemId { get; set; }
        public string Updated { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ContactName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Description { get; set; }
        public float? Price { get; set; }
        public string PropertyType { get; set; }
        public int? Rooms { get; set; }
        public int? Floor { get; set; }
        public int? Square { get; set; }
        public string CheckHour { get; set; }
        public string Extras { get; set; }
        public List<string> Images { get; set; }

        public AdItemKomoDbModel FromDomain(AdItemKomoDomainModel item)
        {
            AdItemId = item.Id;
            Updated = item.Updated;
            Latitude = (double)item.Latitude.ConvertToDouble();
            Longitude = (double)item.Longitude.ConvertToDouble();
            ContactName = item.ContactName;
            Phone1 = item.Phone1;
            Phone2 = item.Phone2;
            Description = item.Description;
            Price = item.Price.ConvertToFloat();
            PropertyType = item.PropertyType;
            Rooms = item.Rooms.ConvertToInt();
            Floor = item.Floor.ConvertToInt();
            Square = item.Square.ConvertToInt();
            CheckHour = item.CheckHour;
            Extras = item.Extras;
            Images = item.Images.Select(x=>x.Full).ToList();

            return this;
        }

        //public AdItemKomoExcelModel FromDto(ItemKomoDtoModel itemDto)
        //{
        //    var location = itemDto?.DataCoordinates?.results?.FirstOrDefault()?.geometry?.location;

        //    TagId_ = itemDto.Id;
        //    Updated = itemDto?.DataPage?.Updated.ClearSymbols().ClearFullTrim();
        //    Latitude = location?.lat;
        //    Longitude = location?.lng;
        //    ContactName = itemDto.DataPage.ContactName;
        //    Phone1 = $"{itemDto?.DataContacts?.data?.phone1_pre} - {itemDto?.DataContacts?.data?.phone1}";
        //    Phone2 = $"{itemDto?.DataContacts?.data?.phone2_pre} - {itemDto?.DataContacts?.data?.phone2}";
        //    Description = itemDto?.DataPage?.Description.ClearSymbols();
        //    Price = itemDto?.DataPage?.Price?.Replace("&nbsp;", "").Replace("&#8362;", "");
        //    PropertyType = itemDto.DataPage.Minisite;
        //    Rooms = itemDto?.DataPage?.Rooms.ClearSymbols().RemoveNotDigits();
        //    Floor = itemDto?.DataPage?.Floor?.ClearFullTrim();
        //    Square = itemDto?.DataPage?.Square.ClearSymbols();
        //    CheckHour = itemDto?.DataPage?.CheckHour;
        //    Extras = itemDto.DataPage?.ExtDescription?.Aggregate((a, b) => a + "," + b).ClearSymbols();
        //    Images = itemDto.DataPage?.Images?.Select(x => new ExcelImageModel() { Full = x }).ToList() ?? new List<ExcelImageModel>();

        //    return this;
        //}
    }
}
