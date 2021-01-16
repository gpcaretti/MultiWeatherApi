using System;
using Newtonsoft.Json;

namespace MultiWeatherApi.Model {

    /// <summary>
    ///     Temperatures of the time frame they relate to
    /// </summary>
    [Serializable]
    public class Temperature {

        /// <summary>Daily temperature</summary>
        [JsonProperty("day")]
        public float? Daily { get; set; }

        /// <summary>Morning temperature</summary>
        [JsonProperty("morn")]
        public float? Morning { get; set; }

        /// <summary>Evening temperature</summary>
        [JsonProperty("eve")]
        public float? Evening { get; set; }

        /// <summary>Night temperature</summary>
        [JsonProperty("night")]
        public float? Night { get; set; }

        /// <summary>Min temperature of the day</summary>
        [JsonProperty("min")]
        public float? Min { get; set; }

        /// <summary>Max temperature of the day.</summary>
        [JsonProperty("max")]
        public float? Max { get; set; }

        /// <summary>Dew point</summary>
        public float? DewPoint { get; set; }

        /// <summary>Humidity (percentage)</summary>
        [JsonProperty("humidity")]
        public int? Humidity { get; set; }

        /// <summary>Pressure at the level sea</summary>
        [JsonProperty("pressure")]
        public float? Pressure { get; set; }
    }

}
