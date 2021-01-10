using System;
using Newtonsoft.Json;

namespace MultiWeatherApi.OpenWeather.Model {

    /// <summary>
    ///     Minute by minute weather info (rain)
    /// </summary>
    [Serializable]
    public class MinutelyDataPoint {

        /// <summary>Gets or sets the time of this data point (unix, UTC)</summary>
        [JsonProperty("dt")]
        internal int UnixTime { get; set; }

        /// <summary>Gets or sets the time of this data point (unix, UTC)</summary>
        public DateTimeOffset Time {
            get => UnixTime.ToDateTimeOffset();
            set => UnixTime = value.ToUnixTime();
        }

        /// <summary>Precipitation quantity</summary>
        [JsonProperty("precipitation")]
        public float Rain { get; set; }
    }

}
