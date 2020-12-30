using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace MultiWeatherApi {

    /// <summary>
    ///     General commond methods for an Weather API fetcher
    /// </summary>
    public class WeatherServiceBase {

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
        ///     Given a successful response from the forecast service, parses the weather data contained within and returns it.
        /// </summary>
        /// <param name="response">A successful response containing weather data.</param>
        /// <returns>A <see cref="Task"/> for an object containing the weather data from the response.</returns>
        /// <remarks>This implementation is based on standard .NET json deserializer. Can throw HTTP and serializer exceptions.</remarks>
        protected virtual async Task<TResult> ParseForecastFromResponse<TResult>(HttpResponseMessage response) /*where TResult: object*/ {
            using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
                var serializer = new DataContractJsonSerializer(typeof(TResult));
                return (TResult)serializer.ReadObject(responseStream);
            }
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