using System;
using System.Collections.Generic;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather.Helpers;
using Newtonsoft.Json;

namespace MultiWeatherApi.OpenWeather.Model {

    /// <summary>
    ///     The weather conditions at a particular location and time frame.
    /// </summary>
    [Serializable]
    public class DataPointDSL {

        private Temperature _temperature = new Temperature();
        private Temperature _apparentTemperature = new Temperature();
        private Wind _wind = new Wind();

        /// <summary>A human-readable summary of the weather conditions of this data point</summary>
        public string Summary => WeatherInfo?[0]?.Summary;

        /// <summary>A human-readable full description of the weather conditions</summary>
        public string Description => WeatherInfo?[0]?.Description;

        /// <summary>Icon code</summary>
        public string Icon => WeatherInfo?[0]?.Icon;

        /// <summary>Icon URL, if available</summary>
        public string IconUrl => WeatherInfo?[0]?.IconUrl;

        /// <summary>the average visibility (default meters)</summary>
        [JsonProperty("visibility")]
        public int? Visibility { get; set; }

        /// <summary>the wind features</summary>
        public Wind Wind {
            get => _wind;
            set => _wind = value;
        }

        /// <summary>Various real temperatures of this time frame</summary>
        [JsonProperty("temp"), JsonConverter(typeof(MyTemperatureConverter))]
        public Temperature Temperature {
            get => _temperature;
            set => _temperature = value;
        }

        /// <summary>Various apparent temperatures of this time frame</summary>
        [JsonProperty("feels_like"), JsonConverter(typeof(MyTemperatureConverter))]
        public Temperature ApparentTemperature {
            get => _apparentTemperature;
            set => _apparentTemperature = value;
        }

        /// <summary>Time of this data point (unix, UTC). See also <see cref="Time"/></summary>
        [JsonProperty("dt")]
        public int UnixTime { get; set; }

        /// <summary>Sunrise time (unix, UTC). See also <see cref="SunriseTime"/></summary>
        [JsonProperty("sunrise")]
        public int? SunriseUnixTime { get; set; }

        /// <summary>Sunset time (unix, UTC). See also <see cref="SunsetTime"/></summary>
        [JsonProperty("sunset")]
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

        /// <summary>UV index</summary>
        [JsonProperty("uvi")]
        public float UVIndex { get; set; }

        /// <summary>the percentage of cloud cover (from 0 to 100).</summary>
        [JsonProperty("clouds")]
        public int Cloudness { get; set; }

        /// <summary>Snow volume (where available). Default in mm.</summary>
        [JsonProperty("snow"), JsonConverter(typeof(MySnowRainConverter))]
        public float? Snow { get; set; }

        /// <summary>Rain volume (where available). Default in mm.</summary>
        [JsonProperty("rain"), JsonConverter(typeof(MySnowRainConverter))]
        public float? Rain { get; set; }

        /// <summary>Probability of precipitation (0..1)</summary>
        [JsonProperty("pop")]
        public float? ProbOfPrecipitation { get; set; }

        [JsonProperty("weather")]
        public List<WeatherInfo> WeatherInfo { get; set; }

        #region Internals


        [JsonProperty("humidity")]
        internal int? Humidity {
            //get => _temperature.Humidity;
            set => _temperature.Humidity = value;
        }

        [JsonProperty("dew_point")]
        internal float? DewPoint {
            //get => _temperature.DewPoint;
            set => _temperature.DewPoint = value;
        }

        /// <summary>the wind speed</summary>
        [JsonProperty("wind_speed")]
        internal float WindSpeed {
            //get => _wind.Speed;
            set => _wind.Speed = value;
        }

        /// <summary>
        ///     the direction (in degrees) the wind is coming from.
        /// </summary>
        [JsonProperty("wind_deg")]
        internal int WindBearing {
            //get => _wind.Bearing;
            set => _wind.Bearing = value;
        }

        /// <summary>The pressure</summary>
        [JsonProperty("pressure")]
        internal int Pressure {
            //get => (int)_temperature.Pressure.GetValueOrDefault();
            set => _temperature.Pressure = value;
        }

        #endregion
    }

}
