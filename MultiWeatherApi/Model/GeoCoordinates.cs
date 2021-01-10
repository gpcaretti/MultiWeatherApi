using System;
using Newtonsoft.Json;

namespace MultiWeatherApi.Model {

    /// <summary>
    ///     Geographical coordinates
    /// </summary>
    [Serializable]
    public class GeoCoordinates {

        /// <summary>Latidute</summary>
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        /// <summary>longitude</summary>
        [JsonProperty("lon")]
        public double Longitude { get; set; }
    }

}
