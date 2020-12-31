using MultiWeatherApi.DarkSky;
using MultiWeatherApi.OpenWeather;

namespace MultiWeatherApi {

    /// <summary>
    ///     Interface for the weather api concreate factory class
    /// </summary>
    public interface IWeatherFactory {

        /// <summary>
        ///     Override this metho if you want to add a new weather service to the set of available weather services
        /// </summary>
        /// <typeparam name="T">Currentyl can only be <see cref="DarkSkyService"/> or <see cref="OpenWeatherService"/></typeparam>
        /// <param name="apiKey">the api used to access the selected weather service</param>
        IWeatherService Create<T>(string apiKey) where T : WeatherServiceBase;
    }
}