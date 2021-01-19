using System;
using MultiWeatherApi.Mappers;

namespace MultiWeatherApi {

    /// <summary>
    ///     Concreate factory class to create the required weather service wrapper
    /// </summary>
    public class WeatherFactory {

        /// <summary>Use this key to use the DarkSky service</summary>
        public static readonly Guid DarkSkyServiceId = new Guid("B9AD579A-27BB-4C93-B0BA-253AD7D64456");

        /// <summary>Use this key to use the OpenWeatherMap service</summary>
        public static readonly Guid OpenWeatherServiceId = new Guid("32B4F4D9-DA26-4F47-9826-FB8E6C5F298D");

        static WeatherFactory() {
            // init the tinyMapper maps
            TinyMappers.Maps();
        }

        /// <summary>
        ///     Factory method to create the service.
        /// </summary>
        /// <remarks>Inherit this method to add new services</remarks>
        /// <param name="serviceId"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public virtual IWeatherService Create(Guid serviceId, string apiKey) {
            if (WeatherFactory.DarkSkyServiceId.Equals(serviceId)) {
                return new DarkSkyWrapper(apiKey);
            } else if (WeatherFactory.OpenWeatherServiceId.Equals(serviceId)) {
                return new OpenWeatherWrapper(apiKey);
            }

            throw new ArgumentException($"No type registered for the passed service id ({serviceId})");
        }

    }

}
