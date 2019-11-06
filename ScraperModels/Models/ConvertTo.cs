using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public static class ConvertTo
    {
        public static int? ConvertToInt(this string str)
        {
            int? result = null;

            try
            {
                result = Int32.Parse(str);
            }
            catch
            {
                ;
            }

            return result;
        }

        public static double? ConvertToDouble(this string str)
        {
            double? result = null;

            try
            {
                result = double.Parse(str);
            }
            catch
            {
                ;
            }

            return result;
        }

        public static float? ConvertToFloat(this string str)
        {
            float? result = null;

            try
            {
                result = float.Parse(str);
            }
            catch
            {
                ;
            }

            return result;
        }

        public static string GetFirstDigit(this string str)
        {
            var result = "";

            try
            {
                var d1 = str.Split(" ");
                result = d1[0];
            }
            catch
            {
                ;
            }

            return result;
        }

        public static string ClearDigits(this string str)
        {
            var result = "";
            var validChars = "1234567890,.";

            if (!string.IsNullOrWhiteSpace(str))
            try
            {
                foreach(var ch in str)
                {
                    if (validChars.Contains(ch)) result += ch;
                }
            }
            catch (Exception exception)
            {
                ;
            }

            return result;
        }
    }
}
