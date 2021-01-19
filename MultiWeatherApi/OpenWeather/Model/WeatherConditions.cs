using System;
using System.Collections.Generic;
using MultiWeatherApi.Model;
using Newtonsoft.Json;

namespace MultiWeatherApi.OpenWeather.Model {

    /// <summary>
    ///     Weather conditions / forecast for a specific day (<see cref="Time"/>)
    /// </summary>
    [Serializable]
    public class WeatherConditions {

        private Temperature _temperature = new Temperature();
        private Temperature _apparentTemperature = new Temperature();

        /// <summary>the time of this data point (unix, UTC)</summary>
        [JsonProperty("dt")]
        internal int UnixTime { get; set; }

        /// <summary>the time of this data point (unix, UTC)</summary>
        public DateTimeOffset Time {
            get => UnixTime.ToDateTimeOffset();
            set => UnixTime = value.ToUnixTime();
        }

        /// <summary>City info</summary>
        public City City { get; set; } = new City();

        /// <summary>City geo location</summary>
        [JsonProperty("coord")]
        public GeoCoordinates Coordinates { get; set; }

        [JsonProperty("weather")]
        public List<WeatherInfo> WeatherInfo { get; set; }

        /// <summary>
        ///     Various temperatures of this time frame
        /// </summary>
        public Temperature Temperature {
            get => _temperature;
            set => _temperature = value;
        }

        /// <summary>
        ///     Various apparent temperatures of this time frame
        /// </summary>
        public Temperature ApparentTemperature {
            get => _apparentTemperature;
            set => _apparentTemperature = value;
        }

        [JsonProperty("visibility")]
        public long Visibility { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; } = new Wind();

        [JsonIgnore]
        public string Summary => WeatherInfo?[0]?.Summary;

        [JsonIgnore]
        public string Description => WeatherInfo?[0]?.Description;

        [JsonIgnore]
        public string Icon => WeatherInfo?[0]?.Icon;

        [JsonIgnore]
        public string IconUrl => WeatherInfo?[0]?.IconUrl;

        #region Internals

        [JsonProperty("name")]
        internal string CityName {
            //get => City.Name;
            set => City.Name = value;
        }

        [JsonProperty("timezone")]
        internal string TimeZone {
            //get => City.Name;
            set => City.TimeZone = value;
        }

        /// <summary>City Additional info</summary>
        [JsonProperty("sys")]
        internal CityAdditionalSys Sys {
            //get => City.Sys;
            set => City.Sys = value;
        }

        [JsonProperty("main")]
        internal TemperatureMain Main {
            //get; 
            set {
                if (value.Temperature.HasValue) _temperature.Daily = value.Temperature;
                if (value.TemperatureMax.HasValue) _temperature.Max = value.TemperatureMax;
                if (value.TemperatureMin.HasValue) _temperature.Min = value.TemperatureMin;
                if (value.Humidity.HasValue) _temperature.Humidity = value.Humidity;
                if (value.Pressure.HasValue) _temperature.Pressure = value.Pressure;

                if (value.ApparentTemperature.HasValue) _apparentTemperature.Daily = value.ApparentTemperature;
            }
        }

        #endregion
    }

}
