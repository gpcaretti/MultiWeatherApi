using System;
using MultiWeatherApi.Model;
using Newtonsoft.Json;

namespace MultiWeatherApi.OpenWeather.Model {

    /// <summary>
    ///     Information about the city 
    /// </summary>
    [Serializable]
    public class City {

        private CityAdditionalSys _sys = new CityAdditionalSys();

        /// <summary>City name</summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        ///// <summary>City geo location</summary>
        //[JsonProperty("coord")]
        //public GeoCoordinates Coordinates { get; set; }

        /// <summary>City country</summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        ///     the IANA time zone name for this location.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>Country code (GB, JP etc.)</summary>
        public string CountryCode {
            get => _sys.CountryCode;
            set => _sys.CountryCode = value;
        }

        /// <summary>Gets or sets the time of this data point (unix, UTC)</summary>
        public DateTimeOffset SunriseTime {
            get => _sys.SunriseUnixTime.ToDateTimeOffset();
            set => _sys.SunriseUnixTime = value.ToUnixTime();
        }
        /// <summary>Gets or sets the time of this data point (unix, UTC)</summary>
        public DateTimeOffset SunsetTime {
            get => _sys.SunsetUnixTime.ToDateTimeOffset();
            set => _sys.SunsetUnixTime = value.ToUnixTime();
        }

        /// <summary>City Additional info</summary>
        [JsonProperty("sys")]
        internal CityAdditionalSys Sys { 
            //get => _sys; 
            set => _sys = value; 
        }
    }

    /// <summary>
    ///     Additional info about the city
    /// </summary>
    [Serializable]
    internal class CityAdditionalSys {

        /// <summary>Country code (GB, JP etc.)</summary>
        [JsonProperty("country")]
        public string CountryCode { get; set; } = string.Empty;

        /// <summary>Gets or sets the sunrise time of this data point (unix, UTC)</summary>
        [JsonProperty("sunrise")]
        internal int SunriseUnixTime { get; set; }

        /// <summary>Gets or sets the time of this data point (unix, UTC)</summary>
        public DateTimeOffset SunriseTime {
            get => SunriseUnixTime.ToDateTimeOffset();
            set => SunriseUnixTime = value.ToUnixTime();
        }

        /// <summary>Gets or sets the sunrise time of this data point (unix, UTC)</summary>
        [JsonProperty("sunset")]
        internal int SunsetUnixTime { get; set; }

        /// <summary>Gets or sets the time of this data point (unix, UTC)</summary>
        public DateTimeOffset SunsetTime {
            get => SunsetUnixTime.ToDateTimeOffset();
            set => SunsetUnixTime = value.ToUnixTime();
        }
    }

}
