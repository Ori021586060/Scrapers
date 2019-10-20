using OfficeOpenXml;
using OfficeOpenXml.Style;
using ScraperModels.Models;
using ScraperModels.Models.Excel;
using ScraperServices.Models.Onmap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ScraperServices.Services
{
    public class ExcelOnmapService: ExcelServiceBase, IExcelService
    {
        public ExcelOnmapService(IStateExcelService state):base(state)
        {
        }
        public MemoryStream CreateExcel(DataScrapeModel data)
        {
            MemoryStream result = null;

            var items = (List<ExcelRowOnmapModel>)data.Data;
            var amountDataCols = 0;
            var hasAmountImages = 1;
            _log($"Amount input items: {items.Count}");

            using (ExcelPackage eP = new ExcelPackage())
            {
                eP.Workbook.Properties.Author = "Scrap";
                eP.Workbook.Properties.Title = "Scrap Data";
                eP.Workbook.Properties.Company = "Onmap";

                var sheet = eP.Workbook.Worksheets.Add($"Onmap");

                var row = 1;
                var col = 1;

                // head line
                sheet.Cells[row, col++].Value = "ItemId";
                sheet.Cells[row, col++].Value = "DateCreate";
                sheet.Cells[row, col++].Value = "DateUpdate";
                sheet.Cells[row, col++].Value = "Latitude";
                sheet.Cells[row, col++].Value = "Longitude";
                sheet.Cells[row, col++].Value = "EnCity";
                sheet.Cells[row, col++].Value = "EnHouseNumber";
                sheet.Cells[row, col++].Value = "EnNeighborhood";
                sheet.Cells[row, col++].Value = "EnStreetName";
                sheet.Cells[row, col++].Value = "HeCity";
                sheet.Cells[row, col++].Value = "HeHouseNumber";
                sheet.Cells[row, col++].Value = "HeNeighborhood";
                sheet.Cells[row, col++].Value = "HeStreetName";
                sheet.Cells[row, col++].Value = "AriaBase";
                sheet.Cells[row, col++].Value = "Balconies";
                sheet.Cells[row, col++].Value = "Bathrooms";
                sheet.Cells[row, col++].Value = "Elevators";
                sheet.Cells[row, col++].Value = "FloorOn";
                sheet.Cells[row, col++].Value = "FloorOf";
                sheet.Cells[row, col++].Value = "Rooms";
                sheet.Cells[row, col++].Value = "ContactEmail";
                sheet.Cells[row, col++].Value = "ContactName";
                sheet.Cells[row, col++].Value = "ContactPhone";
                sheet.Cells[row, col++].Value = "Price";
                var descriptionDataCol = col;
                sheet.Cells[row, col++].Value = "Description";
                sheet.Cells[row, col++].Value = "PropertyType";
                sheet.Cells[row, col++].Value = "Section";
                amountDataCols = col;
                sheet.Cells[row, col++].Value = "Link";

                sheet.Cells[row, 1, row, col-1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[1, 1, 1, amountDataCols - 1].AutoFilter = true;

                row++; col = 1;

                foreach (var item in items)
                {
                    col = 1;
                    sheet.Cells[row, col++].Value = item.TagId_;
                    _addCellDate(sheet.Cells[row, col], item.DateCreate); col++;
                    _addCellDate(sheet.Cells[row, col], item.DateUpdate); col++;
                    sheet.Cells[row, col++].Value = item.Latitude;
                    sheet.Cells[row, col++].Value = item.Longitude;
                    sheet.Cells[row, col++].Value = item.EnCity;

                    _addCellInt(sheet.Cells[row, col], item.EnHouseNumber); col++;

                    sheet.Cells[row, col++].Value = item.EnNeighborhood;
                    sheet.Cells[row, col++].Value = item.EnStreetName;
                    sheet.Cells[row, col++].Value = item.HeCity;

                    _addCellInt(sheet.Cells[row, col], item.HeHouseNumber); col++;

                    sheet.Cells[row, col++].Value = item.HeNeighborhood;
                    sheet.Cells[row, col++].Value = item.HeStreetName;

                    _addCellInt(sheet.Cells[row, col], item.AriaBase); col++;

                    _addCellInt(sheet.Cells[row, col], item.Balconies); col++;

                    _addCellInt(sheet.Cells[row, col], item.Bathrooms); col++;

                    _addCellInt(sheet.Cells[row, col], item.Elevators); col++;

                    _addCellInt(sheet.Cells[row, col], item.FloorOn); col++;

                    _addCellInt(sheet.Cells[row, col], item.FloorOf); col++;

                    _addCellFloat(sheet.Cells[row, col], item.Rooms); col++;

                    sheet.Cells[row, col++].Value = item.ContactEmail;
                    sheet.Cells[row, col++].Value = item.ContactName;
                    sheet.Cells[row, col++].Value = item.ContactPhone;

                    _addCellInt(sheet.Cells[row, col], item.Price); col++;
                    sheet.Cells[row, col++].Value = item.Description;
                    sheet.Cells[row, col++].Value = item.PropertyType;
                    sheet.Cells[row, col++].Value = item.Section;

                    sheet.Cells[row, col].Value = $"https://www.onmap.co.il/?id=property_{item.TagId_}";
                    sheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[row, col].Hyperlink = new Uri($"https://www.onmap.co.il/?id=property_{item.TagId_}");
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
                    sheet.Cells[1, i].Value = $"Images {i-startPositionOnFileLinks+1}";
                }

                foreach (var i in Enumerable.Range(3, row)) sheet.Row(i).Height = 15;

                result = new MemoryStream(eP.GetAsByteArray());
            }

            return result;
        }
    }
}
