using OfficeOpenXml;
using OfficeOpenXml.Style;
using ScraperModels.Models;
using ScraperModels.Models.Excel;
using ScrapModels.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ScraperServices.Services
{
    public class ExcelYad2Service : ExcelServiceBase, IExcelService
    {
        public ExcelYad2Service(IStateExcelService state) : base(state)
        {
        }
        public MemoryStream CreateExcel(DataScrapeModel data)
        {
            MemoryStream result = null;

            var items = (List<ExcelRowYad2Model>)data.Data ?? new List<ExcelRowYad2Model>();
            var amountDataCols = 0;
            var hasAmountImages = 1;
            _log($"Amount input items: {items.Count}");

            using (ExcelPackage eP = new ExcelPackage())
            {
                eP.Workbook.Properties.Author = "Scrap";
                eP.Workbook.Properties.Title = "Scrap Data";
                eP.Workbook.Properties.Company = "Yad2";

                var sheet = eP.Workbook.Worksheets.Add($"Yad2");

                var row = 1;
                var col = 1;

                // head line
                sheet.Cells[row, col++].Value = "Date Create";
                sheet.Cells[row, col++].Value = "Date Update";
                sheet.Cells[row, col++].Value = "City";
                sheet.Cells[row, col++].Value = "HouseNumber";
                sheet.Cells[row, col++].Value = "Neighborhood";
                sheet.Cells[row, col++].Value = "StreetName";
                sheet.Cells[row, col++].Value = "Latitude";
                sheet.Cells[row, col++].Value = "Longitude";
                sheet.Cells[row, col++].Value = "AriaBase";
                sheet.Cells[row, col++].Value = "Balconies";
                sheet.Cells[row, col++].Value = "Pets";
                sheet.Cells[row, col++].Value = "Elevators";
                sheet.Cells[row, col++].Value = "FloorOn";
                sheet.Cells[row, col++].Value = "FloorOf";
                sheet.Cells[row, col++].Value = "Rooms";
                sheet.Cells[row, col++].Value = "Parking";
                //sheet.Cells[row, col++].Value = "ContactEmail";
                sheet.Cells[row, col++].Value = "ContactName";
                sheet.Cells[row, col++].Value = "ContactPhone";
                sheet.Cells[row, col++].Value = "Price";
                sheet.Cells[row, col++].Value = "Description";
                sheet.Cells[row, col++].Value = "PropertyType";
                sheet.Cells[row, col++].Value = "AirConditioner";
                sheet.Cells[row, col].Value = "Link";

                //sheet.Cells[row, 1, row, 23].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells["A1:V1"].AutoFilter = true;

                row++; col = 1;
                

                foreach (var item in items)
                {
                    col = 1;

                    _addCellDate(sheet.Cells[row, col], item.DateCreate); col++;

                    _addCellDate(sheet.Cells[row, col], item.DateUpdate); col++;

                    sheet.Cells[row, col++].Value = item.HeCity;

                    _addCellInt(sheet.Cells[row, col], item.HeHouseNumber); col++;

                    sheet.Cells[row, col++].Value = item.HeNeighborhood;
                    sheet.Cells[row, col++].Value = item.HeStreetName;
                    sheet.Cells[row, col++].Value = item.Latitude;
                    sheet.Cells[row, col++].Value = item.Longitude;

                    _addCellInt(sheet.Cells[row, col], item.AriaBase); col++;

                    _addCellInt(sheet.Cells[row, col], item.Balconies); col++;

                    sheet.Cells[row, col++].Value = item.Pets;
                    sheet.Cells[row, col++].Value = item.Elevators;

                    _addCellInt(sheet.Cells[row, col], item.FloorOn); col++;

                    _addCellInt(sheet.Cells[row, col], item.FloorOf); col++;

                    _addCellInt(sheet.Cells[row, col], item.Rooms); col++;

                    sheet.Cells[row, col++].Value = item.Parking;
                    //sheet.Cells[row, col++].Value = item.ContactEmail;
                    sheet.Cells[row, col++].Value = item.ContactName;
                    sheet.Cells[row, col++].Value = item.ContactPhone;
                    sheet.Cells[row, col++].Value = item.Price;
                    sheet.Cells[row, col++].Value = item.Description;
                    sheet.Cells[row, col++].Value = item.PropertyType;
                    sheet.Cells[row, col++].Value = item.AirConditioner;
                    // link
                    var url = $"https://www.yad2.co.il/item/{item.Id}";
                    sheet.Cells[row, col].Value = url;
                    sheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[row, col].Hyperlink = new Uri(url);

                    amountDataCols = col++;

                    if (item?.Images != null)
                    {
                        var i = 1;
                        foreach (var image in item.Images)
                        {
                            _addCellLinks(sheet.Cells[row, col], image, i++);
                            col++;
                        }

                        if (item.Images.Count > hasAmountImages) hasAmountImages = item.Images.Count;
                    }

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

                sheet.Column(20).Width = 50;

                var startPositionOnFileLinks = 24;
                var endPositionOnFileLinks = startPositionOnFileLinks + hasAmountImages - 1;

                foreach (var i in Enumerable.Range(startPositionOnFileLinks, hasAmountImages))
                {
                    sheet.Cells[1, i].Value = $"Images {i - startPositionOnFileLinks + 1}";
                }

                foreach (var i in Enumerable.Range(2, row)) sheet.Row(i).Height = 15;

                result = new MemoryStream(eP.GetAsByteArray());
            }

            return result;
        }
    }
}
