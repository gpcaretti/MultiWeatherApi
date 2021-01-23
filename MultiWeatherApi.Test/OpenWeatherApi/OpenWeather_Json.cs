using System;
using System.IO;
using System.Threading.Tasks;
using Helpers;
using Microsoft.Extensions.Configuration;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather.Helpers;
using MultiWeatherApi.OpenWeather.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace OpenWeather.Test {

    /// <summary>
    ///     Tests of serialize/deserialize to/from json files
    /// </summary>
    public class OpenWeather_Json {
        private string _openWeatherApiKey;
        private string _darkSkyApiKey;

        /// <summary>
        ///     Sets up all tests by retrieving the API key from cfg file.
        /// </summary>
        public OpenWeather_Json() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("./xunit.config.json")
                .Build();
            _darkSkyApiKey = config["DarkSkyApiKey"];
            _openWeatherApiKey = config["OpenWeatherApiKey"];
        }

        [Fact]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task ParseJsonFromStream_using_OpenW_currentWeather_json() {
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
            var filename = "./Resources/OpenW_currentweather.json";

            var client = new WeatherServiceBase_Wrapper(_openWeatherApiKey);
            using (var jsonStream = File.OpenRead(filename)) {
                var output = client.ParseJsonFromStream_Wrapper<MultiWeatherApi.OpenWeather.Model.WeatherConditions>(jsonStream);
                output.Coordinates.Latitude.ShouldBe(44.4667);
                output.Coordinates.Longitude.ShouldBe(11.4333);
                output.WeatherInfo[0].Summary.ShouldBe("Clouds");
                output.WeatherInfo[0].Description.ShouldBe("nubi sparse");
                output.WeatherInfo[0].Icon.ShouldBe("03d");
                output.City.Name.ShouldBe("Bologna");
                output.City.CountryCode.ShouldBe("IT");
                output.City.SunriseTime.ToUnixTimeSeconds().ShouldBe(1610174975);
            }
        }

        [Fact]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task Serialize_OpenW_currentWeather() {
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
            var filename = "./Resources/OpenW_currentweather.json";
            var client = new WeatherServiceBase_Wrapper(_openWeatherApiKey);

            MultiWeatherApi.OpenWeather.Model.WeatherConditions input = null;
            using (var jsonStream = File.OpenRead(filename)) {
                input = client.ParseJsonFromStream_Wrapper<MultiWeatherApi.OpenWeather.Model.WeatherConditions>(jsonStream);
            }

            string outputJson = null;
            using (var sw = new StringWriter())
            using (var jw = new JsonTextWriter(sw)) {
                var serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
                //serializer.Formatting = Formatting.Indented;
                serializer.Serialize(jw, input);
                outputJson = sw.ToString();
            }
            outputJson.ShouldNotBeNullOrWhiteSpace();
            outputJson[0].ShouldBe('{');
            outputJson[outputJson.Length - 1].ShouldBe('}');
        }

        [Fact]
        public void Serialize_OpenW_onecall() {
            var filename = "./Resources/OpenW_onecall.json";
            var client = new WeatherServiceBase_Wrapper(_openWeatherApiKey);

            ForecastDSL input = null;
            using (var jsonStream = File.OpenRead(filename)) {
                input = client.ParseJsonFromStream_Wrapper<ForecastDSL>(jsonStream, new MyAlertConverter());
            }

            string outputJson = null;
            using (var sw = new StringWriter())
            using (var jw = new JsonTextWriter(sw)) {
                var serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
                //serializer.Formatting = Formatting.Indented;
                serializer.Converters.Add(new MyAlertConverter());
                serializer.Serialize(jw, input);
                outputJson = sw.ToString();
            }
            outputJson.ShouldNotBeNullOrWhiteSpace();
            outputJson[0].ShouldBe('{');
            outputJson[outputJson.Length - 1].ShouldBe('}');
        }

        [Fact]
        public void ParseJsonFromStream_using_OpenW_onecall_json() {
            var filename = "./Resources/OpenW_onecall.json";

            var client = new WeatherServiceBase_Wrapper(_openWeatherApiKey);

            using (var jsonStream = File.OpenRead(filename)) {
                var output = client.ParseJsonFromStream_Wrapper<ForecastDSL>(jsonStream, new MyAlertConverter());
                output.Coordinates.Latitude.ShouldBe(44.47);
                output.Coordinates.Longitude.ShouldBe(11.43);
                output.TimeZone.ShouldBe("Europe/Rome");
                output.TimeZoneOffset.ShouldBe(3600);

                output.Currently.ShouldNotBeNull();
                output.Currently.Time.ToUnixTimeSeconds().ShouldBe(1609711216L);
                output.Currently.Wind.Speed.ShouldBe(1.5f);
                output.Currently.Wind.Bearing.ShouldBe(260);
                output.Currently.Visibility.ShouldBe(10000);
                output.Currently.Rain.ShouldBe(0.24f);
                output.Currently.Snow.ShouldBe(1.01f);
                output.Currently.ProbOfPrecipitation.ShouldBeNull();
                output.Currently.SunriseTime.Value.ToUnixTimeSeconds().ShouldBe(1609656638);
                output.Currently.SunsetTime.Value.ToUnixTimeSeconds().ShouldBe(1609688798);
                //output.Current.SunriseTime.ShouldBe(((int)1609656638).ToDateTimeOffset());
                output.Currently.SunriseTime.ShouldBe(new DateTime(2021, 01, 03, 6, 50, 38, DateTimeKind.Utc)); ;
                output.Currently.SunsetTime.Value.ShouldBeGreaterThan(output.Currently.SunriseTime.Value);
                output.Currently.Temperature.Daily.ShouldBe(4.14f);
                output.Currently.Temperature.DewPoint.ShouldBe(4.11f);
                output.Currently.ApparentTemperature.Daily.ShouldBe(1.8f);
                output.Currently.Temperature.Humidity.ShouldBe(100);
                output.Currently.Temperature.Pressure.ShouldBe(1010);

                output.Hourly.Count.ShouldBe(48);
                output.Hourly[0].Visibility.ShouldBe(10000);
                output.Hourly[0].Rain.ShouldBe(0.24f);
                output.Hourly[0].ProbOfPrecipitation.ShouldBe(0.3f);
                output.Hourly[0].Temperature.Daily.ShouldBe(4.14f);
                output.Hourly[0].Temperature.DewPoint.ShouldBe(4.11f);
                output.Hourly[0].Temperature.Min.ShouldBeNull();
                output.Hourly[0].Temperature.Max.ShouldBeNull();
                output.Hourly[0].Temperature.Night.ShouldBeNull();
                output.Hourly[0].Temperature.Morning.ShouldBeNull();
                output.Hourly[0].Temperature.Humidity.ShouldBe(100);
                output.Hourly[0].Temperature.Pressure.ShouldBe(1010);
                output.Hourly[0].ApparentTemperature.Daily.ShouldBe(1.7f);
                output.Hourly[0].ApparentTemperature.Min.ShouldBeNull();
                output.Hourly[0].ApparentTemperature.Max.ShouldBeNull();
                output.Hourly[0].ApparentTemperature.Night.ShouldBeNull();
                output.Hourly[0].ApparentTemperature.Morning.ShouldBeNull();
                output.Hourly[0].Temperature.Humidity.ShouldBe(100);

                output.Daily.Count.ShouldBe(8);
                output.Daily[0].Visibility.ShouldBeNull();
                output.Daily[0].Rain.ShouldBe(0.58f);
                output.Daily[0].Snow.ShouldBe(0.99f);
                output.Daily[0].ProbOfPrecipitation.ShouldBe(0.6f);
                output.Daily[0].UVIndex.ShouldBe(0.94f);
                output.Daily[0].Temperature.Daily.ShouldBe(9.5f);
                output.Daily[0].Temperature.Night.ShouldBe(4.16f);
                output.Daily[0].Temperature.Evening.ShouldBe(6.87f);
                output.Daily[0].Temperature.Morning.ShouldBe(5.93f);
                output.Daily[0].Temperature.Min.ShouldBe(4.14f);
                output.Daily[0].Temperature.Max.ShouldBe(9.5f);
                output.Daily[0].Temperature.Humidity.ShouldBe(74);
                output.Daily[0].Temperature.Pressure.ShouldBe(1009);
                output.Daily[0].Temperature.DewPoint.ShouldBe(4.83f);
                output.Daily[0].ApparentTemperature.Daily.ShouldBe(7.3f);
                output.Daily[0].ApparentTemperature.Night.ShouldBe(1.7f);
                output.Daily[0].ApparentTemperature.Evening.ShouldBe(5.18f);
                output.Daily[0].ApparentTemperature.Morning.ShouldBe(2.8f);
                output.Daily[0].ApparentTemperature.Min.ShouldBeNull();
                output.Daily[0].ApparentTemperature.Max.ShouldBeNull();

                output.Alerts.ShouldNotBeNull();
                output.Alerts.Count.ShouldBe(2);
                output.Alerts[0].Sender.ShouldBe("NWS Tulsa (Eastern Oklahoma)");
                output.Alerts[0].Title.ShouldBe("Heat Advisory");
                output.Alerts[0].StartTime.ToUnixTimeSeconds().ShouldBe(1597341600);
                output.Alerts[0].ExpiresTime.ToUnixTimeSeconds().ShouldBe(1597366800);
                output.Alerts[0].Description.ShouldStartWith("...HEAT ADVISORY REMAINS IN EFFECT");
                output.Alerts[0].Severity.ShouldBe(Severity.Unknown);

                output.Alerts[1].Description.ShouldBe("...Test\nWeather\nCondition.");
            }

        }

    }

}