using System;
using System.Net.Http;
using System.Threading.Tasks;
using MultiWeatherApi.OpenWeatherMap.Model;

namespace MultiWeatherApi.OpenWeatherMap {

    /// <summary>
    ///     The OpenWeatherMap service. Returns weather data for given locations, and provides API usage information.
    /// </summary>
    public class OpenWeatherService : WeatherServiceBase {

        private const string OpenWeatherMapEndpoint = "https://api.openweathermap.org/data/2.5/";

        const string WeatherCoordinatesUri = OpenWeatherMapEndpoint + "weather?lat={0}&lon={1}&units={2}&lang={3}&appid={4}";
        const string WeatherCityUri = OpenWeatherMapEndpoint + "weather?q={0}&units={1}&lang={2}&appid={3}";

        const string ForecastCoordinatesUri = OpenWeatherMapEndpoint + "forecast?lat={0}&lon={1}&units={2}&lang={3}&appid={4}";
        const string ForecastCityUri = OpenWeatherMapEndpoint + "forecast?q={0}&units={1}&lang={2}&appid={3}";

        const string ForecastOneCallUri = OpenWeatherMapEndpoint + "onecall?lat={0}&lon={1}&units={2}&lang={3}&appid={4}";

        /// <summary>
        ///     The API key to use in all requests.
        /// </summary>
        private readonly string apiKey;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OpenWeatherService"/> class.
        /// </summary>
        /// <param name="key">The API key to use.</param>
        public OpenWeatherService(string key) 
            : base(key) {
        }

        public async Task<WeatherDto> GetCurrentWeather(
            string city,
            Unit units = Unit.Metric,
            Language language = Language.It) {

            ThrowExceptionIfApiKeyInvalid();

            var compressionHandler = GetCompressionHandler();
            using (var client = new HttpClient(compressionHandler)) {
                try {
                    var url = string.Format(WeatherCityUri, city, units.ToString().ToLower(), language.ToString().ToLower(), _apiKey);
                    HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                    ThrowExceptionIfResponseError(response);
                    return await ParseForecastFromResponse<WeatherDto>(response).ConfigureAwait(false);
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
        ///     Wrap the default parser to catch the possible parser exceptions in a <see cref="WeatherException"/>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="response"></param>
        /// <returns>the parsed object or a <see cref="WeatherException"/></returns>
        protected override async Task<TResult> ParseForecastFromResponse<TResult>(HttpResponseMessage response) {
            try {
                return await base.ParseForecastFromResponse<TResult>(response).ConfigureAwait(false);
            }
            catch (System.Runtime.Serialization.SerializationException jex) {
                throw new WeatherException(WeatherException.JsonParsingError, jex.Message, jex);
            }
            catch (System.Runtime.Serialization.InvalidDataContractException jex) {
                throw new WeatherException(WeatherException.JsonParsingError, jex.Message, jex);
            }
        }

    }
}
