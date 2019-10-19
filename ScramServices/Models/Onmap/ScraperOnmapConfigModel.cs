using ScrapModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperServices.Models.Onmap
{
    public class ScraperOnmapConfigModel
    {
        public List<SelenoidConfigModel> Selenoid { get; set; } = new List<SelenoidConfigModel>() {
            new SelenoidConfigModel(){
                Address = "selenoid-service-1.localnet",
            },
            new SelenoidConfigModel(){
                Address = "selenoid-service-2.localnet",
                Enabled = false,
            },
        };
    }
}
