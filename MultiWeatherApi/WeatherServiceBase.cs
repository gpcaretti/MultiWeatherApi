using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MultiWeatherApi.OpenWeather.Helpers;
using Newtonsoft.Json;

namespace MultiWeatherApi {

    /// <summary>
    ///     General commond methods for an Weather API fetcher
    /// </summary>
    public abstract class WeatherServiceBase {

        private protected readonly string _apiKey;

        /// <summary>
        ///     Initializes a new instance of the weather service.
        /// </summary>
        /// <param name="key">The API key to use.</param>
        public WeatherServiceBase(string key) { 
            _apiKey = key;
        }

        /// <summary>
        ///     Creates a HttpClientHandler that supports compression for responses.
        /// </summary>
        /// <returns>The <see cref="HttpClientHandler"/> with compression support.</returns>
        protected virtual HttpClientHandler GetCompressionHandler() {
            var compressionHandler = new HttpClientHandler();
            if (compressionHandler.SupportsAutomaticDecompression) {
                compressionHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            return compressionHandler;
        }

        /// <summary>
        ///     Given a successful response from the forecast service, read the stream and parses it as weather data
        /// </summary>
        /// <param name="jsonStream">A successful stream containing weather data in json format</param>
        /// <param name="customConverters">An optional array of custom converts</param>
        /// <returns>A <see cref="Task"/> for an object containing the weather data from the response.</returns>
        /// <remarks>throw a <see cref="WeatherException" /> with <see cref="WeatherException.JsonParsingError"/></remarks>
        /// <exception cref="WeatherException">thrown new WeatherException(WeatherException.JsonParsingError, jex.Message, jex)</exception>
        protected virtual TResult ParseJsonFromStream<TResult>(Stream jsonStream, params JsonConverter[] customConverters) /*where TResult: object*/ {
            try {
                using (var jsonReader = new JsonTextReader(new StreamReader(jsonStream))) {
                    var serializer = new JsonSerializer();
                    for (int i = 0; i < customConverters.Length; i++) {
                        serializer.Converters.Add(customConverters[0]);
                    }
                    return serializer.Deserialize<TResult>(jsonReader);
                }
            }
            catch (Newtonsoft.Json.JsonException jex) {
                throw new WeatherException(WeatherException.JsonParsingError, jex.Message, jex);
            }

            // ==== this section is based on System.Text.Json instead of Newtonsoft.Json
            //    return await System.Text.Json.JsonSerializer.DeserializeAsync<TResult>(responseStream);
            //    //var serializer = new DataContractJsonSerializer(typeof(TResult));
            //    //return (TResult)serializer.ReadObject(responseStream);
            // === end
        }

        /// <summary>
        ///     Checks if the user provided a non-null API key during initialization, and throws an exception if not.
        /// </summary>
        /// <exception cref="WeatherException">Thrown if the API key is null or the empty string.</exception>
        protected virtual void ThrowExceptionIfApiKeyInvalid() {
            if (string.IsNullOrEmpty(_apiKey)) {
                throw new WeatherException(WeatherException.EmptyApiKey, "No API key was given.");
            }
        }

        /// <summary>
        ///     Throws an <see cref="WeatherException"/> if the given response didn't have a status code indicating success, 
        ///     with the status code included in the exception message.
        /// </summary>
        /// <param name="response">The response</param>
        /// <exception cref="WeatherException">Thrown if the response did not have a status code indicating success.</exception>
        protected virtual void ThrowExceptionIfResponseError(HttpResponseMessage response) {
            if (response.StatusCode == HttpStatusCode.Unauthorized) {
                throw new WeatherException(
                    WeatherException.HttpUnauthorized,
                    !string.IsNullOrWhiteSpace(response.ReasonPhrase) ? response.ReasonPhrase : "Couldn't retrieve data: status " + response.StatusCode);
            } else if (!response.IsSuccessStatusCode) {
                throw new WeatherException(
                    WeatherException.HttpError,
                    !string.IsNullOrWhiteSpace(response.ReasonPhrase) ? response.ReasonPhrase : "Couldn't retrieve data: status " + response.StatusCode);
            }
        }
    }
}