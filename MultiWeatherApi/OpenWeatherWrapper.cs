using System;
using System.Threading.Tasks;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather;
using OWModel = MultiWeatherApi.OpenWeather.Model;
using Nelibur.ObjectMapper;

namespace MultiWeatherApi {

    internal class OpenWeatherWrapper : IWeatherService {

        private IOpenWeatherService _service;

        /// <summary>Set the api key for the underneath weather service</summary>
        public string ApiKey { set => _service = new OpenWeatherService(value); }

        /// <summary>
        ///     Default constructor. Require to soon set the <see cref="ApiKey"/>.
        /// </summary>
        public OpenWeatherWrapper() {
        }

        /// <summary>
        ///     Constructor allowing to set the <see cref="ApiKey"/> for the underneath weather service
        /// </summary>
        /// <param name="apiKey">the API key for the underneath weather service</param>
        public OpenWeatherWrapper(string apiKey) {
            ApiKey = apiKey;
        }

        public async Task<Weather> GetCurrentWeather(double latitude, double longitude, Unit unit = Unit.Auto, Language language = Language.English) {
            var src = await _service.GetForecastDSL(latitude, longitude, (OWModel.OWUnit)unit, language);
            var output = TinyMapper.Map<Weather>(src);

            // === ALTERNATIVE using GetCurrentWeather
            //var innerW = await _service.GetCurrentWeather(latitude, longitude, (OWUnit)unit, language);
            //var output = TinyMapper.Map<Model.Weather>((object)innerW);
            // ===

            // === do some adaptation that TinyMapper does not do quickly

            // Convert time offset from seconds to hours
            if (src.TimeZoneOffset > 0) output.TimeZoneOffset = (float)Math.Round(src.TimeZoneOffset / 3600.0f, 1);

            // TODO Map this: output.Hourly = innerW.Hourly,

            return output;
        }

        public async Task<WeatherGroup> GetForecast(double latitude, double longitude, Unit unit = Unit.Auto, Language language = Language.English) {
            OWModel.ForecastDSL src = await _service.GetForecastDSL(latitude, longitude, (OWModel.OWUnit)unit, language);

            var output = new WeatherGroup(src.Daily?.Count ?? 8);
            output = TinyMapper.Map<OWModel.ForecastDSL, WeatherGroup>(src, output);
            foreach (var dataPoint in src.Daily) {
                output.Add(TinyMapper.Map<Weather>(dataPoint));
            }

            return output;
        }

        public Task GetWeatherByDate(double latitude, double longitude, DateTime dateTime) {
            throw new NotImplementedException();
        }

    }
}