using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.Models
{
    public enum EnumJobs
    {
        ScrapeNewThenSaveStore = 1,
        ScrapeContinueThenSaveStore,
        GenerateExcelThenSaveStore,
        StatusWorkspace,
    }
}
