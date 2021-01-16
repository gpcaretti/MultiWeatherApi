using System;
using MultiWeatherApi.Model;
using Newtonsoft.Json;

namespace MultiWeatherApi.DarkSky.Model {

    /// <summary>
    ///     The weather conditions at a particular location and time frame.
    /// </summary>
    [Serializable]
    public class DataPoint {

        private Temperature _temperature = new Temperature();
        private Temperature _apparentTemperature = new Temperature();
        private Wind _wind = new Wind();

        /// <summary>A human-readable summary of this data point.</summary>
        [JsonProperty("summary")]
        public string Summary { get; set; }

        /// <summary>Icon code</summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>the average visibility (km/miles). Optional</summary>
        [JsonProperty("visibility")]
        public float? Visibility { get; set; }

        /// <summary>the wind features</summary>
        public Wind Wind {
            get => _wind;
            set => _wind = value;
        }

        /// <summary>Various real temperatures of this time frame</summary>
        [JsonProperty("temperatures")]
        public Temperature Temperature {
            get => _temperature;
            set => _temperature = value;
        }

        /// <summary>Various apparent temperatures of this time frame</summary>
        [JsonProperty("apparent_temperatures")]
        public Temperature ApparentTemperature {
            get => _apparentTemperature;
            set => _apparentTemperature = value;
        }

        /// <summary>Time of this data point (unix, UTC). See also <see cref="Time"/></summary>
        [JsonProperty("time")]
        public int UnixTime { get; set; }

        /// <summary>Sunrise time (unix, UTC). See also <see cref="SunriseTime"/></summary>
        [JsonProperty("sunriseTime")]
        public int? SunriseUnixTime { get; set; }

        /// <summary>Sunset time (unix, UTC). See also <see cref="SunsetTime"/></summary>
        [JsonProperty("sunsetTime")]
        public int? SunsetUnixTime { get; set; }

        /// <summary>Time of this data point (UTC). See also <see cref="UnixTime"/></summary>
        [JsonProperty("dt_as_offset")]
        public DateTimeOffset Time {
            get => UnixTime.ToDateTimeOffset();
            set => UnixTime = value.ToUnixTime();
        }

        /// <summary>Sunrise time (UTC). See also <see cref="SunsetUnixTime"/></summary>
        /// <remarks>Null for hourly details</remarks>
        public DateTimeOffset? SunriseTime {
            get => (DateTimeOffset?)(SunriseUnixTime?.ToDateTimeOffset());
            set => SunriseUnixTime = value.HasValue ? value.Value.ToUnixTime() : (int?)null;
        }

        /// <summary>Sunset time (UTC). See also <see cref="SunsetUnixTime"/></summary>
        /// <remarks>Null for hourly details</remarks>
        public DateTimeOffset? SunsetTime {
            get => (DateTimeOffset?)(SunsetUnixTime?.ToDateTimeOffset());
            set => SunsetUnixTime = value.HasValue ? value.Value.ToUnixTime() : (int?)null;
        }

        /// <summary>UV index (optional)</summary>
        [JsonProperty("uvIndex")]
        public int? UVIndex { get; set; }

        /// <summary>UV index time (optional). Available only on daily</summary>
        [JsonProperty("uvIndexTime")]
        public int? UVIndexTime { get; set; }

        /// <summary>the percentage of cloud cover (from 0 to 100).</summary>
        public int Cloudness { get; set; }

        /// <summary>the columnar density of total atmospheric ozone, in Dobson units.</summary>
        [JsonProperty("ozone")]
        public float Ozone { get; set; }

        [JsonProperty("moonPhase")]
        public float? MoonPhase { get; set; }

        /// <summary>
        ///     the average expected precipitation assuming any precipitation occurs.
        /// </summary>
        [JsonProperty("precipIntensity")]
        public float? PrecipitationIntensity { get; set; }

        /// <summary>
        ///     the probability of precipitation (from 0 to 1).
        /// </summary>
        [JsonProperty("precipProbability")]
        public float? PrecipitationProbability { get; set; }

        /// <summary>
        ///     the type of precipitation (rain, sleet, or snow).
        /// </summary>
        [JsonProperty("precipType")]
        public string PrecipitationType { get; set; }

        [JsonProperty("precipIntensityMax")]
        public float? PrecipIntensityMax { get; set; }

        [JsonProperty("precipIntensityMaxTime")]
        public int? PrecipIntensityMaxTime { get; set; }

        [JsonProperty("temperatureHighTime")]
        public int? TemperatureHighTime { get; set; }

        [JsonProperty("temperatureLowTime")]
        public int? TemperatureLowTime { get; set; }

        [JsonProperty("apparentTemperatureHighTime")]
        public int? ApparentTemperatureHighTime { get; set; }

        [JsonProperty("apparentTemperatureLowTime")]
        public int? ApparentTemperatureLowTime { get; set; }

        #region Internals

        [JsonProperty("windSpeed")]
        internal float WindSpeed {
            //get => _wind.Speed;
            set => _wind.Speed = value;
        }

        [JsonProperty("windBearing")]
        internal int WindBearing {
            //get => _wind.Bearing;
            set => _wind.Bearing = value;
        }

        [JsonProperty("windGust")]
        internal float? WindGustSpeed {
            //get => _wind.WindGustSpeed;
            set => _wind.GustSpeed = value;
        }

        /// <summary>Time of wind gust (unix, UTC)</summary>
        [JsonProperty("windGustTime")]
        internal int? WindGustTime {
            //get => _wind.WindGustUnixTime;
            set => _wind.GustUnixTime = value;
        }

        [JsonProperty("dewPoint")]
        internal float? DewPoint {
            //get => _temperature.DewPoint;
            set => _temperature.DewPoint = value;
        }

        [JsonProperty("humidity")]
        internal float? Humidity {
            //get => (_temperature.Humidity.HasValue) ? (_temperature.Humidity / 100.0f) : (float?)null;
            set => _temperature.Humidity = value.HasValue ? (int)Math.Round(value.Value * 100.0f, 0) : (int?)null;
        }

        [JsonProperty("temperature")]
        internal float? AirTemperature {
            //get => _temperature.Daily;
            set => _temperature.Daily = value;
        }

        [JsonProperty("apparentTemperature")]
        internal float? ApparentAirTemperature {
            //get => _apparentTemperature.Daily;
            set => _apparentTemperature.Daily = value;
        }

        [JsonProperty("temperatureLow")]
        internal float? TemperatureLow {
            //get => _temperature.Min;
            set => _temperature.Min = value;
        }

        [JsonProperty("temperatureHigh")]
        internal float? TemperatureHigh {
            //get => _temperature.Max;
            set => _temperature.Max = value;
        }

        [JsonProperty("apparentTemperatureHigh")]
        internal float? ApparentTemperatureHigh {
            //get => _apparentTemperature.Max;
            set => _apparentTemperature.Max = value;
        }

        [JsonProperty("apparentTemperatureLow")]
        internal float? ApparentTemperatureLow {
            //get => _apparentTemperature.Min;
            set { _apparentTemperature.Min = value; }
        }

        [JsonProperty("pressure")]
        internal float Pressure {
            //get => _temperature.Pressure.GetValueOrDefault();
            set => _temperature.Pressure = value;
        }

        /// <summary>the percentage of cloud cover (from 0 to 1).</summary>
        [JsonProperty("cloudCover")]
        internal float CloudCover {
            // get => (float)(Cloudness / 100.0f); 
            set => Cloudness = (int)Math.Round(value * 100.0f, 0);
        }

        #endregion
    }

}
