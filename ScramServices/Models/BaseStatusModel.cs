using System;

namespace ScraperServices.Models
{
    public class BaseStatusModel: IStatus
    {
        public DateTime ScrapeDate { get; set; }
        public int AmountItemsFromPath { get; set; }
        public int AmountItemsWithWrongDataFromPath { get; set; }
        public int AmountItemUniquesFromPages { get; set; }
        public int AmountPages { get; set; }
        public int AmountItemsFromPages { get; set; }
        public int AmountItemDuplicatesFromPages { get => AmountItemsFromPages - AmountItemUniquesFromPages; }
    }
}