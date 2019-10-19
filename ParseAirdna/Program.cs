using Flurl.Http;
using Newtonsoft.Json;
using ParseJson.Models;
using ScraperModels.Models;
using ScraperServices.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ParseJson
{
    class Program
    {
        static int CityId { get; set; } = 1;
        //static private ConfigRepository _configRepository { get; set; } = new ConfigRepository();
        //static private CityRepository _cityRepository {get;set;} = new CityRepository();
        static void Main(string[] args)
        {
            InitApplication();
            
            if (args.Length == 0)
            {
                EchoHelp();
                return;
            }

            Console.WriteLine("Start scrapping...");

            var canCloseApp = ParseParams(args);

            if (canCloseApp)
            {
                Environment.Exit(0);
            }

            var loginUrl = "https://www.airdna.co/api/v1/account/login";
            var loginPost = loginUrl
                .PostUrlEncodedAsync(new { })
                //.PostUrlEncodedAsync(new { username = Config.Login, password = Config.Password, remember_me = "true" })
                .ReceiveJson<ResponseLogin>();

            var loginResult = loginPost.Result;

            if (loginResult.Status == null || loginResult.Status.ToLower() != "success")
            {
                Console.WriteLine($"Error authorizations on airdna.co site.");
                return;
            }

            var token = loginResult.Token;

            var url = $"https://api.airdna.co/v1/market/property_list?access_token={token}&city_id={CityId}";

            var dto = url.GetJsonAsync<AirdnaScrapeDataModel>().Result;

            var excel = new ExcelService();

            //var resultExport = excel.ExportAirDnaProperies(dto);

            Console.WriteLine("\ndone");
        }
        static bool ParseParams(string[] args)
        {
            var result = false;

            for(var i = 0; i < args.Length; i++)
            {
                var param = GetParambyIndex(args,i);

                //switch (param.ToLower())
                //{
                //    case "-password":
                //        Config.Password = GetParambyIndex(args, ++i);
                //        break;
                //    case "-username":
                //        Config.Login = GetParambyIndex(args, ++i);
                //        break;
                //    case "-city":
                //        var cityParam = GetParambyIndex(args, ++i).ToLower();
                //        var city = Config.Cities.Where(x => x.Key == cityParam).Select(x => x.Value).FirstOrDefault();
                //        if (city != null) CityId = city.CityId;
                //        break;
                //    case "-url-parse":
                //        var url = GetParambyIndex(args, ++i).ToLower();
                //        result = true;
                //        var selenoidService = new SelenoidService();
                //        selenoidService.Airdna_ParseUrlForGetCityId(url);
                //        break;
                //}
            }

            return result;
        }
        static string GetParambyIndex(string[] args, int index)
        {
            var result = "";

            if (index < args.Length) result = args[index];

            return result;
        }
        static void EchoHelp()
        {
            Console.WriteLine();
            Console.WriteLine($"ParseAirdna.exe -city CityName -password Password -username Username -url-parse URL");
            Console.WriteLine($"\tUsername is username for airdna.co site");
            Console.WriteLine($"\tPassword is passord for airdna.co site");
            Console.WriteLine($"\t-url-parse need for parse URL then getting CityId into configuration.json file");

            //if (Config.Cities.Count() > 0)
            //{
            //    Console.Write($"\tCityName is:");
            //    var firstCityKey = Config.Cities.First().Key;
            //    foreach (var city in Config.Cities)
            //    {
            //        if (city.Key == firstCityKey)
            //            Console.Write(" ");
            //        else
            //            Console.Write(", ");
            //        Console.Write($"{city.Key}");
            //    }
            //}

            Console.WriteLine();
        }
        static void InitApplication()
        {
            //var configFile = @"configuration.json";

            //if (File.Exists(configFile))
            //{
            //    Config = JsonConvert.DeserializeObject<ConfigurationModel>(File.ReadAllText(configFile));
            //}

            //if (Config is null)
            //{
            //    Config = new ConfigurationModel();
            //    File.WriteAllText(@"configuration.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
            //}

            return;
        }
    }
}
