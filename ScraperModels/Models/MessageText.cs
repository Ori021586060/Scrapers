using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public static class MessageText
    {
        public static Dictionary<EnumErrorCode, string> Text { get;set; } = new Dictionary<EnumErrorCode, string>(){
            {EnumErrorCode.NoConnectToSolenoid, "No connect to web-driver"},
            {EnumErrorCode.Airdna_CityExistInDictionary, "City exist in dictionary"},
            {EnumErrorCode.Airdna_InvalidUrlForParsing, "Invalid URL for parsing" },
            {EnumErrorCode.Airdna_CannotDetectCityId, "Cannot detect CityId" },
            };
    }
}
