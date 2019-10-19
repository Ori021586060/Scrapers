using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Yad2Dto
{
    public class Phase3ObjectContactsData
    {
        public string contact_name { get; set; }
        public List<Phase3ObjectContactsDataPhoneNumbers> phone_numbers { get; set; }
        public string favorites_userid { get; set; }
        public string is_virtual_phone { get; set; }
        public string email { get; set; }
    }
}
