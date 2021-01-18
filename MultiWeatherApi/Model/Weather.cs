using System;
using System.Collections.Generic;

namespace MultiWeatherApi.Model {

    /// <summary>
    ///     Current weather conditions for a particular location.
    /// </summary>
    [Serializable]
    public class Weather : IWeatherData {

        /// <summary>the geo coordinates of this forecast</summary>
        public GeoCoordinates Coordinates { get; set; }

        /// <summary>the IANA time zone name for this location.</summary>
        public string TimeZone { get; set; }

        /// <summary>the time zone offset, in hours from GMT.</summary>
        public float TimeZoneOffset { get; set; }

        /// <summary>A human-readable summary of the weather conditions of this data point</summary>
        public string Summary { get; set; }

        /// <summary>A human-readable full description of the weather conditions</summary>
        public string Description { get; set; }

        /// <summary>Icon code</summary>
        public string Icon { get; set; }

        /// <summary>Icon URL, if available</summary>
        public string IconUrl { get; set; }

        // TODO normalize the same unit for OpenWeather (meters/yards?) and DarkSky (km/miles)
        /// <summary>the average visibility. Can be null on daily data point</summary>
        public float? Visibility { get; set; }

        /// <summary>the wind features</summary>
        public Wind Wind { get; set; }

        /// <summary>Any weather alerts related to this location.</summary>
        public IList<Alert> Alerts { get; set; }

        /// <summary>Various real temperatures of this time frame</summary>
        public Temperature Temperature { get; set; }

        /// <summary>Various apparent temperatures of this time frame</summary>
        public Temperature ApparentTemperature { get; set; }

        //TODO public City City { get; set; }

        /// <summary>Time of this data point (unix, UTC). See also <see cref="Time"/></summary>
        public int UnixTime { get; set; }

        /// <summary>Sunrise time (unix local time). See also <see cref="SunriseTime"/></summary>
        public int? SunriseUnixTime { get; set; }

        /// <summary>Sunset time (unix local time). See also <see cref="SunsetTime"/></summary>
        public int? SunsetUnixTime { get; set; }

        /// <summary>Time of this data point (UTC). See also <see cref="UnixTime"/></summary>
        public DateTimeOffset Time {
            get => UnixTime.ToDateTimeOffset();
            set => UnixTime = value.ToUnixTime();
        }

        /// <summary>Sunrise local time. See also <see cref="SunsetUnixTime"/></summary>
        /// <remarks>Null for hourly details</remarks>
        public DateTimeOffset? SunriseTime {
            get => (DateTimeOffset?)(SunriseUnixTime?.ToDateTimeOffset());
            set => SunriseUnixTime = value.HasValue ? value.Value.ToUnixTime() : (int?)null;
        }

        /// <summary>Sunset local time. See also <see cref="SunsetUnixTime"/></summary>
        /// <remarks>Null for hourly details</remarks>
        public DateTimeOffset? SunsetTime {
            get => (DateTimeOffset?)(SunsetUnixTime?.ToDateTimeOffset());
            set => SunsetUnixTime = value.HasValue ? value.Value.ToUnixTime() : (int?)null;
        }

        ///// <summary>
        /////     the hour-by-hour conditions for the next two days.
        ///// </summary>
        //[JsonProperty("hourly")]
        //public ForecastDetails Hourly { get; set; }

    }
}