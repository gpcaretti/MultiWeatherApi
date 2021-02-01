using System;
using System.Linq;
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

        /// <summary>
        ///     The root url of the end point
        /// </summary>
        public const string EndPointRoot = "https://api.openweathermap.org/data/2.5/";

        const string WeatherCoordinatesUri = EndPointRoot + "weather?lat={0}&lon={1}&units={2}&lang={3}&appid={4}";
        const string WeatherCityUri = EndPointRoot + "weather?q={0}&units={1}&lang={2}&appid={3}";

        const string ForecastCoordinatesUri = EndPointRoot + "forecast?lat={0}&lon={1}&units={2}&lang={3}&appid={4}";
        const string ForecastCityUri = EndPointRoot + "forecast?q={0}&units={1}&lang={2}&appid={3}";

        const string ForecastOneCallUri = EndPointRoot + "onecall?lat={0}&lon={1}&units={2}&lang={3}&appid={4}";

        /// <summary>
        ///     Initializes a new instance of the weather service using the default <see cref="HttpMessageHandler"/>
        /// </summary>
        /// <param name="key">The API key to use.</param>
        public OpenWeatherService(string key) 
            : base(key) {
        }

        /// <summary>
        ///     Initializes a new instance of the weather service.
        /// </summary>
        /// <param name="apiKey">The API key to use.</param>
        /// <param name="handler">the http message handler. If null use the one from <see cref="GetMessageHandler"/></param>
        public OpenWeatherService(string apiKey, HttpMessageHandler handler) 
            : base(apiKey, handler) {
        }

        public async Task<WeatherConditions> GetCurrentWeather(
            double latitude, double longitude, 
            OWUnit unit = OWUnit.Standard,
            Language language = Language.English) {

            ThrowExceptionIfApiKeyInvalid();

            try {
                var url = string.Format(WeatherCoordinatesUri, latitude, longitude, unit.ToString().ToLower(), language.ToValue(), _apiKey);
                HttpResponseMessage response = await _httpClient.GetAsync(url).ConfigureAwait(false);
                ThrowExceptionIfResponseError(response);
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
                    return ParseJsonFromStream<WeatherConditions>(responseStream, new MyAlertConverter());
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <param name="unit"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public async Task<WeatherConditions> GetCurrentWeather(
            string city,
            OWUnit unit = OWUnit.Standard,
            Language language = Language.English) {

            ThrowExceptionIfApiKeyInvalid();

            try {
                var url = string.Format(WeatherCityUri, city, unit.ToString().ToLower(), language.ToValue().ToLower(), _apiKey);
                HttpResponseMessage response = await _httpClient.GetAsync(url).ConfigureAwait(false);
                ThrowExceptionIfResponseError(response);
#if DEBUG
    var json = await response.Content.ReadAsStringAsync();
#endif
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
                    return ParseJsonFromStream<WeatherConditions>(responseStream, new MyAlertConverter());
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

        public async Task<MultiWeatherConditions> GetForecast(
            double latitude, double longitude,
            OWUnit unit = OWUnit.Standard,
            Language language = Language.English) {

            ThrowExceptionIfApiKeyInvalid();

            try {
                var url = string.Format(ForecastCoordinatesUri, latitude, longitude, unit.ToString().ToLower(), language.ToValue().ToLower(), _apiKey);
                HttpResponseMessage response = await _httpClient.GetAsync(url).ConfigureAwait(false);
                ThrowExceptionIfResponseError(response);
#if DEBUG
                var json = await response.Content.ReadAsStringAsync();
#endif
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

        public async Task<MultiWeatherConditions> GetForecast(
            string city,
            OWUnit unit = OWUnit.Standard,
            Language language = Language.English) {

            ThrowExceptionIfApiKeyInvalid();

            try {
                var url = string.Format(ForecastCityUri, city, unit.ToString().ToLower(), language.ToValue(), _apiKey);
                HttpResponseMessage response = await _httpClient.GetAsync(url).ConfigureAwait(false);
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

            try {
                var url = string.Format(ForecastOneCallUri, latitude, longitude, unit.ToString().ToLower(), language.ToValue(), _apiKey);
                HttpResponseMessage response = await _httpClient.GetAsync(url).ConfigureAwait(false);
                ThrowExceptionIfResponseError(response);
#if DEBUG
    var json = await response.Content.ReadAsStringAsync();
#endif                    
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
                    var output = ParseJsonFromStream<ForecastDSL>(responseStream, new MyAlertConverter());
                    // patch a bit the output and return it
                    return PatchReturnedData(output);
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

        /// <summary>
        ///     if there is more info on the daily data, copy it on Currently
        /// </summary>
        private ForecastDSL PatchReturnedData(ForecastDSL forecast) {
            // check if there is more info on the daily data. If not return the input
            DataPointDSL sameDay = null;
            if ((forecast.Daily?.Count ?? 0) > 0) {
                sameDay = forecast.Daily.FirstOrDefault(d =>
                                                (d.SunriseTime != null) &&
                                                (d.SunriseTime.Value.Date.Equals(forecast.Currently.Time.Date)));
            }
            if (sameDay == null) return forecast;

            // if there are more info on the daily data, copy it on output.Currently

            var currently = forecast.Currently;
            //if (string.IsNullOrEmpty(currently.Description)) currently.Description = sameDay.Description;
            //if (string.IsNullOrEmpty(currently.Summary)) currently.Summary = sameDay.Summary;
            //if (string.IsNullOrEmpty(currently.Icon)) currently.Icon = sameDay.Icon;

            if (!currently.Temperature.Daily.HasValue) currently.Temperature.Daily = sameDay.Temperature.Daily;
            if (!currently.Temperature.DewPoint.HasValue) currently.Temperature.DewPoint = sameDay.Temperature.DewPoint;
            if (!currently.Temperature.Evening.HasValue) currently.Temperature.Evening = sameDay.Temperature.Evening;
            if (!currently.Temperature.Max.HasValue) currently.Temperature.Max = sameDay.Temperature.Max;
            if (!currently.Temperature.Min.HasValue) currently.Temperature.Min = sameDay.Temperature.Min;
            if (!currently.Temperature.Morning.HasValue) currently.Temperature.Morning = sameDay.Temperature.Morning;
            if (!currently.Temperature.Night.HasValue) currently.Temperature.Night = sameDay.Temperature.Night;
            if (!currently.Temperature.Humidity.HasValue) currently.Temperature.Humidity = sameDay.Temperature.Humidity;
            if (!currently.Temperature.Pressure.HasValue) currently.Temperature.Pressure = sameDay.Temperature.Pressure;

            if (!currently.ApparentTemperature.Daily.HasValue) currently.ApparentTemperature.Daily = sameDay.ApparentTemperature.Daily;
            if (!currently.ApparentTemperature.DewPoint.HasValue) currently.ApparentTemperature.DewPoint = sameDay.ApparentTemperature.DewPoint;
            if (!currently.ApparentTemperature.Evening.HasValue) currently.ApparentTemperature.Evening = sameDay.ApparentTemperature.Evening;
            if (!currently.ApparentTemperature.Max.HasValue) currently.ApparentTemperature.Max = sameDay.ApparentTemperature.Max;
            if (!currently.ApparentTemperature.Min.HasValue) currently.ApparentTemperature.Min = sameDay.ApparentTemperature.Min;
            if (!currently.ApparentTemperature.Morning.HasValue) currently.ApparentTemperature.Morning = sameDay.ApparentTemperature.Morning;
            if (!currently.ApparentTemperature.Night.HasValue) currently.ApparentTemperature.Night = sameDay.ApparentTemperature.Night;
            if (!currently.ApparentTemperature.Humidity.HasValue) currently.ApparentTemperature.Humidity = sameDay.ApparentTemperature.Humidity;
            if (!currently.ApparentTemperature.Pressure.HasValue) currently.ApparentTemperature.Pressure = sameDay.ApparentTemperature.Pressure;

            return forecast;
        }
    }
}
