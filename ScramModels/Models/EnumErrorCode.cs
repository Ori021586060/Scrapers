using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public enum EnumErrorCode
    {
        NoError = 1,

        // Errors
        NoConnectToSolenoid = 500,
        InvalidCityId,
        ErrorByDeleteCity,
        ErrorGenerateExcelFile,

        Airdna_CityExistInDictionary = 570,
        Airdna_InvalidUrlForParsing,
        Airdna_CannotDetectCityId,

    }
}
