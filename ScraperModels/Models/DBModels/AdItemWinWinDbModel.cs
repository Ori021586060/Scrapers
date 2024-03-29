﻿using ScraperModels.Models.Domain;
using ScraperModels.Models.WinWinDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ScraperModels.Models.Db
{
    public class AdItemWinWinDbModel : BaseDbModel
    {
        [Required]
        [Key]
        public int id { get; set; }
        public string AdItemId { get; set; }
        public string DateUpdate { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string StreetAddress { get; set; }
        public float? Rooms { get; set; }
        public int? Floor { get; set; }
        public string State { get; set; }
        public string DateEnter { get; set; }
        public string Square { get; set; }
        public string IsPartners { get; set; }
        public string AmountPayment { get; set; }
        public string Description { get; set; }
        public double? Price { get; set; }
        public string IsAgent { get; set; }
        public string ContactName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public List<string> Images { get; set; }

        public AdItemWinWinDbModel FromDomain(AdItemWinWinDomainModel item)
        {
            AdItemId = item.Id;
            DateUpdate = item.DateUpdate;
            Longitude = item.Longitude.ConvertToDouble();
            Latitude = item.Latitude.ConvertToDouble();
            City = item.City;
            Area = item.Area;
            StreetAddress = item.StreetAddress;
            Rooms = item.Rooms.ConvertToFloat();
            Floor = item.Floor.GetFirstDigit().ConvertToInt();
            State = item.State;
            DateEnter = item.DateEnter;
            Square = item?.Square;
            IsPartners = item.IsPartners;
            AmountPayment = item.AmountPayment;
            Description = item.Description;
            Price = item.Price.ClearDigits().ConvertToFloat();
            IsAgent = item.IsAgent;
            ContactName = item.ContactName;
            Phone1 = item.Phone1;
            Phone2 = item.Phone2;
            Images = item.Images.Select(x=>x.Full).ToList();

            return this;
        }
    }
}
