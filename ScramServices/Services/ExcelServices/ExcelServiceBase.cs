using OfficeOpenXml;
using ScraperModels.Models;
using ScrapModels.Models;
using System;
using System.IO;

namespace ScraperServices.Services
{
    public class ExcelServiceBase
    {
        protected IStateExcelService _state { get; set; }
        public ExcelServiceBase(IStateExcelService state)
        {
            _state = state;
            _state.WorkPhase = "ExcelService";
        }
        public string SaveToFile(Stream stream, string filename = "")
        {
            _log($"Start saving excel data to file");

            if (string.IsNullOrEmpty(filename)) filename = _state.ExcelFilename;

            var pathFilename = $"{filename}";

            using (var fileStream = File.Create(pathFilename))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }

            stream.Close();

            FileInfo fileInfo = new FileInfo(pathFilename);
            var fullPath = fileInfo.FullName;

            _log($"Save excel data to file:{fullPath}");

            return fullPath;
        }
        protected void _log(string message)
        {
            var logMessage = Formats.LogRow(DateTime.UtcNow, _state.SpentTime, _state.TypeScraper.ToString(), _state.WorkPhase, message);

            Console.WriteLine(logMessage);
        }
        protected void _addCellLinks(ExcelRange cell, string image, int i)
        {
            cell.Value = $"{image}";
            //cell.Vale.WrapText = true;
            //cell.Hyperlink = new Uri(image);
        }
        protected void _addCellInt(ExcelRange cell, string value)
        {
            int dateValue;
            bool isInt;

            isInt = int.TryParse(value, out dateValue);
            if (isInt)
            {
                cell.Style.Numberformat.Format = "0";
                cell.Value = dateValue;
            }
            else
                cell.Value = value;
        }
        protected void _addCellDate(ExcelRange cell, string value)
        {
            DateTime dateValue;
            bool isDate;

            isDate = DateTime.TryParse(value, out dateValue);
            if (isDate)
            {
                cell.Style.Numberformat.Format = "dd.MM.yyyy";
                cell.Value = dateValue.ToString("dd.MM.yyyy");
            }
            else
                cell.Value = value;
        }
        protected void _addCellFloat(ExcelRange cell, string value)
        {
            float dateValue;
            bool isfloat;

            isfloat = float.TryParse(value, out dateValue);
            if (isfloat)
            {
                cell.Style.Numberformat.Format = "0.0";
                cell.Value = dateValue;
            }
            else
                cell.Value = value;
        }
    }
}