using System;
using Newtonsoft.Json;

namespace MultiWeatherApi.Model {

    /// <summary>
    ///     Geographical coordinates
    /// </summary>
    [Serializable]
    public class GeoCoordinates {
        /// <summary>Default contructor</summary>
        public GeoCoordinates() { 
        }

        /// <summary>Contructor setting lat and lon</summary>
        public GeoCoordinates(double latitude, double longitude) {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>Latidute</summary>
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        /// <summary>longitude</summary>
        [JsonProperty("lon")]
        public double Longitude { get; set; }
    }

}
