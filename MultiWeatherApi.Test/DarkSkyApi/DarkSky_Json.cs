using System.IO;
using System.Threading.Tasks;
using Helpers;
using Microsoft.Extensions.Configuration;
using MultiWeatherApi.DarkSky.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace DarkSky.Test {

    /// <summary>
    ///     Tests of serialize/deserialize to/from json files
    /// </summary>
    public class DarkSky_Json {
        // These coordinates came from the Forecast API documentation, and should return forecasts with all blocks.
        private const double BolognaLatitude = 44.482732;
        private const double BolognaLongitude = 11.352134;
        private const string BolognaCityName = "Bologna";

        private string _openWeatherApiKey;
        private string _darkSkyApiKey;

        /// <summary>
        ///     Sets up all tests by retrieving the API key from cfg file.
        /// </summary>
        public DarkSky_Json() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("./xunit.config.json")
                .Build();
            _darkSkyApiKey = config["DarkSkyApiKey"];
            _openWeatherApiKey = config["OpenWeatherApiKey"];
        }

        [Fact]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task ParseJsonFromStream_using_onecall_json() {
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
            var filename = "./Resources/DarkSky_onecall.json";
            var client = new WeatherServiceBase_Wrapper(_darkSkyApiKey);

            using (var jsonStream = File.OpenRead(filename)) {
                Forecast output = client.ParseJsonFromStream_Wrapper<Forecast>(jsonStream);
                output.Coordinates.Latitude.ShouldBe(44.482732);
                output.Coordinates.Longitude.ShouldBe(11.352134);
                output.TimeZone.ShouldBe("Europe/Rome");
                output.TimeZoneOffset.ShouldBe(1.0f);
                output.Currently.ShouldNotBeNull();
                output.Currently.Time.ToUnixTimeSeconds().ShouldBe(1609611060);
                output.Currently.Wind.Speed.ShouldBe(3.27f);
                output.Currently.Wind.Bearing.ShouldBe(96);
                output.Currently.Visibility.ShouldBe(16.093f);
                //output.Currently.PrecipitationIntensity.ShouldBe(0.24f);
                //output.Currently.SunriseTime.Value.ToUnixTimeSeconds().ShouldBe(1609656638);
                //output.Currently.SunsetTime.Value.ToUnixTimeSeconds().ShouldBe(1609688798);
                //output.Current.SunriseTime.ShouldBe(((int)1609656638).ToDateTimeOffset());
                //output.Current.SunriseTime.ShouldBe(new DateTime(2021, 01, 03, 6, 50, 38, DateTimeKind.Utc)); ;
                //output.Current.SunsetTime.Value.ShouldBeGreaterThan(output.Current.SunriseTime.Value);
                //output.Current.Temperature.Daily.ShouldBe(4.14f);
                //output.Current.Temperature.DewPoint.ShouldBe(4.11f);
                //output.Current.ApparentTemperature.Daily.ShouldBe(1.8f);
                //output.Current.Temperature.Humidity.ShouldBe(100);
                //output.Current.Temperature.Pressure.ShouldBe(1010);

                //output.Hourly.Count.ShouldBe(48);
                //output.Hourly[0].Visibility.ShouldBe(10000);
                //output.Hourly[0].Rain.ShouldBe(0.24f);
                //output.Hourly[0].ProbOfPrecipitation.ShouldBe(0.3f);
                //output.Hourly[0].Temperature.Daily.ShouldBe(4.14f);
                //output.Hourly[0].Temperature.DewPoint.ShouldBe(4.11f);
                //output.Hourly[0].Temperature.Min.ShouldBeNull();
                //output.Hourly[0].Temperature.Max.ShouldBeNull();
                //output.Hourly[0].Temperature.Night.ShouldBeNull();
                //output.Hourly[0].Temperature.Morning.ShouldBeNull();
                //output.Hourly[0].Temperature.Humidity.ShouldBe(100);
                //output.Hourly[0].Temperature.Pressure.ShouldBe(1010);
                //output.Hourly[0].ApparentTemperature.Daily.ShouldBe(1.7f);
                //output.Hourly[0].ApparentTemperature.Min.ShouldBeNull();
                //output.Hourly[0].ApparentTemperature.Max.ShouldBeNull();
                //output.Hourly[0].ApparentTemperature.Night.ShouldBeNull();
                //output.Hourly[0].ApparentTemperature.Morning.ShouldBeNull();
                //output.Hourly[0].Temperature.Humidity.ShouldBe(100);

                //output.Daily.Count.ShouldBe(8);
                //output.Daily[0].Visibility.ShouldBeNull();
                //output.Daily[0].Rain.ShouldBe(0.58f);
                //output.Daily[0].ProbOfPrecipitation.ShouldBe(0.6f);
                //output.Daily[0].UVIndex.ShouldBe(0.94f);
                //output.Daily[0].Temperature.Daily.ShouldBe(9.5f);
                //output.Daily[0].Temperature.Night.ShouldBe(4.16f);
                //output.Daily[0].Temperature.Evening.ShouldBe(6.87f);
                //output.Daily[0].Temperature.Morning.ShouldBe(5.93f);
                //output.Daily[0].Temperature.Min.ShouldBe(4.14f);
                //output.Daily[0].Temperature.Max.ShouldBe(9.5f);
                //output.Daily[0].Temperature.Humidity.ShouldBe(74);
                //output.Daily[0].Temperature.Pressure.ShouldBe(1009);
                //output.Daily[0].Temperature.DewPoint.ShouldBe(4.83f);
                //output.Daily[0].ApparentTemperature.Daily.ShouldBe(7.3f);
                //output.Daily[0].ApparentTemperature.Night.ShouldBe(1.7f);
                //output.Daily[0].ApparentTemperature.Evening.ShouldBe(5.18f);
                //output.Daily[0].ApparentTemperature.Morning.ShouldBe(2.8f);
                //output.Daily[0].ApparentTemperature.Min.ShouldBeNull();
                //output.Daily[0].ApparentTemperature.Max.ShouldBeNull();

                //output.Alerts.ShouldNotBeNull();
                //output.Alerts.Count.ShouldBe(2);
                //output.Alerts[0].Severity.ShouldBe(Severity.Unknown);
                //output.Alerts[0].Sender.ShouldNotBeNullOrEmpty();
                //output.Alerts[0].Title.ShouldNotBeNullOrEmpty();
            }
        }

        [Fact]
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task Serialize_onecall() {
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
            var filename = "./Resources/DarkSky_onecall.json";
            var client = new WeatherServiceBase_Wrapper(_darkSkyApiKey);

            Forecast input = null;
            using (var jsonStream = File.OpenRead(filename)) {
                input = client.ParseJsonFromStream_Wrapper<Forecast>(jsonStream);
            }

            string outputJson = null;
            using (var sw = new StringWriter())
            using (var jw = new JsonTextWriter(sw)) {
                var serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
                //serializer.Formatting = Formatting.Indented;
                //serializer.Converters.Add(new MyAlertConverter());
                serializer.Serialize(jw, input);
                outputJson = sw.ToString();
            }
            outputJson.ShouldNotBeNullOrWhiteSpace();
            outputJson[0].ShouldBe('{');
            outputJson[outputJson.Length - 1].ShouldBe('}');
        }

    }

}