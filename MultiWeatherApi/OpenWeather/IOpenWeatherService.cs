using System.Threading.Tasks;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather.Model;

namespace MultiWeatherApi.OpenWeather {
    public interface IOpenWeatherService {

        /// <summary>
        ///     current weather data for one location by geographic coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="unit">Default <see cref="OWUnit.Standard"/></param>
        /// <param name="language">Default <see cref="Language.English"/></param>
        /// <returns></returns>
        Task<WeatherConditions> GetCurrentWeather(double latitude, double longitude, OWUnit unit = OWUnit.Standard, Language language = Language.English);

        /// <summary>
        ///     current weather data for one location by city name
        /// </summary>
        /// <param name="city"></param>
        /// <param name="unit">Default <see cref="OWUnit.Standard"/></param>
        /// <param name="language">Default <see cref="Language.English"/></param>
        /// <returns></returns>
        Task<WeatherConditions> GetCurrentWeather(string city, OWUnit unit = OWUnit.Standard, Language language = Language.English);

        /// <summary>
        ///     weather forecast for 5 days with data every 3 hours
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="unit">Default <see cref="OWUnit.Standard"/></param>
        /// <param name="language">Default <see cref="Language.English"/></param>
        Task<MultiWeatherConditions> GetForecast(double latitude, double longitude, OWUnit unit = OWUnit.Standard, Language language = Language.English);

        /// <summary>
        ///     weather forecast for 5 days with data every 3 hours by city name
        /// </summary>
        /// <param name="city"></param>
        /// <param name="unit">Default <see cref="OWUnit.Standard"/></param>
        /// <param name="language">Default <see cref="Language.English"/></param>
        /// <returns></returns>
        Task<MultiWeatherConditions> GetForecast(string city, OWUnit unit = OWUnit.Standard, Language language = Language.English);

        /// <summary>
        ///     'darksky-like' weather forecast: current weather plus daily forecast for 7 days 
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="unit">Default <see cref="OWUnit.Standard"/></param>
        /// <param name="language">Default <see cref="Language.English"/></param>
        Task<ForecastDSL> GetForecastDSL(double latitude, double longitude, OWUnit unit = OWUnit.Standard, Language language = Language.English);

    }
}