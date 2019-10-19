using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Yad2Dto
{
    public class Phase3ObjectContactsDto
    {
        public string api_version { get; set; }
        public Phase3ObjectContactsData data { get; set; }
        public string status_code { get; set; }
        public string error_message { get; set; }
        public string server_number { get; set; }
    }
}
