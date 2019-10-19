using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ScraperModels.Models;
using ScraperServices.Services;
using ScraperCore.Repositories;
using WebScraperManager.Models;
using WebScraperManager.DtoModels;

namespace WebScraperManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirdnaController : Controller
    {
        private CityRepository _cityRepository { get; set; } = new CityRepository();
        private IConfiguration _configuration { get; set; }
        private readonly ILogger<AirdnaController> _logger;

        public AirdnaController(ILogger<AirdnaController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public JsonResult ParseUrlForGetCity([FromBody]RequestParseUrlForGetCityIdDto request)
        {
            _logger.LogInformation("Start ParseUrlForGetCity");

            var selenoidProtocol = _configuration.GetSection("WebScraperManager:Selenoid:Protocol").Value;
            var selenoidAddress = _configuration.GetSection("WebScraperManager:Selenoid:Address").Value;

            _logger.LogInformation($"selenoidAddress=={selenoidAddress}");

            var selenoidService = new SelenoidService()
                .UseSelenoidProtocol(selenoidProtocol)
                .UseSelenoidAddress(selenoidAddress);

            var resultRequest = selenoidService.Airdna_ParseUrlForGetCity(request.Url);

            if (resultRequest.State.IsOk)
            {
                var cityExist = _cityRepository.Get((AirdnaModel)resultRequest.City);

                if (cityExist == null)
                {
                    var city = (AirdnaModel)resultRequest.City;

                    _cityRepository.Add(city);
                    resultRequest.State.Message = $"New city {city.CityOriginalName} added";
                }
                else
                {
                    resultRequest.State.ErrorCode = EnumErrorCode.Airdna_CityExistInDictionary;
                }
            }

            return Json(resultRequest);
        }
    }
}