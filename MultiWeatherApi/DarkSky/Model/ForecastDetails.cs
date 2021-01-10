using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MultiWeatherApi.DarkSky.Model {

    /// <summary>
    ///     Forecast details
    /// </summary>
    [Serializable]
    public class ForecastDetails {

        /// <summary>
        ///     a human-readable summary of the forecast.
        /// </summary>
        [JsonProperty("summary")]
        public string Summary { get; set; }

        /// <summary>
        ///     machine-readable text that can be used to select an icon to display.
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        ///     the individual data points that make up this forecast.
        /// </summary>
        [JsonProperty("data")]
        public List<DataPoint> Data { get; set; }
    }

}
