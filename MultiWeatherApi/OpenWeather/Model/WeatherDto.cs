using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiWeatherApi.OpenWeather.Model {

    [Serializable, DataContract]
    public class WeatherDto {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "name")]
        public string CityName { get; set; }

        [DataMember(Name = "coord")]
        public Coordinates Coordinates { get; set; } = new Coordinates();

        /// <summary>Gets or sets the time of this data point (unix, UTC)</summary>
        [DataMember(Name = "dt")]
        public int UnixTime { get; set; }

        public DateTimeOffset Time { get => UnixTime.ToDateTimeOffset(); }

        [DataMember(Name = "sys")]
        public Sys System { get; set; } = new Sys();

        [DataMember(Name = "weather")]
        public List<WeatherInfo> Weather { get; set; }

        [DataMember(Name = "main")]
        public Main Main { get; set; } = new Main();

        [DataMember(Name = "visibility")]
        public long Visibility { get; set; }

        [DataMember(Name = "wind")]
        public Wind Wind { get; set; } = new Wind();

        //[JsonIgnore]
        public double? WindSpeed => Wind.Speed;

        //[JsonIgnore]
        public int? WindDegree => Wind.Degree;

        //[JsonIgnore]
        public string Country => System.Country;

        //[JsonIgnore]
        public double Temperature => Main.Temperature;

        //[JsonIgnore]
        public double ApparentTemperature => Main.ApparentTemperature;

        //[JsonIgnore]
        public double TemperatureMin => Main.TemperatureMin;

        //[JsonIgnore]
        public double TemperatureMax => Main.TemperatureMax;

        //[JsonIgnore]
        public long Humidity => Main.Humidity;

        //[JsonIgnore]
        public string Description => Weather?[0]?.Description;

        //[JsonIgnore]
        public string IconUrl => Weather?[0]?.IconUrl;
    }

}