using System;
using System.Collections.Generic;

namespace ScraperServices.Models
{
    public class BaseStatusModel: IStatus
    {
        public WorkStatusModel WorkStatus { get; set; } = null;

        // scrape ext
        public Dictionary<string, string> ValueStatus { get; set; } = new Dictionary<string, string>();

        // scrape
        public DateTime ScrapeDate { get; set; }
        public int AmountItemsFromPath { get; set; }
        public int AmountItemsWithWrongDataFromPath { get; set; }
        public int AmountItemUniquesFromPages { get; set; }
        public int AmountPages { get; set; }
        public int AmountItemsFromPages { get; set; }
        public int AmountItemDuplicatesFromPages { get => AmountItemsFromPages - AmountItemUniquesFromPages; }
    }
}