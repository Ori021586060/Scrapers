using OfficeOpenXml;
using OfficeOpenXml.Style;
using ScraperCore;
using ScraperModels.Models;
using ScraperModels.Models.Excel;
using ScraperServices.Models.HomeLess;
using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ScraperModels.Models.Domain;

namespace ScraperServices.Services
{
    public class ExcelHomeLessService: ExcelServiceBase, IExcelService
    {
        public ExcelHomeLessService(IStateExcelService state): base(state)
        {
        }

        public MemoryStream CreateExcel(DataScrapeModel data)
        {
            _log($"Start create excel data");

            MemoryStream result = null;

            var itemsDomainModel = (List<AdItemHomeLessDomainModel>)data.Data;
            var items = new List<AdItemHomeLessExcelModel>();
            foreach (var item in itemsDomainModel) items.Add(new AdItemHomeLessExcelModel().FromDomain(item));

            var amountDataCols = 0;
            var hasAmountImages = 1;
            _log($"Amount input items: {items.Count}");

            using (ExcelPackage eP = new ExcelPackage())
            {
                eP.Workbook.Properties.Author = "Scrap";
                eP.Workbook.Properties.Title = "Scrap Data";
                eP.Workbook.Properties.Company = "HomeLess";

                var sheet = eP.Workbook.Worksheets.Add($"HomeLess");

                var row = 1;
                var col = 1;

                // head line
                sheet.Cells[row, col++].Value = "ItemId";
                sheet.Cells[row, col++].Value = "Date Update";
                sheet.Cells[row, col++].Value = "Latitude";
                sheet.Cells[row, col++].Value = "Longitude";
                sheet.Cells[row, col++].Value = "City";
                sheet.Cells[row, col++].Value = "Region";
                var descriptionDataCol = col;
                sheet.Cells[row, col++].Value = "Address";
                //sheet.Cells[row, col++].Value = "Description";
                sheet.Cells[row, col++].Value = "Floor";
                sheet.Cells[row, col++].Value = "Size";
                sheet.Cells[row, col++].Value = "AgencyName";
                sheet.Cells[row, col++].Value = "Contact";
                sheet.Cells[row, col++].Value = "Phone";
                sheet.Cells[row, col++].Value = "Phone1";
                sheet.Cells[row, col++].Value = "Phone2";
                sheet.Cells[row, col++].Value = "סורגים";
                sheet.Cells[row, col++].Value = "לשותפים";
                sheet.Cells[row, col++].Value = "ריהוט";
                sheet.Cells[row, col++].Value = "מעלית";
                sheet.Cells[row, col++].Value = "מרפסת";
                sheet.Cells[row, col++].Value = "חניה";
                sheet.Cells[row, col++].Value = "מזגן";
                amountDataCols = col;
                sheet.Cells[row, col++].Value = "Link";

                sheet.Cells[row, 1, row, col - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[1, 1, 1, amountDataCols - 1].AutoFilter = true;

                row++; col = 1;

                foreach (var item in items)
                {
                    col = 1;
                    sheet.Cells[row, col++].Value = item.ItemId;
                    _addCellDate(sheet.Cells[row, col], item.DateUpdated); col++;

                    sheet.Cells[row, col++].Value = item.Latitude;
                    sheet.Cells[row, col++].Value = item.Longitude;
                    sheet.Cells[row, col++].Value = item.City;
                    sheet.Cells[row, col++].Value = item.Region;
                    sheet.Cells[row, col++].Value = item.Address;

                    //sheet.Cells[row, col++].Value = item.Description;

                    _addCellInt(sheet.Cells[row, col], item.Floor); col++;

                    _addCellInt(sheet.Cells[row, col], item.Size); col++;

                    sheet.Cells[row, col++].Value = item.AgencyName;
                    sheet.Cells[row, col++].Value = item.Contact;
                    sheet.Cells[row, col++].Value = item.Phone;
                    sheet.Cells[row, col++].Value = item.Phone1;
                    sheet.Cells[row, col++].Value = item.Phone2;

                    sheet.Cells[row, col++].Value = item.Field0;
                    sheet.Cells[row, col++].Value = item.Field1;
                    sheet.Cells[row, col++].Value = item.Field2;
                    sheet.Cells[row, col++].Value = item.Field3;
                    sheet.Cells[row, col++].Value = item.Field4;
                    sheet.Cells[row, col++].Value = item.Field5;
                    sheet.Cells[row, col++].Value = item.Field6;
                    // link
                    var url = $"{item.LinkToProfile}";
                    sheet.Cells[row, col].Value = url;
                    sheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[row, col].Hyperlink = new Uri(url);
                    col++;

                    var i = 1;
                    foreach (var image in item.Images)
                    {
                        _addCellLinks(sheet.Cells[row, col], image.Full, i++);
                        col++;
                    }

                    if (item.Images.Count > hasAmountImages) hasAmountImages = item.Images.Count;

                    row++;
                }
                col--;

                using (var cells = sheet.Cells[sheet.Cells[1, 1, 1 + items.Count, amountDataCols + hasAmountImages].Address])
                {
                    cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    cells.AutoFitColumns();
                }

                sheet.Column(descriptionDataCol).Width = 50;

                var startPositionOnFileLinks = amountDataCols + 1;
                var endPositionOnFileLinks = startPositionOnFileLinks + hasAmountImages - 1;

                foreach (var i in Enumerable.Range(startPositionOnFileLinks, hasAmountImages))
                {
                    //sheet.Column(i).Width = 5;
                    sheet.Cells[1, i].Value = $"Images {i - startPositionOnFileLinks + 1}";
                }

                foreach (var i in Enumerable.Range(2, row)) sheet.Row(i).Height = 15;

                result = new MemoryStream(eP.GetAsByteArray());
            }

            _log("Excel data create done");

            return result;
        }
    }
}
