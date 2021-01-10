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

        /// <summary>Time of this data point (unix, UTC)</summary>
        [JsonProperty("dt")]
        public int UnixTime { get; set; }

        /// <summary>Time of this data point (unix, UTC)</summary>
        [JsonProperty("dt_as_offset")]
        public DateTimeOffset Time {
            get => UnixTime.ToDateTimeOffset();
            set => UnixTime = value.ToUnixTime();
        }

        /// <summary>the time of this data point (unix, UTC)</summary>
        /// <remarks>Null for hourly details</remarks>
        public DateTimeOffset? SunriseTime {
            get => (DateTimeOffset?)(SunriseUnixTime?.ToDateTimeOffset());
            set => SunriseUnixTime = value.HasValue ? value.Value.ToUnixTime() : (int?)null;
        }

        /// <summary>the time of this data point (unix, UTC)</summary>
        /// <remarks>Null for hourly details</remarks>
        public DateTimeOffset? SunsetTime {
            get => (DateTimeOffset?)(SunsetUnixTime?.ToDateTimeOffset());
            set => SunsetUnixTime = value.HasValue ? value.Value.ToUnixTime() : (int?)null;
        }

        /// <summary>Real temperatures</summary>
        [JsonProperty("temp"), JsonConverter(typeof(MyTemperatureConverter))]
        public Temperature Temperature {
            get => _temperature;
            set => _temperature = value;
        }

        /// <summary>Apparent temperatures</summary>
        [JsonProperty("feels_like"), JsonConverter(typeof(MyTemperatureConverter))]
        public Temperature ApparentTemperature {
            get => _apparentTemperature;
            set => _apparentTemperature = value;
        }

        /// <summary>the wind features</summary>
        public Wind Wind {
            get => _wind;
            set => _wind = value;
        }

        /// <summary>Rain volume (where available)</summary>
        [JsonProperty("rain"), JsonConverter(typeof(MyRainConverter))]
        public float? Rain { get; set; }

        /// <summary>Probability of precipitation (0..1)</summary>
        [JsonProperty("pop")]
        public float? ProbOfPrecipitation { get; set; }

        [JsonProperty("weather")]
        public IList<WeatherInfo> WeatherInfo { get; set; }

        /// <summary>UV index</summary>
        [JsonProperty("uvi")]
        public float UVIndex { get; set; }

        [JsonProperty("clouds")]
        public int Clouds { get; set; }

        [JsonProperty("visibility")]
        public int? Visibility { get; set; }

        #region Internal

        [JsonProperty("sunrise")]
        internal int? SunriseUnixTime { get; set; }

        [JsonProperty("sunset")]
        internal int? SunsetUnixTime { get; set; }

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
