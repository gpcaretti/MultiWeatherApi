using System;
using Newtonsoft.Json;

namespace MultiWeatherApi.OpenWeather.Model {
    [Serializable]
    public class WeatherInfo {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("main")]
        public string Summary { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
 
        //[JsonIgnore]
        public string IconUrl => $"https://openweathermap.org/img/wn/{Icon}.png";
    }

}
