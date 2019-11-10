using ScraperModels.Models;
using System.Collections.Generic;

namespace ScraperServices.Models.Airdna
{
    internal class ScraperAirdnaConfigModel
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