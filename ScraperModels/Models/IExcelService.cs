using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScraperModels.Models
{
    public interface IExcelService
    {
        MemoryStream CreateExcel(DataDomainModel data);
    }
}
