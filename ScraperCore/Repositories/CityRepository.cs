using Newtonsoft.Json;
using ScraperModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScraperCore.Repositories
{
    public class CityRepository
    {
        private List<CityDomainModel> _cities { get; set; }
        private readonly string storeFilename = @"cities.json";

        public CityRepository()
        {
            _initRepository();
        }
        public IEnumerable<CityDomainModel> Get()
        {
            return _cities;
        }
        public CityDomainModel Get(AirdnaModel cityRequest)
        {
            var city = Get().Where(x => x.Airdna.CityId == cityRequest.CityId).Select(x=>x).FirstOrDefault();

            return city;
        }
        public CityDomainModel Add(AirdnaModel cityRequest)
        {
            var city = Get(cityRequest);

            if (city is null)
            {
                var newKey = 1;
                foreach (var cityTemp in _cities) if (cityTemp.Id > newKey) newKey = cityTemp.Id;

                newKey++;

                city = new CityDomainModel()
                {
                    Id = newKey,
                    CityOriginalName = cityRequest.CityOriginalName,
                    Airdna = (AirdnaModel)cityRequest.Clone(),
                };

                _cities.Add(city);

                _saveChanges();
            }

            return city;
        }
        public ResponseStateModel Delete(int id = 0)
        {
            var result = new ResponseStateModel();

            if (id > 0)
            {
                var city = Get().Where(x => x.Id == id).FirstOrDefault();
                if (city != null)
                {
                    _cities.Remove(city);
                    _saveChanges();
                }
                else result.ErrorCode = EnumErrorCode.ErrorByDeleteCity;
            }
            else
            {
                result.ErrorCode = EnumErrorCode.InvalidCityId;
            }

            return result;
        }
        private void _initRepository()
        {
            if (File.Exists(storeFilename))
            {
                _cities = JsonConvert.DeserializeObject<List<CityDomainModel>>(File.ReadAllText(storeFilename));

            } else
            {
                _cities = _initDefaultData();
                _saveChanges();
            }
        }
        private List<CityDomainModel> _initDefaultData()
        {
            _cities = new List<CityDomainModel>() {
                    new CityDomainModel() { Id=1, CityOriginalName="Ashdod", Airdna = new AirdnaModel() { CityId="84771" , CityName="ashdod", } } ,
                    new CityDomainModel() { Id=2, CityOriginalName="Bat Yam", Airdna = new AirdnaModel() { CityId="84809" , CityName="bat-yam", } },
                    };

            return _cities;
        }
        private bool _saveChanges()
        {
            var result = true;

            File.WriteAllText(storeFilename, JsonConvert.SerializeObject(_cities, Formatting.Indented));

            return result;
        }
    }
}
