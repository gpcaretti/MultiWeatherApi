using System;
using MultiWeatherApi.DarkSky;
using MultiWeatherApi.OpenWeather;

namespace MultiWeatherApi {

    /// <summary>
    ///     Concreate factory class to create the required weather service wrapper
    /// </summary>
    public class WeatherFactory : IWeatherFactory {

        /// <summary>
        ///     Override this metho if you want to add a new weather service to the set of available weather services
        /// </summary>
        /// <typeparam name="T">Currentyl can only be <see cref="DarkSkyService"/> or <see cref="OpenWeatherService"/></typeparam>
        /// <param name="apiKey">the api used to access the selected weather service</param>
        public virtual IWeatherService Create<T>(string apiKey) where T : WeatherServiceBase {
            if (typeof(T) == typeof(DarkSkyService)) {
                return new DarkSkyWrapper(apiKey);
            } else if (typeof(T) == typeof(OpenWeatherService)) {
                return new OpenWeatherWrapper(apiKey);
            } else {
                // default
                throw new NotImplementedException(typeof(T).ToString());
            }

        }
    }

}
