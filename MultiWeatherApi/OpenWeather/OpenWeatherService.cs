using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather.Helpers;
using MultiWeatherApi.OpenWeather.Model;

namespace MultiWeatherApi.OpenWeather {

    /// <summary>
    ///     The OpenWeatherMap service. Returns weather data for given locations, and provides API usage information.
    /// </summary>
    public class OpenWeatherService : WeatherServiceBase, IOpenWeatherService {

        private const string OpenWeatherMapEndpoint = "https://api.openweathermap.org/data/2.5/";

        const string WeatherCoordinatesUri = OpenWeatherMapEndpoint + "weather?lat={0}&lon={1}&units={2}&lang={3}&appid={4}";
        const string WeatherCityUri = OpenWeatherMapEndpoint + "weather?q={0}&units={1}&lang={2}&appid={3}";

        const string ForecastCoordinatesUri = OpenWeatherMapEndpoint + "forecast?lat={0}&lon={1}&units={2}&lang={3}&appid={4}";
        const string ForecastCityUri = OpenWeatherMapEndpoint + "forecast?q={0}&units={1}&lang={2}&appid={3}";

        const string ForecastOneCallUri = OpenWeatherMapEndpoint + "onecall?lat={0}&lon={1}&units={2}&lang={3}&appid={4}";

        /// <summary>
        ///     Initializes a new instance of the <see cref="OpenWeatherService"/> class.
        /// </summary>
        /// <param name="key">The API key to use.</param>
        public OpenWeatherService(string key) 
            : base(key) {
        }

        public async Task<WeatherConditions> GetCurrentWeather(
            double latitude, double longitude, 
            OWUnit unit = OWUnit.Standard,
            Language language = Language.English) {

            ThrowExceptionIfApiKeyInvalid();

            var compressionHandler = GetCompressionHandler();
            using (var client = new HttpClient(compressionHandler)) {
                try {
                    var url = string.Format(WeatherCoordinatesUri, latitude, longitude, unit.ToString().ToLower(), language.ToValue(), _apiKey);
                    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                    ThrowExceptionIfResponseError(response);
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
                        return ParseJsonFromStream<WeatherConditions>(responseStream);
                    }
                }
                catch (WeatherException) {
                    throw;
                }
                catch (HttpRequestException ex) {
                    throw new WeatherException(
                        WeatherException.HttpError,
                        !string.IsNullOrWhiteSpace(ex.Message) ? ex.Message : "Couldn't retrieve data");
                }
                catch (Exception ex) {
                    throw new WeatherException(ex.Message, ex);
                }
            }
        }

        public async Task<WeatherConditions> GetCurrentWeather(
            string city,
            OWUnit unit = OWUnit.Standard,
            Language language = Language.English) {

            ThrowExceptionIfApiKeyInvalid();

            var compressionHandler = GetCompressionHandler();
            using (var client = new HttpClient(compressionHandler)) {
                try {
                    var url = string.Format(WeatherCityUri, city, unit.ToString().ToLower(), language.ToValue().ToLower(), _apiKey);
                    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                    ThrowExceptionIfResponseError(response);
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
                        return ParseJsonFromStream<WeatherConditions>(responseStream);
                    }
                }
                catch (WeatherException) {
                    throw;
                }
                catch (HttpRequestException ex) {
                    throw new WeatherException(
                        WeatherException.HttpError,
                        !string.IsNullOrWhiteSpace(ex.Message) ? ex.Message : "Couldn't retrieve data");
                }
                catch (Exception ex) {
                    throw new WeatherException(ex.Message, ex);
                }
            }
        }

        public async Task<MultiWeatherConditions> GetForecast(
            double latitude, double longitude,
            OWUnit unit = OWUnit.Standard,
            Language language = Language.English) {

            ThrowExceptionIfApiKeyInvalid();

            var compressionHandler = GetCompressionHandler();
            using (var client = new HttpClient(compressionHandler)) {
                try {
                    var url = string.Format(ForecastCoordinatesUri, latitude, longitude, unit.ToString().ToLower(), language.ToValue().ToLower(), _apiKey);
                    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                    ThrowExceptionIfResponseError(response);
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
                        return ParseJsonFromStream<MultiWeatherConditions>(responseStream, new MyAlertConverter());
                    }
                }
                catch (WeatherException) {
                    throw;
                }
                catch (HttpRequestException ex) {
                    throw new WeatherException(
                        WeatherException.HttpError,
                        !string.IsNullOrWhiteSpace(ex.Message) ? ex.Message : "Couldn't retrieve data");
                }
                catch (Exception ex) {
                    throw new WeatherException(ex.Message, ex);
                }
            }
        }

        public async Task<MultiWeatherConditions> GetForecast(
            string city,
            OWUnit unit = OWUnit.Standard,
            Language language = Language.English) {

            ThrowExceptionIfApiKeyInvalid();

            var compressionHandler = GetCompressionHandler();
            using (var client = new HttpClient(compressionHandler)) {
                try {
                    var url = string.Format(ForecastCityUri, city, unit.ToString().ToLower(), language.ToValue(), _apiKey);
                    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                    ThrowExceptionIfResponseError(response);
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
                        return ParseJsonFromStream<MultiWeatherConditions>(responseStream);
                    }
                }
                catch (WeatherException) {
                    throw;
                }
                catch (HttpRequestException ex) {
                    throw new WeatherException(
                        WeatherException.HttpError,
                        !string.IsNullOrWhiteSpace(ex.Message) ? ex.Message : "Couldn't retrieve data");
                }
                catch (Exception ex) {
                    throw new WeatherException(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="unit"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public async Task<ForecastDSL> GetForecastDSL(
            double latitude,
            double longitude,
            OWUnit unit = OWUnit.Standard,
            Language language = Language.English) {

            ThrowExceptionIfApiKeyInvalid();

            var compressionHandler = GetCompressionHandler();
            using (var client = new HttpClient(compressionHandler)) {
                try {
                    var url = string.Format(ForecastOneCallUri, latitude, longitude, unit.ToString().ToLower(), language.ToValue(), _apiKey);
                    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                    ThrowExceptionIfResponseError(response);
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
                        return ParseJsonFromStream<ForecastDSL>(responseStream);
                    }
                }
                catch (WeatherException) {
                    throw;
                }
                catch (HttpRequestException ex) {
                    throw new WeatherException(
                        WeatherException.HttpError,
                        !string.IsNullOrWhiteSpace(ex.Message) ? ex.Message : "Couldn't retrieve data");
                }
                catch (Exception ex) {
                    throw new WeatherException(ex.Message, ex);
                }
            }
        }

    }
}
