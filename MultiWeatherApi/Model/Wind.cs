using System;
using Newtonsoft.Json;

namespace MultiWeatherApi.Model {

    /// <summary>
    ///     Windows speed and direction
    /// </summary>
    [Serializable]
    public class Wind {

        /// <summary>the wind speed.</summary>
        [JsonProperty("speed")]
        public float Speed { get; set; }

        /// <summary>the direction (in degrees) the wind is coming from.</summary>
        [JsonProperty("deg")]
        public int Bearing { get; set; }

        /// <summary>the wind gust speed.</summary>
        [JsonProperty("speed")]
        public float? GustSpeed { get; set; }

        /// <summary>Time of wind gust (unix, UTC)</summary>
        public int? GustUnixTime { get; internal set; }
    }

}
