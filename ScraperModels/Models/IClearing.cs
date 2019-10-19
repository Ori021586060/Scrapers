using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public interface IClearing
    {
        string ClearSymbols(string str);
        string ClearFullTrim(string str);

        string ClearNotDigits(string str);
    }
}
