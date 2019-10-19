using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScraperCore.Repositories;

namespace WebScraperManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : Controller
    {
        private CityRepository _cityRepository { get; set; } = new CityRepository();

        [HttpDelete("{id?}")]
        public JsonResult Remove(int id=0)
        {
            var resultRequest = _cityRepository.Delete(id); 

            return Json(resultRequest);
        }
    }
}