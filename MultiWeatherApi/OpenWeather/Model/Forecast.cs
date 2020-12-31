using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiWeatherApi.OpenWeather.Model {

    [Serializable, DataContract]
    public class Forecast {
        [DataMember(Name = "city")]
        public City City { get; set; } = new City();

        public string CityName { get => City.Name; }

        public Coordinates Coordinates { get => City.Coord ?? new Coordinates(); }

        [DataMember(Name = "cod")]
        public string Cod { get; set; }

        [DataMember(Name = "message")]
        public double Message { get; set; }

        [DataMember(Name = "cnt")]
        public int Cnt { get; set; }

        [DataMember(Name = "list")]
        public IList<WeatherDto> Items { get; set; }

        ////[JsonIgnore]
        //public double TemperatureMin => Items.Select(item => item.TemperatureMin).Min();

        ////[JsonIgnore]
        //public double TemperatureMax => Items.Select(item => item.TemperatureMax).Max();
    }
}
