using System;
using System.Collections.Generic;
using MultiWeatherApi.Model;
using Newtonsoft.Json;

namespace MultiWeatherApi.OpenWeather.Model {

    /// <summary>
    ///     A set of weather conditions for a time frame (<see cref="Items"/>)
    /// </summary>
    [Serializable]
    public class MultiWeatherConditions {

        [JsonProperty("city")]
        public City City { get; set; } = new City();

        /// <summary>City geo location</summary>
        public GeoCoordinates Coordinates { get; set; }

        [JsonProperty("cod")]
        public string Cod { get; set; }

        [JsonProperty("message")]
        public double Message { get; set; }

        [JsonProperty("cnt")]
        public int Cnt { get; set; }

        [JsonProperty("list")]
        public List<WeatherConditions> Items { get; set; }
    }
}
