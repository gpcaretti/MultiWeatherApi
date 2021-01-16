using System;
using System.Collections.Generic;
using MultiWeatherApi.DarkSky;
using MultiWeatherApi.Mappers;
using MultiWeatherApi.OpenWeather;

namespace MultiWeatherApi {

    /// <summary>
    ///     Concreate factory class to create the required weather service wrapper
    /// </summary>
    public class WeatherFactory {

        public static readonly Guid DarkSkyServiceId = new Guid("B9AD579A-27BB-4C93-B0BA-253AD7D64456");
        public static readonly Guid OpenWeatherServiceId = new Guid("32B4F4D9-DA26-4F47-9826-FB8E6C5F298D");

        static WeatherFactory() {
            // init the tinyMapper maps
            Mappering.Maps();
        }

        public IWeatherService Create(Guid serviceId, string apiKey) {
            if (WeatherFactory.DarkSkyServiceId.Equals(serviceId)) {
                return new DarkSkyWrapper(apiKey);
            } else if (WeatherFactory.OpenWeatherServiceId.Equals(serviceId)) {
                return new OpenWeatherWrapper(apiKey);
            }

            throw new ArgumentException($"No type registered for the passed service id ({serviceId})");
        }

    }

}
