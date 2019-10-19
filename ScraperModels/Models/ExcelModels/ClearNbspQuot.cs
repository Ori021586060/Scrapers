using System.Text.RegularExpressions;

namespace ScraperModels.Models.Excel
{
    public static class StringExtension
    {
        public static string ClearSymbols(this string str)
        {
            var result = str?.Replace("&nbsp;", "").Replace("&quot;", "").Replace("&rsquo;", "’").Replace("&nbsp", "");

            return result;
        }
        public static string ClearFullTrim(this string str)
        {
            var result = str;
            if (!string.IsNullOrEmpty(str))
            {
                var str1 = str?.Replace("\r", "").Replace("\n", "").Trim();
                result = Regex.Replace(str, "[ ]+", " ");
            }

            return result;
        }
        public static string RemoveNotDigits(this string str)
        {
            var result = str;

            if (!string.IsNullOrEmpty(str))
            {
                result = Regex.Replace(str, @"[^\d\.]+", "");
            }

            return result;
        }
    }
}