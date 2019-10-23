using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperServices.Models
{
    public class WorkStatusModel
    {
        public bool IsWorking { get; set; } = false;
        public string StateString { get; set; } = "default";
        public DateTime? StartWork { get; set; } = null;
        public DateTime? EndWork { get; set; } = null;
    }
}
