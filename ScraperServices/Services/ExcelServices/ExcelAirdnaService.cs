﻿using OfficeOpenXml;
using OfficeOpenXml.Style;
using ScraperModels.Models;
using ScraperModels.Models.Domain;
using ScraperModels.Models.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ScraperServices.Services
{
    public class ExcelAirdnaService: ExcelServiceBase, IExcelService
    {
        public ExcelAirdnaService(IStateExcelService state) : base(state)
        {
        }

        public MemoryStream CreateExcel(DataDomainModel data)
        {
            _log($"Start create excel data");

            MemoryStream result = null;

            var itemsDomainModel = (List<AdItemAirdnaDomainModel>)data.Data;
            var items = new List<AdItemAirdnaExcelModel>();
            foreach (var item in itemsDomainModel) items.Add(new AdItemAirdnaExcelModel().FromDomain(item));

            _log($"Amount input items: {items.Count}");

            using (ExcelPackage eP = new ExcelPackage())
            {
                eP.Workbook.Properties.Author = "Scrap";
                eP.Workbook.Properties.Title = "Scrap Data";
                eP.Workbook.Properties.Company = "Airdna";

                var sheet = eP.Workbook.Worksheets.Add($"Airdna");

                var row = 1;
                var col = 1;

                // head line
                var descriptionDataCol = col;
                sheet.Cells[row, col++].Value = "Title (link to site)";
                sheet.Cells[row, col++].Value = "Location";
                sheet.Cells[row, col++].Value = "Latitude";
                sheet.Cells[row, col++].Value = "Longitude";
                sheet.Cells[row, col++].Value = "Adr";
                sheet.Cells[row, col++].Value = "Rating";
                sheet.Cells[row, col++].Value = "Bathrooms";
                sheet.Cells[row, col++].Value = "Bedrooms";
                sheet.Cells[row, col++].Value = "Accommodates";
                sheet.Cells[row, col++].Value = "Revenue";
                sheet.Cells[row, col++].Value = "Property Type";
                sheet.Cells[row, col++].Value = "Occ";
                sheet.Cells[row, col++].Value = "Reviews";
                sheet.Cells[row, col++].Value = "RoomType";
                sheet.Cells[row, col++].Value = "Link";

                row++; col = 1;

                foreach (var item in items)
                {
                    col = 1;
                    sheet.Cells[row, col++].Value = item.Title;
                    sheet.Cells[row, col++].Value = item.Location;

                    sheet.Cells[row, col++].Value = item.Latitude;
                    sheet.Cells[row, col++].Value = item.Longitude;
                    sheet.Cells[row, col++].Value = item.Adr;
                    sheet.Cells[row, col++].Value = item.Rating;
                    sheet.Cells[row, col++].Value = item.Bathrooms;
                    sheet.Cells[row, col++].Value = item.Bedrooms;
                    sheet.Cells[row, col++].Value = item.Accommodates;
                    sheet.Cells[row, col++].Value = item.Revenue;
                    sheet.Cells[row, col++].Value = item.PropertyType;
                    sheet.Cells[row, col++].Value = item.Occ;
                    sheet.Cells[row, col++].Value = item.Reviews;
                    sheet.Cells[row, col++].Value = item.RoomType;

                    // link
                    sheet.Cells[row, col].Value = item.LinkToProfile;
                    sheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[row, col].Hyperlink = new Uri(item.LinkToProfile);
                    col++;

                    row++;
                }
                col--;

                sheet.Column(descriptionDataCol).Width = 50;

                result = new MemoryStream(eP.GetAsByteArray());
            }

            _log("Excel data create done");

            return result;
        }

        public Stream DataToExcel(List<DataDomainModel> listDataScrape)
        {
            MemoryStream result = null;

            using (ExcelPackage eP = new ExcelPackage())
            {
                eP.Workbook.Properties.Author = "Ori";
                eP.Workbook.Properties.Title = "Scrap Data";
                eP.Workbook.Properties.Company = "Renta Co";

                foreach (var dataScrape in listDataScrape)
                {
                    switch (dataScrape.Scraper)
                    {
                        case EnumScrapers.Airdna:
                            var data = dataScrape.Data;
                            _addSheet_Airdna(eP, (AirdnaScrapeDataModel)dataScrape.Data);

                            break;
                    }
                }

                result = new MemoryStream(eP.GetAsByteArray());
            }

            return result;
        }
        public Stream CreateExcel(object data)
        {
            var result = new MemoryStream();

            return result;
        }
        private bool _addSheet_Airdna(ExcelPackage eP, AirdnaScrapeDataModel dto)
        {
            var result = true;

            var sheet = eP.Workbook.Worksheets.Add($"Airdna");
            var items = dto.Properties;
            var city = $"{dto.AreaInfo.Geom.Name.City}";

            var row = 1;
            var col = 1;

            sheet.Cells[row, col, row, col + 16].Merge = true;
            sheet.Cells[row, col].Value = $"Scrap data";
            sheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            row++; col = 1;
            // head line
            sheet.Cells[row, col++].Value = "Title (link to site)";
            sheet.Cells[row, col++].Value = "Link to Gmaps";
            sheet.Cells[row, col++].Value = "Link to Panaram";
            sheet.Cells[row, col++].Value = "Location";
            sheet.Cells[row, col++].Value = "Longitude";
            sheet.Cells[row, col++].Value = "Latitude";
            sheet.Cells[row, col++].Value = "Adr";
            sheet.Cells[row, col++].Value = "Rating";
            sheet.Cells[row, col++].Value = "Bathrooms";
            sheet.Cells[row, col++].Value = "Bedrooms";
            sheet.Cells[row, col++].Value = "Accommodates";
            sheet.Cells[row, col++].Value = "Revenue";
            sheet.Cells[row, col++].Value = "Property Type";
            sheet.Cells[row, col++].Value = "Rating";
            sheet.Cells[row, col++].Value = "Occ";
            sheet.Cells[row, col++].Value = "Reviews";
            sheet.Cells[row, col++].Value = "RoomType";
            sheet.Cells[row, 1, row, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            row++; col = 1;

            foreach (var item in items)
            {
                sheet.Cells[row, col].Value = item.Title;
                Uri url;
                double? cellValue;
                if (item.HomeawayPropertyId is null)
                {
                    url = new Uri($"https://www.airbnb.com/rooms/{item.Platforms.AirbnbPropertyId}");
                }
                else
                {
                    url = new Uri($"https://www.homeaway.com/vacation-rental/p{item.Platforms.HomeawayPropertyId}");
                }
                sheet.Cells[row, col++].Hyperlink = url;

                sheet.Cells[row, col].Value = "gmaps";
                sheet.Cells[row, col].Hyperlink = new Uri($"https://www.google.com/maps/search/?api=1&query={item.Latitude},{item.Longitude}");
                sheet.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                sheet.Cells[row, col].Value = "panaram";
                sheet.Cells[row, col].Hyperlink = new Uri($"https://www.google.com/maps/@?api=1&map_action=pano&viewpoint={item.Latitude},{item.Longitude}&heading=90&pitch=38&fov=100");
                sheet.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                sheet.Cells[row, col].Value = city;
                sheet.Cells[row, col].Hyperlink = new Uri($"https://www.airdna.co/vacation-rental-data/app/{dto.AreaInfo.Geom.Code.Country}/default/{dto.AreaInfo.Geom.Code.City}/overview");
                sheet.Cells[row, col++].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                sheet.Cells[row, col++].Value = item.Longitude;
                sheet.Cells[row, col++].Value = item.Latitude;

                cellValue = !string.IsNullOrEmpty(item.Adr) ? double.Parse(item.Adr, CultureInfo.InvariantCulture) : 0;
                sheet.Cells[row, col].Value = cellValue;
                sheet.Cells[row, col++].Style.Numberformat.Format = "0.000";

                cellValue = !string.IsNullOrEmpty(item.Rating) ? double.Parse(item.Rating, CultureInfo.InvariantCulture) : 0;
                sheet.Cells[row, col].Value = cellValue;
                sheet.Cells[row, col++].Style.Numberformat.Format = "0.0";

                cellValue = !string.IsNullOrEmpty(item.Bathrooms) ? double.Parse(item.Bathrooms, CultureInfo.InvariantCulture) : 0;
                sheet.Cells[row, col].Value = cellValue;
                sheet.Cells[row, col++].Style.Numberformat.Format = "0.0";

                cellValue = !string.IsNullOrEmpty(item.Bedrooms) ? double.Parse(item.Bedrooms, CultureInfo.InvariantCulture) : 0;
                sheet.Cells[row, col].Value = cellValue;
                sheet.Cells[row, col++].Style.Numberformat.Format = "0";

                cellValue = !string.IsNullOrEmpty(item.Accommodates) ? double.Parse(item.Accommodates, CultureInfo.InvariantCulture) : 0;
                sheet.Cells[row, col].Value = cellValue;
                sheet.Cells[row, col++].Style.Numberformat.Format = "0";

                if (item.Revenue == "permission_denied")
                {
                    sheet.Cells[row, col++].Value = "Permission Denied";
                }
                else
                {
                    cellValue = !string.IsNullOrEmpty(item.Revenue) ? double.Parse(item.Revenue, CultureInfo.InvariantCulture) : 0;
                    sheet.Cells[row, col].Value = cellValue;
                    sheet.Cells[row, col++].Style.Numberformat.Format = "0";
                }

                sheet.Cells[row, col++].Value = item.PropertyType;

                cellValue = !string.IsNullOrEmpty(item.Rating) ? (double?)double.Parse(item.Rating, CultureInfo.InvariantCulture) : null;
                sheet.Cells[row, col].Value = cellValue;
                sheet.Cells[row, col++].Style.Numberformat.Format = "0.0";

                if (item.Occ == "permission_denied")
                {
                    sheet.Cells[row, col++].Value = "Permission Denied";
                }
                else
                {
                    cellValue = !string.IsNullOrEmpty(item.Occ) ? (double?)double.Parse(item.Occ, CultureInfo.InvariantCulture) : null;
                    sheet.Cells[row, col].Value = cellValue;
                    sheet.Cells[row, col++].Style.Numberformat.Format = "0.00%";
                }

                cellValue = !string.IsNullOrEmpty(item.Reviews) ? (double?)double.Parse(item.Reviews, CultureInfo.InvariantCulture) : null;
                sheet.Cells[row, col].Value = cellValue;
                sheet.Cells[row, col++].Style.Numberformat.Format = "0";

                sheet.Cells[row, col++].Value = item.RoomType;

                row++;
                col = 1;
            }

            using (var cells = sheet.Cells[sheet.Cells[1, 1, 2 + items.Count(), 17].Address])
            {
                cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                cells.AutoFitColumns();
            }

            return result;
        }
    }
}
