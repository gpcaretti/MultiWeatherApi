using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiWeatherApi.OpenWeather.Model {

    [Serializable, DataContract]
    public class DailyForecast {

        [DataMember(Name = "lat")]
        internal double Latitude { get; set; }

        [DataMember(Name = "lon")]
        internal double Longitude { get; set; }

        public Coordinates Coordinates => new Coordinates(Latitude, Longitude);

        [DataMember(Name = "timezone")]
        public string Timezone { get; set; }

        [DataMember(Name = "timezone_offset")]
        public int TimezoneOffset { get; set; }

        [DataMember(Name = "current")]
        public Current Current { get; set; }

        [DataMember(Name = "daily")]
        public List<Daily> Daily { get; set; }
    }


    [Serializable, DataContract]
    public class Current {
        /// <summary>Gets or sets the time of this data point (unix, UTC)</summary>
        [DataMember(Name = "dt")]
        public int UnixTime { get; set; }
        public DateTimeOffset Time { get => UnixTime.ToDateTimeOffset(); }

        [DataMember(Name = "sunrise")]
        public int Sunrise { get; set; }

        [DataMember(Name = "sunset")]
        public int Sunset { get; set; }

        [DataMember(Name = "temp")]
        public double Temperature { get; set; }

        [DataMember(Name = "feels_like")]
        public double ApparentTemperature { get; set; }

        [DataMember(Name = "pressure")]
        public int Pressure { get; set; }

        [DataMember(Name = "humidity")]
        public int Humidity { get; set; }

        [DataMember(Name = "dew_point")]
        public double DewPoint { get; set; }

        [DataMember(Name = "uvi")]
        public int Uvi { get; set; }

        [DataMember(Name = "clouds")]
        public int Clouds { get; set; }

        [DataMember(Name = "visibility")]
        public int Visibility { get; set; }

        [DataMember(Name = "wind_speed")]
        public double WindSpeed { get; set; }

        [DataMember(Name = "wind_deg")]
        public int WindDegree { get; set; }

        [DataMember(Name = "weather")]
        public List<WeatherInfo> Weather { get; set; }
    }

    [Serializable, DataContract]
    public class Temperature {
        [DataMember(Name = "day")]
        public double Day { get; set; }
        public double ApparentDay { get; set; }

        [DataMember(Name = "min")]
        public virtual double Min { get; set; }
        public double ApparentMin { get; set; }

        [DataMember(Name = "max")]
        public virtual double Max { get; set; }
        public double ApparentMax { get; set; }

        [DataMember(Name = "night")]
        public double Night { get; set; }
        public double ApparentNight { get; set; }

        [DataMember(Name = "eve")]
        public double Eve { get; set; }
        public double ApparentEve { get; set; }

        [DataMember(Name = "morn")]
        public double Morning { get; set; }
        public double ApparentMorning { get; set; }
    }

    //[Serializable, DataContract]
    //public class ApparentTemperature : Temperature {
    //    public override double Min => (Math.Min(Day, Math.Min(Night, Morning)));
    //    public override double Max => (Math.Max(Day, Math.Max(Night, Morning)));
    //}

    public class Daily {
        /// <summary>Gets or sets the time of this data point (unix, UTC)</summary>
        [DataMember(Name = "dt")]
        public int UnixTime { get; set; }
        public DateTimeOffset Time { get => UnixTime.ToDateTimeOffset(); }

        [DataMember(Name = "sunrise")]
        public int Sunrise { get; set; }

        [DataMember(Name = "sunset")]
        public int Sunset { get; set; }

        [DataMember(Name = "temp")]
        internal Temperature Temp { get; set; }

        [DataMember(Name = "feels_like")]
        internal Temperature ApparentTemp {
            //get => Temp;
            set {
                Temp.ApparentDay = value.Day;
                Temp.ApparentMax = Math.Max(value.Day, Math.Max(value.Night, value.Morning));
                Temp.ApparentMin = Math.Min(value.Day, Math.Min(value.Night, value.Morning));
                Temp.ApparentEve = value.Eve;
                Temp.ApparentNight = value.Night;
                Temp.ApparentMorning = value.Morning;
            }
        }

        public Temperature Temperature => Temp;

        [DataMember(Name = "pressure")]
        public int Pressure { get; set; }

        [DataMember(Name = "humidity")]
        public int Humidity { get; set; }

        [DataMember(Name = "dew_point")]
        public double DewPoint { get; set; }

        [DataMember(Name = "wind_speed")]
        public double WindSpeed { get; set; }

        [DataMember(Name = "wind_deg")]
        public int WindDeg { get; set; }

        [DataMember(Name = "weather")]
        public List<WeatherInfo> Weather { get; set; }

        [DataMember(Name = "clouds")]
        public int Clouds { get; set; }

        [DataMember(Name = "pop")]
        public double Pop { get; set; }

        [DataMember(Name = "rain")]
        public double Rain { get; set; }

        [DataMember(Name = "uvi")]
        public double Uvi { get; set; }
    }

}
