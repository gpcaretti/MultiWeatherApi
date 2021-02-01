using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Helpers;
using Microsoft.Extensions.Configuration;
using MultiWeatherApi;
using MultiWeatherApi.DarkSky;
using MultiWeatherApi.DarkSky.Model;
using MultiWeatherApi.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace DarkSky.Test {

    /// <summary>
    ///     Tests for the main ForecastApi class.
    /// </summary>
    public class DarkSky_Api {
        // These coordinates came from the Forecast API documentation, and should return forecasts with all blocks.
        private const double AlcatrazLatitude = 37.8267;
        private const double AlcatrazLongitude = -122.423;

        private const double MumbaiLatitude = 18.975;
        private const double MumbaiLongitude = 72.825833;

        private const double BolognaLatitude = 44.482732;
        private const double BolognaLongitude = 11.352134;

        /// <summary>
        ///     Sets up all tests by retrieving the API key from app.config.
        /// </summary>
        public DarkSky_Api() {
            //var config = new ConfigurationBuilder()
            //    .AddJsonFile("xunit.config.json")
            //    .Build();
            //_apiKey = config["DarkSkyApiKey"];
        }

        /// <summary>
        ///     Checks that attempting to retrieve data with a null API key throws an exception.
        /// </summary>
        [Fact]
        public void InvalidKeyThrowsException() {
            // prepare
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(DarkSkyService.EndPointRoot + "*")
                .Respond(HttpStatusCode.Unauthorized);
            // assert
            var client = new DarkSkyService(null, mockHttp);
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetForecast(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, DateTime.Today.AddDays(+1)));

            client = new DarkSkyService(string.Empty, mockHttp);
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetForecast(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, DateTime.Today.AddDays(+1)));

            client = new DarkSkyService("fake_key", mockHttp);
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, DateTime.Today.AddDays(+1)));
        }

        /// <summary>
        ///     Checks that using a valid API key will allow requests to be made.
        ///     <para>An API key can be specified in the project's app.config file.</para>
        /// </summary>
        [Fact]
        public async void GetCurrentWeatherByCoordinates_Test() {
            // prepare
            Forecast output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/DarkSky_GetCurrentWeather_US.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(DarkSkyService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                IDarkSkyService client = new DarkSkyService("a_valid_key", mockHttp);

                output = await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude, DSUnit.US, Language.English);
                stream.Close();
            }

            // assert
            Assert.NotNull(output);
            Assert.NotNull(output.Currently);
            Assert.NotNull(output.Hourly);
            Assert.NotNull(output.Daily);
            output.Coordinates.ShouldNotBeNull();
            output.Coordinates.Latitude.ShouldBe(37.8267);
            output.Coordinates.Longitude.ShouldBe(-122.423);
            output.TimeZone.ShouldBe("America/Los_Angeles");
            output.TimeZoneOffset.ShouldBe(-8.0f);

            output.Alerts.ShouldNotBeNull();
            output.Alerts.Count.ShouldBe(2);
            output.Alerts[0].Title.ShouldBe("High Wind Warning");
            output.Alerts[0].Description.ShouldNotBeNullOrEmpty();
            output.Alerts[0].ExpiresTime.ShouldBe(1611759600.ToDateTimeOffset());
            output.Alerts[0].Uri.ShouldStartWith("https://alerts.weather.gov/");
            output.Alerts[1].Title.ShouldBe("Wind Advisory");

            output.Currently.ShouldNotBeNull();
            output.Currently.UnixTime.ShouldBe(1611642210);
            output.Currently.Time.ToUnixTimeSeconds().ShouldBe(output.Currently.UnixTime);

            output.Currently.Wind.Speed.ShouldBe(9.23f);
            output.Currently.Wind.Bearing.ShouldBe(308);
            output.Currently.Visibility.ShouldBe(10f);

            output.Hourly.Data.Count.ShouldBe(49);
            output.Hourly.Data[0].ApparentTemperature.Daily.ShouldBe(40.9f);
            output.Daily.Data.Count.ShouldBe(8);
            output.Daily.Data[1].Temperature.Max.ShouldBe(49.4f);
        }

        /// <summary>
        ///     Checks that specifying multiple blocks to be excluded causes them to left out.
        /// </summary>
        [Fact]
        public async void CurrentAnd7DaysForecastWorksCorrectly() {
            // prepare
            Forecast output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/DarkSky_CurrentAnd7DaysForecast_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(DarkSkyService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                IDarkSkyService client = new DarkSkyService("a_valid_key", mockHttp);

                var exclusionList = new List<Exclude> { Exclude.Hourly, Exclude.Minutely, Exclude.Alerts, Exclude.Flags };
                output = await client.GetWeather(BolognaLatitude, BolognaLongitude, null, exclusionList, DSUnit.Auto, Language.Italian);
                stream.Close();
            }

            // assert
            Assert.NotNull(output);
            Assert.NotNull(output.Currently);
            Assert.Null(output.Minutely);
            Assert.Null(output.Hourly);
            Assert.NotNull(output.Currently);
            Assert.NotNull(output.Daily);
            Assert.NotNull(output.Daily.Data);
            Assert.NotEmpty(output.Daily.Data);
        }

        ///// <summary>
        ///// Checks that retrieving data for a past date works correctly.
        ///// </summary>
        //[Fact]
        //public async void CanRetrieveForThePast() {
        //    var client = new DarkSkyService("a_valid_key");
        //    var date = DateTime.UtcNow.AddDays(-2);

        //    var result = await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, date);

        //    Assert.NotNull(result);
        //    Assert.NotNull(result.Currently);
        //}

        [Fact]
        public async void GetForecast_ByCoordinates_Test() {
            // prepare
            Forecast output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/DarkSky_GetForecast_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(DarkSkyService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                IDarkSkyService client = new DarkSkyService("a_valid_key", mockHttp);

                output = await client.GetForecast(BolognaLatitude, BolognaLongitude, DSUnit.SI, Language.Italian);
                stream.Close();
            }

            // assert
            Assert.NotNull(output);
            Assert.NotNull(output.Currently);
            Assert.Null(output.Minutely);
            Assert.NotNull(output.Hourly);
            Assert.NotNull(output.Hourly.Data);
            Assert.NotEmpty(output.Hourly.Data);
            Assert.NotNull(output.Daily);
            Assert.NotNull(output.Daily.Data);
            Assert.NotEmpty(output.Daily.Data);
        }

        /// <summary>
        /// Checks that specifying a block to be excluded from forecasts for past dates
        /// will cause it to be null in the returned forecast.
        /// </summary>
        [Fact]
        public async void TimeMachineExclusionWorksCorrectly() {
            // prepare
            var date = new DateTime(2021, 01, 24, 10, 00, 01, DateTimeKind.Utc);
            Forecast output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/DarkSky_GetWeatherByDate_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(DarkSkyService.EndPointRoot + "*")
                    .Respond("application/json", stream);

                var exclusionList = new List<Exclude> { Exclude.Minutely, Exclude.Hourly };
                IDarkSkyService client = new DarkSkyService("a_valid_key", mockHttp);
                output = await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, date, exclusionList, DSUnit.SI);
                stream.Close();
            }

            Assert.NotNull(output);
            Assert.Null(output.Hourly);
            Assert.NotNull(output.Currently);
            Assert.NotNull(output.Daily);
            Assert.NotNull(output.Daily.Data);
            output.Daily.Data.Count.ShouldBe(1);
            output.Daily.Data[0].Time.Date.ShouldBe(date.Date);
        }

        [Fact]
        public void Serialize_onecall() {
            var filename = "./Resources/DarkSky_onecall_SI.json";
            var client = new WeatherServiceBase_Wrapper();

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
            outputJson.Length.ShouldBeGreaterThan(32);
            outputJson[0].ShouldBe('{');
            outputJson[outputJson.Length - 1].ShouldBe('}');
            outputJson[outputJson.Length - 1].ShouldBe('}');
            outputJson.ShouldContain("\"lat\":");
            outputJson.ShouldContain("\"currently\":");
        }
    }
}