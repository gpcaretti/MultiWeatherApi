using System;
using System.Threading.Tasks;
using MultiWeatherApi.DarkSky;

namespace MultiWeatherApi {

    internal class DarkSkyWrapper : IWeatherService {
        
        private IDarkSkyService _service;

        /// <summary>Set the api key for the underneath weather service</summary>
        public string ApiKey { set => _service = new DarkSkyService(value); }

        /// <summary>
        ///     Default constructor. Require to soon set the <see cref="ApiKey"/>.
        /// </summary>
        public DarkSkyWrapper() {
        }

        /// <summary>
        ///     Constructor allowing to set the <see cref="ApiKey"/> for the underneath weather service
        /// </summary>
        /// <param name="apiKey">the API key for the underneath weather service</param>
        public DarkSkyWrapper(string apiKey) {
            ApiKey = apiKey;
        }

        public Task GetCurrentWeather(double alcatrazLatitude, double alcatrazLongitude) {
            throw new NotImplementedException();
        }

        public Task GetForecast(double alcatrazLatitude, double alcatrazLongitude) {
            throw new NotImplementedException();
        }

        public Task GetWeatherByDate(double alcatrazLatitude, double alcatrazLongitude, DateTime dateTime) {
            throw new NotImplementedException();
        }
    }
}