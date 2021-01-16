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

        Task GetWeatherByDate(double latitude, double longitude, DateTime dateTime);
    }
}