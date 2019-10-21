using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScraperModels.Models.HomeLess;

namespace ScraperModels.Models.Domain
{
    public class AdItemHomeLessDomainModel
    {
        public string ItemId { get; set; }
        public string DateUpdated { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Phone { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
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

        public AdItemHomeLessDomainModel FromDto(DetailsItemDtoModel dto)
        {
            var noData = "-";
            var values = dto.AdDetails?.BoolValues;
            var extraValues = dto.AdDetails?.ExtraValues;
            var itemId = dto.AdDetails.ID;

            ItemId = itemId;
            DateUpdated = dto.RowDataFromPage.DateUpdated;
            City = dto.RowDataFromPage?.City;
            Region = dto.RowDataFromPage?.Region;
            Phone = dto.AdDetails.Phone;
            Images = dto.AdDetails.Images.Select(x=>new ExcelImageModel() { Full=x }).ToList();
            Description = dto.AdDetails.Description;
            Field0 = values.Where(x => x.Name == "סורגים").Select(x => x.IsOn).FirstOrDefault();
            Field1 = values.Where(x => x.Name == "לשותפים").Select(x => x.IsOn).FirstOrDefault();
            Field2 = values.Where(x => x.Name == "ריהוט").Select(x => x.IsOn).FirstOrDefault();
            Field3 = values.Where(x => x.Name == "מעלית").Select(x => x.IsOn).FirstOrDefault();
            Field4 = values.Where(x => x.Name == "מרפסת").Select(x => x.IsOn).FirstOrDefault();
            Field5 = values.Where(x => x.Name == "חניה").Select(x => x.IsOn).FirstOrDefault();
            Field6 = values.Where(x => x.Name == "מזגן").Select(x => x.IsOn).FirstOrDefault();
            Size = extraValues.Where(x => x.Name == "מ\"ר").Select(x => x.Value).FirstOrDefault();
            Floor = extraValues.Where(x => x.Name == "קומה").Select(x => x.Value).FirstOrDefault();
            Contact = extraValues.Where(x => x.Name == "איש קשר").Select(x => x.Value).FirstOrDefault();
            AgencyName = extraValues.Where(x => x.Name == "שם הסוכנות").Select(x => x.Value).FirstOrDefault();
            Phone1 = extraValues.Where(x => x.Name == "טלפון 1").Select(x => x.Value).FirstOrDefault();
            Phone2 = extraValues.Where(x => x.Name == "טלפון 2").Select(x => x.Value).FirstOrDefault();
            Address = extraValues.Where(x => x.Name == "כתובת").Select(x => x.Value).FirstOrDefault();
            Latitude = dto.Coordinates?.FirstOrDefault()?.lat;
            Longitude = dto.Coordinates?.FirstOrDefault()?.lon;
            LinkToProfile = $"https://www.homeless.co.il/rent/{dto.RowDataFromPage.TypeItem.ToString()}/viewad,{itemId}.aspx";

            return this;
        }
    }
}
