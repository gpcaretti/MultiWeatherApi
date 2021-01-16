using System;
using System.Threading.Tasks;
using MultiWeatherApi.DarkSky;
using MultiWeatherApi.Model;
using Nelibur.ObjectMapper;
using DSModel = MultiWeatherApi.DarkSky.Model;

namespace MultiWeatherApi {

    internal class DarkSkyWrapper : IWeatherService {

        public static readonly Guid _uniqueGuid = new Guid("9176F103-2BCA-4C20-A8BE-9E8C63690F89");
        private IDarkSkyService _service;

        /// <summary>Unique guid Id of this service</summary>
        public Guid Id { get => _uniqueGuid; }

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

        public async Task<Weather> GetCurrentWeather(double latitude, double longitude, Unit unit = Unit.Auto, Language language = Language.English) {
            DSModel.Forecast src = await _service.GetCurrentWeather(latitude, longitude, (DSModel.DSUnit)unit, language);
            var output = TinyMapper.Map<Weather>(src);
            // TODO Map this: output.Hourly = innerW.Hourly,
            return output;
        }

        public async Task<WeatherGroup> GetForecast(double latitude, double longitude, Unit unit = Unit.Auto, Language language = Language.English) {
            DSModel.Forecast src = await _service.GetForecast(latitude, longitude, (DSModel.DSUnit)unit, language);

            var output = new WeatherGroup(src.Daily?.Data?.Count ?? 8);
            output = TinyMapper.Map<DSModel.Forecast, WeatherGroup>(src, output);
            foreach (var dataPoint in src.Daily.Data) {
                output.Add(TinyMapper.Map<Weather>(dataPoint));
            }

            return output;
        }

        public Task GetWeatherByDate(double latitude, double longitude, DateTime dateTime) {
            throw new NotImplementedException();
        }

    }
}