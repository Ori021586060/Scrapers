using OfficeOpenXml;
using OfficeOpenXml.Style;
using ScraperModels.Models;
using ScraperModels.Models.Excel;
using ScraperServices.Models.Komo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScraperServices.Services
{
    public class ExcelKomoService: ExcelServiceBase, IExcelService
    {
        public ExcelKomoService(IStateExcelService state): base(state)
        {
        }

        public MemoryStream CreateExcel(DataScrapeModel data)
        {
            _log($"Start create excel data");

            MemoryStream result = null;

            var items = (List<ExcelRowKomoModel>)data.Data;
            var amountDataCols = 0;
            var hasAmountImages = 1;
            _log($"Amount input items: {items.Count}");

            using (ExcelPackage eP = new ExcelPackage())
            {
                eP.Workbook.Properties.Author = "Scrap";
                eP.Workbook.Properties.Title = "Scrap Data";
                eP.Workbook.Properties.Company = "Komo";

                var sheet = eP.Workbook.Worksheets.Add($"Komo");

                var row = 1;
                var col = 1;

                col = 1;
                // head line
                sheet.Cells[row, col++].Value = "ItemId";
                sheet.Cells[row, col++].Value = "Updated";
                sheet.Cells[row, col++].Value = "Latitude";
                sheet.Cells[row, col++].Value = "Longitude";
                sheet.Cells[row, col++].Value = "ContactName";
                //sheet.Cells[row, col++].Value = "Phone1";
                //sheet.Cells[row, col++].Value = "Phone2";
                var descriptionDataCol = col;
                sheet.Cells[row, col++].Value = "Description";
                sheet.Cells[row, col++].Value = "Price";
                sheet.Cells[row, col++].Value = "PropertyType";
                sheet.Cells[row, col++].Value = "Rooms";
                sheet.Cells[row, col++].Value = "Floor";
                sheet.Cells[row, col++].Value = "Square";
                sheet.Cells[row, col++].Value = "CheckHour";
                sheet.Cells[row, col++].Value = "Extras";
                amountDataCols = col;
                sheet.Cells[row, col++].Value = "Link";

                sheet.Cells[row, 1, row, col - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[1, 1, 1, amountDataCols-1].AutoFilter = true;

                row++; col = 1;

                foreach (var item in items)
                {
                    col = 1;
                    sheet.Cells[row, col++].Value = item.TagId_;
                    _addCellDate(sheet.Cells[row, col++], item.Updated);

                    sheet.Cells[row, col++].Value = item.Latitude;
                    sheet.Cells[row, col++].Value = item.Longitude;

                    sheet.Cells[row, col++].Value = item.ContactName;
                    //sheet.Cells[row, col++].Value = item.Phone1;
                    //sheet.Cells[row, col++].Value = item.Phone2;
                    sheet.Cells[row, col++].Value = item.Description;
                    sheet.Cells[row, col++].Value = item.Price;
                    sheet.Cells[row, col++].Value = item.PropertyType;
                    sheet.Cells[row, col++].Value = item.Rooms;
                    sheet.Cells[row, col++].Value = item.Floor;
                    sheet.Cells[row, col++].Value = item.Square;
                    sheet.Cells[row, col++].Value = item.CheckHour;
                    sheet.Cells[row, col++].Value = item.Extras;
                    // link
                    var url = $"https://www.komo.co.il/code/nadlan/?modaaNum={item.TagId_}";
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

                var startPositionOnFileLinks = amountDataCols+1;
                var endPositionOnFileLinks = startPositionOnFileLinks + hasAmountImages - 1;

                foreach (var i in Enumerable.Range(startPositionOnFileLinks, hasAmountImages))
                {
                    sheet.Cells[1, i].Value = $"Images {i - startPositionOnFileLinks + 1}";
                }

                foreach (var i in Enumerable.Range(3, row)) sheet.Row(i).Height = 15;

                result = new MemoryStream(eP.GetAsByteArray());
            }

            _log("Excel data create done");

            return result;
        }
    }
}
