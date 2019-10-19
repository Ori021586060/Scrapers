using Newtonsoft.Json.Linq;
using ScraperModels.Models.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScraperModels.Models.HomeLess
{
    public class Phase4XmlDtoModel
    {
        public AdDetailsDtoModel AdDetails { get; set; }
        public DetailsItemDtoModel ItemDetails { get; set; }
        public AdDtoModel RowDataFromPage { get; set; }
        public ExcelRowHomeLessModel ToDomainModel()
        {
            var images = new List<ExcelImageModel>();

            if (AdDetails?.Images != null && AdDetails?.Images.Count>0)
            {
                images = (AdDetails?.Images).Select(x => new ExcelImageModel() { Full = x }).ToList();
            }

            var noData = "-";
            var values = AdDetails?.BoolValues;
            var extraValues = AdDetails?.ExtraValues;
            var itemId = AdDetails.ID;

            var item = new ExcelRowHomeLessModel()
            {
                ItemId = itemId,
                DateUpdated = AdDetails.DateUpdated,
                City = RowDataFromPage?.City,
                Region = RowDataFromPage?.Region,
                Phone = AdDetails.Phone,
                Images = images,
                Description = AdDetails.Description,
                Field0 = values.Where(x=>x.Name== "סורגים").Select(x=>x.IsOn).FirstOrDefault(),
                Field1 = values.Where(x => x.Name == "לשותפים").Select(x => x.IsOn).FirstOrDefault(),
                Field2 = values.Where(x => x.Name == "ריהוט").Select(x => x.IsOn).FirstOrDefault(),
                Field3 = values.Where(x => x.Name == "מעלית").Select(x => x.IsOn).FirstOrDefault(),
                Field4 = values.Where(x => x.Name == "מרפסת").Select(x => x.IsOn).FirstOrDefault(),
                Field5 = values.Where(x => x.Name == "חניה").Select(x => x.IsOn).FirstOrDefault(),
                Field6 = values.Where(x => x.Name == "מזגן").Select(x => x.IsOn).FirstOrDefault(),
                Size = extraValues.Where(x => x.Name == "מ\"ר").Select(x => x.Value).FirstOrDefault(),
                Floor = extraValues.Where(x => x.Name == "קומה").Select(x => x.Value).FirstOrDefault(),
                Contact = extraValues.Where(x => x.Name == "איש קשר").Select(x => x.Value).FirstOrDefault(),
                AgencyName = extraValues.Where(x => x.Name == "שם הסוכנות").Select(x => x.Value).FirstOrDefault(),
                Phone1 = extraValues.Where(x => x.Name == "טלפון 1").Select(x => x.Value).FirstOrDefault(),
                Phone2 = extraValues.Where(x => x.Name == "טלפון 2").Select(x => x.Value).FirstOrDefault(),
                Address = extraValues.Where(x => x.Name == "כתובת").Select(x => x.Value).FirstOrDefault(),
                Latitude = noData,
                Longitude = noData,
                LinkToProfile = $"https://www.homeless.co.il/rent/{RowDataFromPage.TypeItem.ToString()}/viewad,{itemId}.aspx",
            };

            return item;
        }
    }
}
