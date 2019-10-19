using ScrapModels.Models;
using System.Collections.Generic;

namespace ScraperServices.Models.Komo
{
    internal class ScraperKomoConfigModel
    {
        public List<SelenoidConfigModel> Selenoid { get; set; } = new List<SelenoidConfigModel>() {
            new SelenoidConfigModel(){
                Address = "selenoid-service-3.localnet",
            },
            //new SelenoidConfigModel(){
            //    Address = "selenoid-service-2.localnet",
            //    Enabled = false,
            //},
        };
    }
}