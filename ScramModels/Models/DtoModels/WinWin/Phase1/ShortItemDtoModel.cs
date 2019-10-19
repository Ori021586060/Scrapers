using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.WinWinDto
{
    public class ShortItemDtoModel
    {
        public string ItemId { get; set; }
        public string ApportamentType { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string CheckHour { get; set; }
        public string Price { get; set; }
        public string DateUpdate { get; set; }
        public bool Done { get; set; } = false;
    }
}
