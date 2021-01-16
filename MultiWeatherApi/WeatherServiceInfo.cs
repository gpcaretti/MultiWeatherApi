using System;

namespace MultiWeatherApi {

    public class WeatherServiceInfo {
        public Guid Id { get; }
        public String Name { get; }

        protected WeatherServiceInfo(Guid id, string name) {
            Id = id;
            Name = name;
        }

        public static WeatherServiceInfo DarkSky = new WeatherServiceInfo(DarkSkyWrapper._uniqueGuid, "DarkSky");
        public static WeatherServiceInfo OpenWeather = new WeatherServiceInfo(OpenWeatherWrapper._uniqueGuid, "OpenWeatherMap");
    }
}
