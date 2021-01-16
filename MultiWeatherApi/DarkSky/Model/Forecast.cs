using System;
using System.Collections.Generic;
using MultiWeatherApi.Model;
using Newtonsoft.Json;

namespace MultiWeatherApi.DarkSky.Model {

    /// <summary>
    ///     A forecast for a particular location.
    /// </summary>
    [Serializable]
    public class Forecast {

        private GeoCoordinates _coordinates = new GeoCoordinates();

        /// <summary>the geo coordinates of this forecast</summary>
        public GeoCoordinates Coordinates {
            get => _coordinates;
            set => _coordinates = value;
        }

        /// <summary>the IANA time zone name for this location</summary>
        [JsonProperty("timezone")]
        public string TimeZone { get; set; }

        /// <summary>the time zone offset, in hours from GMT.</summary>
        [JsonProperty("offset")]
        public float TimeZoneOffset { get; set; }

        /// <summary>the current conditions at the requested location.</summary>
        [JsonProperty("currently")]
        public DataPoint Currently { get; set; }

        /// <summary>the minute-by-minute conditions for the next hour.</summary>
        [JsonProperty("minutely")]
        public ForecastDetails Minutely { get; set; }

        /// <summary>the hour-by-hour conditions for the next two days.</summary>
        [JsonProperty("hourly")]
        public ForecastDetails Hourly { get; set; }

        /// <summary>the daily conditions for the next week.</summary>
        [JsonProperty("daily")]
        public ForecastDetails Daily { get; set; }

        /// <summary>the metadata (flags) associated with this forecast.</summary>
        [JsonProperty("flags")]
        public Flags Flags { get; set; }

        /// <summary>Any weather alerts related to this location.</summary>
        [JsonProperty("alerts")]
        public IList<Alert> Alerts { get; set; }

        #region Internals

        /// <summary>the latitude of this forecast.</summary>
        [JsonProperty("latitude")]
        internal double Latitude {
            //get => _coordinates.Latitude;
            set => _coordinates.Latitude = value;
        }

        /// <summary>the longitude of this forecast.</summary>
        [JsonProperty("longitude")]
        internal double Longitude {
            //get => _coordinates.Longitude;
            set => _coordinates.Longitude = value;
        }

        #endregion
    }
}