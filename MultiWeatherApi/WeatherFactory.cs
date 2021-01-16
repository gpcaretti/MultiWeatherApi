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

        protected static readonly object _servicesLock = new object();
        protected static readonly IDictionary<Guid, Type> _availableServices = new Dictionary<Guid, Type>(8);

        public static ICollection<Guid> ServiceKeys { get => _availableServices.Keys; }

        static WeatherFactory() {
            // init the tinyMapper maps
            Mappering.Maps();
        }

        public WeatherFactory() {
            // register the available services
            RegisterServices();
        }

        public IWeatherService Create(Guid serviceId, params object[] args) {
            if (_availableServices.TryGetValue(serviceId, out Type type))
                return (IWeatherService)Activator.CreateInstance(type, args);

            throw new ArgumentException($"No type registered for the passed service id ({serviceId})");
        }

        protected virtual void RegisterServices() {
            RegisterIfNotExist<OpenWeatherWrapper>(OpenWeatherWrapper._uniqueGuid);
            RegisterIfNotExist<DarkSkyWrapper>(DarkSkyWrapper._uniqueGuid);
        }

        public virtual void RegisterIfNotExist<TDerived>(Guid id) where TDerived : IWeatherService {
            var type = typeof(TDerived);
            if (type.IsInterface || type.IsAbstract /*|| !typeof(IWeatherService).IsAssignableFrom(type) */) {
                throw new ArgumentException($"Only concreted implementation of {typeof(IWeatherService)} can be passed to this method.", nameof(TDerived));
            }
            
            // before register it, double check the id is not yet present
            if (!_availableServices.ContainsKey(id)) {
                lock (_servicesLock) {
                    if (!_availableServices.ContainsKey(id)) _availableServices.Add(id, type);
                }
            }
        }

    }

}
