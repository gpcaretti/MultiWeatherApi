using System;
using Newtonsoft.Json;

namespace MultiWeatherApi.OpenWeather.Model {

    /// <summary>
    ///     An severe weather alert issued by a weather service for a particular location.
    /// </summary>
    [Serializable]
    public class AlertZ {

        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("end")]
        public int End { get; set; }

        [JsonProperty("sender_name")]
        public string Sender { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
