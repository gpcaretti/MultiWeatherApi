using System;
using System.Collections.Generic;
using MultiWeatherApi.Model;
using Newtonsoft.Json;

namespace MultiWeatherApi.OpenWeather.Model {

    /// <summary>
    ///     A 'darksky-like' set of weather contitions / forecast for a particular location
    /// </summary>
    [Serializable]
    public class ForecastDSL {

        private GeoCoordinates _coordinates = new GeoCoordinates();

        /// <summary>
        ///     the geo coordinates of this forecast.
        /// </summary>
        public GeoCoordinates Coordinates {
            get => _coordinates;
            set => _coordinates = value;
        }

        /// <summary>
        ///     the IANA time zone name for this location.
        /// </summary>
        [JsonProperty("timezone")]
        public string TimeZone { get; set; }

        /// <summary>
        ///     the time zone offset, in hours from GMT.
        /// </summary>
        [JsonProperty("timezone_offset")]
        public int TimeZoneOffset { get; set; }

        /// <summary>
        ///     the current conditions at the requested location.
        /// </summary>
        [JsonProperty("current")]
        public DataPointDSL Current { get; set; }

        /// <summary>
        ///     the minute-by-minute conditions for the next hour.
        /// </summary>
        [JsonProperty("minutely")]
        public IList<MinutelyDataPoint> Minutely { get; set; }

        /// <summary>
        ///     the hour-by-hour conditions for the next two days
        /// </summary>
        [JsonProperty("hourly")]
        public IList<DataPointDSL> Hourly { get; set; }

        /// <summary>
        ///     the day-by-day conditions for the urrent and the next 7 days.
        /// </summary>
        [JsonProperty("daily")]
        public IList<DataPointDSL> Daily { get; set; }

        /// <summary>
        ///     any weather alerts related to this location.
        /// </summary>
        [JsonProperty("alerts")]    //, JsonConverter(typeof(MyAlertsConverter))]
        public IList<Alert> Alerts { get; set; }

        /// <summary>
        ///     the latitude of this forecast.
        /// </summary>
        [JsonProperty("lat")]
        internal double Latitude { 
            //get => _coordinates.Latitude;
            set => _coordinates.Latitude = value;
        }

        /// <summary>
        ///     the longitude of this forecast.
        /// </summary>
        [JsonProperty("lon")]
        internal double Longitude {
            //get => _coordinates.Longitude;
            set => _coordinates.Longitude = value;
        }

    }

}
