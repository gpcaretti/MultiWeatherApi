using System;
using System.Threading.Tasks;
using MultiWeatherApi.Model;

namespace MultiWeatherApi {

    /// <summary>
    ///     General interface for the weather service
    /// </summary>
    public interface IWeatherService {

        /// <summary>
        ///    Returns the current weather conditions, included an hour-by-hour forecast for the next 48 hours and possible alerts
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="unit">Default is <see cref="Model.Unit.Auto"/></param>
        /// <param name="language">Default is <see cref="Language.English"/></param>
        /// <returns>A <see cref="Weather"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="WeatherException">Thrown if the response did not have a status code indicating success.</exception>
        Task<Weather> GetCurrentWeather(double latitude, double longitude, Unit unit = Unit.Auto, Language language = Language.English);

        /// <summary>
        ///    Returns the current weather conditions, included
        ///     * an hour-by-hour forecast for the next 48 hours, 
        ///     * the daily forecast for the next 7 days and 
        ///     * possible alerts
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="unit">Default is <see cref="Model.Unit.Auto"/></param>
        /// <param name="language">Default is <see cref="Language.English"/></param>
        /// <returns>A <see cref="WeatherGroup"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="WeatherException">Thrown if the response did not have a status code indicating success.</exception>
        Task<WeatherGroup> GetForecast(double latitude, double longitude, Unit unit = Unit.Auto, Language language = Language.English);

        /// <summary>
        ///     Retrieve the observed (in the past) or forecasted (in the future) hour-by-hour weather and daily weather 
        ///     conditions for a particular date. Returned data are identical in structure to the current conditions, except:
        ///     * The 'currently' data point will refer to the time provided, rather than the current time.
        ///     * The 'hourly data block will contain data points starting at midnight (local time) of the day requested, and continuing 
        ///       until midnight (local time) of the following day.
        ///     * The 'daily' data block will contain a single data point referring to the requested date.
        ///     * The 'alerts' data block will be omitted.
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="dateTime"></param>
        /// <param name="unit">Default is <see cref="Model.Unit.Auto"/></param>
        /// <param name="language">Default is <see cref="Language.English"/></param>
        /// <returns></returns>
        Task<Weather> GetWeatherByDate(double latitude, double longitude, DateTime dateTime, Unit unit = Unit.Auto, Language language = Language.English);
    }
}