using System;
using Newtonsoft.Json;

namespace MultiWeatherApi.OpenWeather.Model {

    /// <summary>
    ///     This is the class of openweather that has temperatures and apparent temperatures
    /// </summary>
    [Serializable]
    internal class TemperatureMain {

        [JsonProperty("temp")]
        public float? Temperature { get; set; }

        [JsonProperty("feels_like")]
        public float? ApparentTemperature { get; set; }

        [JsonProperty("temp_min")]
        public float? TemperatureMin { get; set; }

        [JsonProperty("temp_max")]
        public float? TemperatureMax { get; set; }

        [JsonProperty("humidity")]
        public int? Humidity { get; set; }

        [JsonProperty("pressure")]
        public float? Pressure { get; set; }
    }

}
