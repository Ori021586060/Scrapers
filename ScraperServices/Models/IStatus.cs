using System;

namespace ScraperServices.Models
{
    public interface IStatus
    {
        DateTime ScrapeDate { get; set; }
        int AmountItemsFromPath { get; set; }
        int AmountPages { get; set; }
        int AmountItemsFromPages { get; set; }
        int AmountItemDuplicatesFromPages { get; }
        int AmountItemUniquesFromPages { get; set; }
    }
}