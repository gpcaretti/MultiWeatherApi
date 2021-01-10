using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MultiWeatherApi.DarkSky.Model;
using MultiWeatherApi.Model;

namespace MultiWeatherApi.DarkSky {

    /// <summary>
    ///     The Dark Sky service. Returns weather data for given locations, and provides API usage information.
    /// </summary>
    public class DarkSkyService : WeatherServiceBase, IDarkSkyService {

        /// <summary>
        ///     The API endpoint to retrieve current weather conditions.
        /// {0} - API key.
        /// {1} - Latitude.
        /// {2} - Longitude.
        /// {3} - Units.
        /// {4} - Extends hourly data to include the next 7 days (if "hourly" is given).
        /// {5} - Any blocks to be excluded from the results.
        /// {6} - The language to be used in text summaries.
        /// </summary>
        private const string CurrentConditionsUrl = "https://api.darksky.net/forecast/{0}/{1},{2}?units={3}&extend={4}&exclude={5}&lang={6}";

        /// <summary>
        ///     The API endpoint to retrieve weather conditions at a particular date and time.
        /// {0} - API key.
        /// {1} - Latitude.
        /// {2} - Longitude.
        /// {3} - Time to retrieve, either Unix time or [YYYY]-[MM]-[DD]T[HH]:[MM]:[SS].
        /// {4} - Units.
        /// {5} - Extends hourly data to include the next 7 days (if "hourly" is given).
        /// {6} - Any blocks to be excluded from the results.
        /// {7} - The language to be used in text summaries. 
        /// </summary>
        private const string SpecificTimeConditionsUrl = "https://api.darksky.net/forecast/{0}/{1},{2},{3}?units={4}&extend={5}&exclude={6}&lang={7}";

        /// <summary>
        ///     Initializes a new instance of the <see cref="DarkSkyService"/> class.
        /// </summary>
        /// <param name="key">The API key to use.</param>
        public DarkSkyService(string key)
            : base(key) {
        }

        /// <summary>
        ///     Gets the number of API calls made today using the given API key.
        ///     <para>This property will be null until a request has been made.</para>
        /// </summary>
        public int? ApiCallsMade { get; private set; }

        /// <summary>
        ///    Returns the current weather conditions, included an hour-by-hour forecast for the next 48 hours and possible alerts
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="unit">Default is <see cref="DSUnit.Auto"/></param>
        /// <param name="language">Default is <see cref="Language.English"/></param>
        /// <returns>A <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the service returned anything other than a 200 (Status OK) code.</exception>
        public Task<Forecast> GetCurrentWeather(
            double latitude,
            double longitude,
            DSUnit unit = DSUnit.Auto,
            Language language = Language.English) {
            return GetWeather(latitude, longitude,
                new Extend[0],
                new Exclude[] { Exclude.Daily, Exclude.Minutely },
                unit, language);
        }

        /// <summary>
        ///    Returns the current weather conditions, included an hour-by-hour forecast for the next 48 hours, the daily forecast for the next 7 days and possible alerts
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="unit">Default is <see cref="DSUnit.Auto"/></param>
        /// <param name="language">Default is <see cref="Language.English"/></param>
        /// <returns>A <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the service returned anything other than a 200 (Status OK) code.</exception>
        public Task<Forecast> GetForecast(
            double latitude,
            double longitude,
            DSUnit unit = DSUnit.Auto,
            Language language = Language.English) {
            return GetWeather(latitude, longitude,
                new Extend[0],
                new Exclude[] { Exclude.Minutely },
                unit, language);
        }

        /// <summary>
        ///    The most general method to retrieve weather data and forecast for a particular latitude and longitude (full optional).
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="excludes">Any blocks that should be excluded from the request.</param>
        /// <param name="extends">The type of forecast to retrieve extended results for. Currently limited to hourly blocks.</param>
        /// <param name="unit">Default is <see cref="DSUnit.Auto"/></param>
        /// <param name="language">Default is <see cref="Language.English"/></param>
        /// <returns>A <see cref="Task"/> for a <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the service returned anything other than a 200 (Status OK) code.</exception>
        public Task<Forecast> GetWeather(
            double latitude,
            double longitude,
            IList<Extend> extends,
            IList<Exclude> excludes,
            DSUnit unit = DSUnit.Auto,
            Language language = Language.English) {

            ThrowExceptionIfApiKeyInvalid();

            var unitValue = unit.ToValue();
            var extendList = (extends != null) ? string.Join(",", extends.Select(x => x.ToValue())) : string.Empty;
            var excludeList = (excludes != null) ? string.Join(",", excludes.Select(x => x.ToValue())) : string.Empty;
            var languageValue = language.ToValue();

            var requestUrl = string.Format(
                CultureInfo.InvariantCulture,
                CurrentConditionsUrl,
                _apiKey,
                latitude,
                longitude,
                unitValue,
                extendList,
                excludeList,
                languageValue);

            return GetForecastFromUrlAsync(requestUrl);
        }

        /// <summary>
        ///     Retrieve the observed (in the past) or forecasted (in the future) hour-by-hour weather and daily weather 
        ///     conditions for a particular date. A Time Machine request is identical in structure to a Forecast Request, except:
        ///     * The 'currently' data point will refer to the time provided, rather than the current time.
        ///     * The 'minutely' data block will be omitted, unless you are requesting a time within an hour of the present.
        ///     * The 'hourly data block will contain data points starting at midnight (local time) of the day requested, and continuing 
        ///       until midnight (local time) of the following day.
        ///     * The 'daily' data block will contain a single data point referring to the requested date.
        ///     * The 'alerts' data block will be omitted.
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="date">Requested date</param>
        /// <param name="unit">Default <see cref="DSUnit.Auto"/></param>
        /// <param name="language">Default <see cref="Language.English"/></param>
        /// <returns>A <see cref="Task"/> for a <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the service returned anything other than a 200 (Status OK) code.</exception>
        public Task<Forecast> GetWeatherByDate(
            double latitude,
            double longitude,
            DateTimeOffset date,
            DSUnit unit = DSUnit.Auto,
            Language language = Language.English) {
            return
                GetWeatherByDate(
                    latitude,
                    longitude,
                    date,
                    null,
                    null,
                    unit,
                    language);
        }

        /// <summary>
        ///     Retrieve the observed (in the past) or forecasted (in the future) hour-by-hour weather and daily weather 
        ///     conditions for a particular date. A Time Machine request is identical in structure to a Forecast Request, except:
        ///     * The 'currently' data point will refer to the time provided, rather than the current time.
        ///     * The 'minutely' data block will be omitted, unless you are requesting a time within an hour of the present.
        ///     * The 'hourly data block will contain data points starting at midnight (local time) of the day requested, and continuing 
        ///       until midnight (local time) of the following day.
        ///     * The 'daily' data block will contain a single data point referring to the requested date.
        ///     * The 'alerts' data block will be omitted.
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="date">Requested date</param>
        /// <param name="excludes">Any blocks that should be excluded from the request.</param>
        /// <param name="unit">Default <see cref="DSUnit.Auto"/></param>
        /// <param name="language">Default <see cref="Language.English"/></param>
        /// <returns>A <see cref="Task"/> for a <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        public Task<Forecast> GetWeatherByDate(
            double latitude,
            double longitude,
            DateTime date,
            IList<Exclude> excludes,
            DSUnit unit = DSUnit.Auto,
            Language language = Language.English) {
            return
                GetWeatherByDate(
                    latitude,
                    longitude,
                    date,
                    null,
                    excludes,
                    unit,
                    language);
        }

        /// <summary>
        ///     The most general method to retrieve the observed (in the past) or forecasted (in the future) hour-by-hour weather and daily weather 
        ///     conditions for a particular date. A Time Machine request is identical in structure to a Forecast Request, except:
        ///     * The 'currently' data point will refer to the time provided, rather than the current time.
        ///     * The 'minutely' data block will be omitted, unless you are requesting a time within an hour of the present.
        ///     * The 'hourly data block will contain data points starting at midnight (local time) of the day requested, and continuing 
        ///       until midnight (local time) of the following day.
        ///     * The 'daily' data block will contain a single data point referring to the requested date.
        ///     * The 'alerts' data block will be omitted.
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="date">Requested date</param>
        /// <param name="excludes">Any blocks that should be excluded from the request.</param>
        /// <param name="extends">The type of forecast to retrieve extended results for. Currently limited to hourly blocks.</param>
        /// <param name="unit">Default <see cref="DSUnit.Auto"/></param>
        /// <param name="language">Default <see cref="Language.English"/></param>
        /// <returns>A <see cref="Task"/> for a <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the service returned anything other than a 200 (Status OK) code.</exception>
        public Task<Forecast> GetWeatherByDate(
            double latitude,
            double longitude,
            DateTimeOffset date,
            IList<Extend> extends,
            IList<Exclude> excludes,
            DSUnit unit = DSUnit.Auto,
            Language language = Language.English) {

            ThrowExceptionIfApiKeyInvalid();

            var unitValue = unit.ToValue();
            var extendList = (extends != null) ? string.Join(",", extends.Select(x => x.ToValue())) : string.Empty;
            var excludeList = (excludes != null) ? string.Join(",", excludes.Select(x => x.ToValue())) : string.Empty;
            var languageValue = language.ToValue();
            var unixTime = date.ToUnixTime();

            var requestUrl = string.Format(
                CultureInfo.InvariantCulture,
                SpecificTimeConditionsUrl,
                _apiKey,
                latitude,
                longitude,
                unixTime,
                unitValue,
                extendList,
                excludeList,
                languageValue);

            return GetForecastFromUrlAsync(requestUrl);
        }

        #region Privates

        /// <summary>
        /// Given a formatted URL containing the parameters for a forecast
        /// request, retrieves, parses, and returns weather data from it.
        /// </summary>
        /// <param name="requestUrl">
        /// The full URL from which the request should be made, including
        /// the API key and other parameters.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> for a <see cref="Forecast"/> containing
        /// weather data.
        /// </returns>
        private async Task<Forecast> GetForecastFromUrlAsync(string requestUrl) {
            var compressionHandler = GetCompressionHandler();
            using (var client = new HttpClient(compressionHandler)) {
                var response = await client.GetAsync(requestUrl).ConfigureAwait(false);
                ThrowExceptionIfResponseError(response);
                UpdateApiCallsMade(response);
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
                    return ParseJsonFromStream<Forecast>(responseStream);
                }
            }
        }

        /// <summary>
        ///     Updates the number of API calls made using the value provided in the response to a weather data request.
        /// </summary>
        /// <param name="response">Response received after successfully requesting weather data.</param>
        private void UpdateApiCallsMade(HttpResponseMessage response) {
            IEnumerable<string> apiCallHeaderValues;
            if (response.Headers.TryGetValues("X-Forecast-API-Calls", out apiCallHeaderValues)) {
                ApiCallsMade = int.Parse(apiCallHeaderValues.First());
            }
        }

        #endregion
    }
}