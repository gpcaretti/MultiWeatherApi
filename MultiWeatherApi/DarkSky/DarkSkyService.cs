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
        ///     The root url of the end point
        /// </summary>
        public const string EndPointRoot = "https://api.darksky.net/forecast/";

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
        private const string CurrentConditionsUrl = EndPointRoot + "{0}/{1},{2}?units={3}&extend={4}&exclude={5}&lang={6}";

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
        private const string SpecificTimeConditionsUrl = EndPointRoot + "{0}/{1},{2},{3}?units={4}&extend={5}&exclude={6}&lang={7}";

        /// <summary>
        ///     Initializes a new instance of the weather service using the default <see cref="HttpMessageHandler"/>
        /// </summary>
        /// <param name="key">The API key to use.</param>
        public DarkSkyService(string key)
            : base(key) {
        }

        /// <summary>
        ///     Initializes a new instance of the weather service
        /// </summary>
        /// <param name="apiKey">The API key to use.</param>
        /// <param name="handler">the http message handler. If null use the one from <see cref="GetMessageHandler"/></param>
        public DarkSkyService(string apiKey, HttpMessageHandler handler)
            : base(apiKey, handler) {
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
                new Exclude[] { /*Exclude.Daily, */Exclude.Minutely },  // I don't exclude the daily as Daily[0] has additional info on the current day
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
        ///     Given a formatted URL containing the parameters for a forecast request, retrieves, parses, and returns weather data from it.
        /// </summary>
        /// <param name="requestUrl">The full URL from which the request should be made, including the API key and other parameters.</param>
        /// <returns>A <see cref="Task"/> for a <see cref="Forecast"/> containing weather data.</returns>
        /// <exception cref="WeatherException">Thrown if the response did not have a status code indicating success.</exception>
        private async Task<Forecast> GetForecastFromUrlAsync(string requestUrl) {
            try {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl).ConfigureAwait(false);
                ThrowExceptionIfResponseError(response);
#if DEBUG
    var json = await response.Content.ReadAsStringAsync();
#endif
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false)) {
                    var output = ParseJsonFromStream<Forecast>(responseStream);
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
        private Forecast PatchReturnedData(Forecast forecast) {
            // check if there is more info on the daily data. If not return the input
            DataPoint sameDay = null;
            if ((forecast.Daily?.Data?.Count ?? 0) > 0) {
                sameDay = forecast.Daily.Data.FirstOrDefault(d =>
                                                (d.SunriseTime != null) &&
                                                (d.SunriseTime.Value.Date.Equals(forecast.Currently.Time.Date)));
            }
            if (sameDay == null) return forecast;

            // if here, there is more info on the daily data, copy it on output.Currently
            var currently = forecast.Currently;
            if (!currently.SunriseUnixTime.HasValue) currently.SunriseUnixTime = sameDay.SunriseUnixTime;
            if (!currently.SunsetUnixTime.HasValue) currently.SunsetUnixTime = sameDay.SunsetUnixTime;
            if (!currently.UVIndexTime.HasValue) currently.UVIndexTime = sameDay.UVIndexTime;
            if (!currently.ApparentTemperatureLowTime.HasValue) currently.ApparentTemperatureLowTime = sameDay.ApparentTemperatureLowTime;
            if (!currently.ApparentTemperatureHighTime.HasValue) currently.ApparentTemperatureHighTime = sameDay.ApparentTemperatureHighTime;

            if (!currently.PrecipIntensityMax.HasValue) currently.PrecipIntensityMax = sameDay.PrecipIntensityMax;
            if (!currently.PrecipIntensityMaxTime.HasValue) currently.PrecipIntensityMaxTime = sameDay.PrecipIntensityMaxTime;
            if (!currently.PrecipitationIntensity.HasValue) currently.PrecipitationIntensity = sameDay.PrecipitationIntensity;
            if (!currently.PrecipitationProbability.HasValue) currently.PrecipitationProbability = sameDay.PrecipitationProbability;
            if (!string.IsNullOrEmpty(currently.PrecipitationType)) currently.PrecipitationType = sameDay.PrecipitationType;

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

            // return the patched data
            return forecast;
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