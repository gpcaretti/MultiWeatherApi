using System;
using System.Runtime.Serialization;

namespace MultiWeatherApi.OpenWeatherMap.Model {

    [Serializable, DataContract]
    public class Main {

        [DataMember(Name = "temp")]
        public double Temperature { get; set; }

        [DataMember(Name = "feels_like")]
        public double ApparentTemperature { get; set; }

        [DataMember(Name = "temp_min")]
        public double TemperatureMin { get; set; }

        [DataMember(Name = "temp_max")]
        public double TemperatureMax { get; set; }

        [DataMember(Name = "humidity")]
        public long Humidity { get; set; }
    }


    [Serializable, DataContract]
    public class WeatherInfo {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "main")]
        public string ShortDescription { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "icon")]
        public string Icon { get; set; }
 
        //[JsonIgnore]
        public string IconUrl => $"https://openweathermap.org/img/wn/{Icon}.png";
    }

    [Serializable, DataContract]
    public class Wind {
        [DataMember(Name = "speed")]
        public double Speed { get; set; }

        [DataMember(Name = "deg")]
        public int Degree { get; set; }
    }

    [Serializable, DataContract]
    public class Coordinates {

        public Coordinates() {
        }

        public Coordinates(double latitude, double longitude) {
            Latitude = latitude;
            Longitude = longitude;
        }

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lon")]
        public double Longitude { get; set; }
    }

    [Serializable, DataContract]
    public class Sys {
        [DataMember(Name = "type")]
        public int Type { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; } = string.Empty;
    }

    public class City {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "coord")]
        public Coordinates Coord { get; set; } = new Coordinates();
        [DataMember(Name = "country")]
        public string Country { get; set; }
        [DataMember(Name = "population")]
        public int Population { get; set; }
        [DataMember(Name = "sys")]
        public Sys System { get; set; } = new Sys();
    }

}
