using System;
using Newtonsoft.Json;

namespace MultiWeatherApi.Model {

    /// <summary>
    ///     Geographical coordinates
    /// </summary>
    [Serializable]
    public class GeoCoordinates : IEquatable<GeoCoordinates> {
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

        public bool Equals(GeoCoordinates other) {
            if (other == null) return false;
            return this.Latitude.Equals(other.Latitude) && this.Longitude.Equals(other.Longitude);
        }

        public override int GetHashCode() {
            unchecked { // integer overflows are accepted here
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ this.Latitude.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Longitude.GetHashCode();
                return hashCode;
            }
        }
    }

}
