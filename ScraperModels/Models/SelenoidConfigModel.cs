using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class SelenoidConfigModel
    {
        public string Address { get; set; } = "selenoid.localnet";
        public string Protocol { get; set; } = "http";
        public bool Enabled { get; set; } = true;
    }
}
