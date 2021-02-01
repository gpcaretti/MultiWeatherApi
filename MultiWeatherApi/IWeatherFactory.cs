using System;
using System.Net.Http;

namespace MultiWeatherApi {

    /// <summary>
    ///     Interface for the factory class to create the required weather service.
    /// </summary>
    public interface IWeatherFactory {

        /// <summary>
        ///     Factory method to create the service.
        /// </summary>
        /// <remarks>Inherit this method to add new services</remarks>
        /// <param name="serviceId"></param>
        /// <param name="apiKey"></param>
        /// <param name="handler">the http message handler. If null use the default one.</param>
        IWeatherService Create(Guid serviceId, string apiKey, HttpMessageHandler handler = null);

    }
}