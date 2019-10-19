using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class Formats
    {
        public static string LogDateFormat(DateTime date)
        {
            var result = date.ToString("dd.MM.yy HH:mm:ss");

            return result;
        }

        public static string LogRow(DateTime date, string col2, string col3, string message)
        {
            var result = $"{LogDateFormat(date)} | {col2,-10}| {col3,-12}| {message}";

            return result;
        }

        public static string LogRow(DateTime date, TimeSpan spentTime, string col2, string col3, string message)
        {
            var spentTimeAround = Math.Round(spentTime.TotalHours, 1);
            var totalHours = "";
            if (spentTimeAround>=0.1)
                totalHours = $"/{spentTimeAround}";
            var result = $"{LogDateFormat(date)}{totalHours} | {col2,-10}| {col3,-14}| {message}";

            return result;
        }

        public static string LogRow(DateTime date, string col2, string message)
        {
            var result = $"{LogDateFormat(date)} | {col2,-24}| {message}"; 

            return result;
        }

        public static string LogRow(DateTime date, TimeSpan spentTime, string col2, string message)
        {
            var totalHours = spentTime.TotalHours;
            var result = $"{LogDateFormat(date)}/{totalHours:#.#} | {col2,-24}| {message}";

            return result;
        }
    }
}
