using System;
using System.Net.Http;
using MultiWeatherApi.Mappers;

namespace MultiWeatherApi {

    /// <summary>
    ///     Concreate factory class to create the required weather service wrapper
    /// </summary>
    public class WeatherFactory : IWeatherFactory {

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
        /// <param name="apiKey">The API key to use.</param>
        /// <param name="handler">the http message handler. If null use the default one.</param>
        public virtual IWeatherService Create(Guid serviceId, string apiKey, HttpMessageHandler handler = null) {
            if (WeatherFactory.DarkSkyServiceId.Equals(serviceId)) {
                return new DarkSkyWrapper(apiKey, handler);
            } else if (WeatherFactory.OpenWeatherServiceId.Equals(serviceId)) {
                return new OpenWeatherWrapper(apiKey, handler);
            }

            throw new ArgumentException($"No type registered for the passed service id ({serviceId})");
        }

    }

}
