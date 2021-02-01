using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MultiWeatherApi;
using MultiWeatherApi.DarkSky;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather;
using RichardSzalay.MockHttp;
using Shouldly;
using Xunit;

namespace GenericApi.Test {

    /// <summary>
    ///     Tests for the main ForecastApi class.
    /// </summary>
    public class GenericApiTest {
        // These coordinates came from the Forecast API documentation, and should return forecasts with all blocks.
        private const double AlcatrazLatitude = 37.8267;
        private const double AlcatrazLongitude = -122.423;

        private const double LondonLatitude = 51.5085300;
        private const double LondonLongitude = -0.1257400;

        private const double BolognaLatitude = 44.482732;
        private const double BolognaLongitude = 11.352134;
        private const string BolognaCityName = "Bologna";

        private WeatherFactory _factory;

        /// <summary>
        ///     Sets up the tests
        /// </summary>
        public GenericApiTest() {
            //var config = new ConfigurationBuilder()
            //    .AddJsonFile("./xunit.config.json")
            //    .Build();
            //"a_valid_key" = config["DarkSkyApiKey"];
            //"a_valid_key" = config["OpenWeatherApiKey"];

            _factory = new WeatherFactory();
        }

        /// <summary>
        ///     Checks that attempting to retrieve data with a null API key throws an exception.
        /// </summary>
        [Fact]
        public void NullKeyThrowsException() {
            var dsClient = _factory.Create(WeatherFactory.DarkSkyServiceId, null);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await dsClient.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await dsClient.GetForecast(AlcatrazLatitude, AlcatrazLongitude));
            Should.ThrowAsync<InvalidOperationException>(async () => await dsClient.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, DateTime.Today.AddDays(+1)));

            var owClient = _factory.Create(WeatherFactory.OpenWeatherServiceId, null);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await owClient.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await owClient.GetForecast(AlcatrazLatitude, AlcatrazLongitude));
            Should.ThrowAsync<InvalidOperationException>(async () => await owClient.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, DateTime.Today.AddDays(+1)));
        }

        [Fact]
        public async void GetCurrentWeather_DarkSky() {
            // prepare
            Weather output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/DarkSky_GetCurrentWeather_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(DarkSkyService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                // execute
                output = await _factory.Create(WeatherFactory.DarkSkyServiceId, "a_valid_key", mockHttp)
                                    .GetCurrentWeather(BolognaLatitude, BolognaLongitude, Unit.SI, Language.Italian);
                stream.Close();
            }

            // asserts
            Check_CurrentWeather_Output(output, 1611642338.ToDateTimeOffset().UtcDateTime, 1.0f);
        }

        [Fact]
        public async void GetCurrentWeather_OpenW() {
            // prepare
            Weather output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/OpenW_onecall_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(OpenWeatherService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                // execute
                output = await _factory.Create(WeatherFactory.OpenWeatherServiceId, "a_valid_key", mockHttp)
                                    .GetCurrentWeather(BolognaLatitude, BolognaLongitude, Unit.SI, Language.Italian);
                stream.Close();
            }

            // asserts
            Check_CurrentWeather_Output(output, 1609711216.ToDateTimeOffset().UtcDateTime, 1.0f);
        }

        [Fact]
        public async void GetForecast_DarkSky() {
            // prepare
            WeatherGroup output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/DarkSky_GetForecast_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(DarkSkyService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                // execute
                output = await _factory.Create(WeatherFactory.DarkSkyServiceId, "a_valid_key", mockHttp)
                                    .GetForecast(BolognaLatitude, BolognaLongitude, Unit.SI, Language.Italian);
                stream.Close();
            }

            // asserts
            output.TimeZone.ShouldNotBeNullOrEmpty();
            output.TimeZoneOffset.ShouldBe(1.0f);
            output.Coordinates.ShouldNotBeNull();
            output.Coordinates.Latitude.ShouldNotBe(0.0f);
            output.Coordinates.Longitude.ShouldNotBe(0.0f);

            output.Count.ShouldBeGreaterThan(0);
            var today = 1611648875.ToDateTimeOffset().UtcDateTime;
            for (int i = 0; i < output.Count; i++) {
                Check_Forecast_Output(output[i], today.AddDays(i), 1.0f);
            }
        }

        [Fact]
        public async void GetForecast_OpenW() {
            // prepare
            WeatherGroup output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/OpenW_onecall_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(OpenWeatherService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                // execute
                output = await _factory.Create(WeatherFactory.OpenWeatherServiceId, "a_valid_key", mockHttp)
                                    .GetForecast(BolognaLatitude, BolognaLongitude, Unit.Imperial, Language.Italian);
                stream.Close();
            }

            // asserts
            output.TimeZone.ShouldNotBeNullOrEmpty();
            output.TimeZoneOffset.ShouldBe(1.0f);
            output.Coordinates.ShouldNotBeNull();
            output.Coordinates.Latitude.ShouldNotBe(0.0f);
            output.Coordinates.Longitude.ShouldNotBe(0.0f);

            output.Count.ShouldBeGreaterThan(0);
            var today = 1609711216.ToDateTimeOffset().UtcDateTime;
            for (int i = 0; i < output.Count; i++) {
                Check_Forecast_Output(output[i], today.AddDays(i), 1.0f);
            }
        }

        [Fact]
        public async void GetWeatherByDate_DarkSky() {
            // prepare
            DateTime theDate = 1611477054.ToDateTimeOffset().Date;
            Weather output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/DarkSky_GetWeatherByDate_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(DarkSkyService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                // execute
                output = await _factory.Create(WeatherFactory.DarkSkyServiceId, "a_valid_key", mockHttp)
                                    .GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, theDate, Unit.SI, Language.Italian);
                stream.Close();
            }

            // asserts
            Check_Forecast_Output(output, theDate, -8.0f);
        }

        [Fact]
        public async void GetWeatherByDate_OpenW() {
            // prepare
            var theDate = 1610276400.ToDateTimeOffset().UtcDateTime;
            Weather output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/OpenW_onecall_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(OpenWeatherService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                // execute
                output = await _factory.Create(WeatherFactory.OpenWeatherServiceId, "a_valid_key", mockHttp)
                                    .GetWeatherByDate(LondonLatitude, LondonLongitude, theDate, Unit.SI, Language.Italian);
                stream.Close();
            }

            // asserts
            Check_Forecast_Output(output, theDate, 1.0f);
        }

        private void Check_CurrentWeather_Output(Weather output, DateTime utcDay, float timeZoneOffset) {
            output.ShouldNotBeNull();
            output.TimeZone.ShouldNotBeNullOrEmpty();
            output.TimeZoneOffset.ShouldBe(timeZoneOffset);
            output.UnixTime.ShouldBeGreaterThanOrEqualTo(utcDay.Date.ToUnixTime());

            output.Summary.ShouldNotBeNullOrEmpty();
            output.Description.ShouldNotBeNullOrEmpty();
            output.Icon.ShouldNotBeNullOrEmpty();
            // TODO output.IconUrl.ShouldNotBeNullOrEmpty();
            output.Wind.ShouldNotBeNull();
            output.Coordinates.ShouldNotBeNull();
            output.Coordinates.Latitude.ShouldNotBe(0.0);
            output.Coordinates.Longitude.ShouldNotBe(0.0);
            output.Temperature.ShouldNotBeNull();
            output.Temperature.Daily.ShouldNotBeNull();
            output.Temperature.Humidity.ShouldNotBeNull();
            output.Temperature.Pressure.ShouldNotBeNull();
            output.ApparentTemperature.ShouldNotBeNull();
            output.Visibility.ShouldNotBeNull();
            output.Visibility.Value.ShouldBeGreaterThan(0);
            output.SunriseUnixTime.ShouldNotBeNull();
            output.SunsetUnixTime.ShouldNotBeNull();
            output.SunriseUnixTime.Value.ShouldBeGreaterThan(utcDay.Date.ToUnixTime());
            output.SunsetUnixTime.Value.ShouldBeGreaterThan(output.SunriseUnixTime.Value);
            output.Hourly.ShouldNotBeEmpty();
            output.Hourly.ShouldAllBe(hr => hr.Time > utcDay.Date);
        }

        private void Check_Forecast_Output(Weather output, DateTime utcDay, float timeZoneOffset) {
            output.ShouldNotBeNull();
            output.TimeZone.ShouldNotBeNullOrEmpty();
            output.TimeZoneOffset.ShouldBe(timeZoneOffset);
            output.UnixTime.ShouldBeGreaterThanOrEqualTo(utcDay.Date.ToUnixTime());

            (!string.IsNullOrEmpty(output.Summary) || !string.IsNullOrEmpty(output.Description)).ShouldBeTrue();
            output.Icon.ShouldNotBeNullOrEmpty();
            // TODO output.IconUrl.ShouldNotBeNullOrEmpty();
            output.Wind.ShouldNotBeNull();
            output.Coordinates.ShouldNotBeNull();
            output.Coordinates.Latitude.ShouldNotBe(0.0);
            output.Coordinates.Longitude.ShouldNotBe(0.0);
            output.Temperature.ShouldNotBeNull();
            (output.Temperature.Min.HasValue || output.Temperature.Night.HasValue).ShouldBeTrue();
            (output.Temperature.Max.HasValue || output.Temperature.Daily.HasValue).ShouldBeTrue();
            //output.Temperature.DewPoint.ShouldNotBeNull();
            output.Temperature.Humidity.ShouldNotBeNull();
            output.Temperature.Pressure.ShouldNotBeNull();
            output.ApparentTemperature.ShouldNotBeNull();
            (output.ApparentTemperature.Min.HasValue || output.ApparentTemperature.Night.HasValue).ShouldBeTrue();
            (output.ApparentTemperature.Max.HasValue || output.ApparentTemperature.Daily.HasValue).ShouldBeTrue();
            //output.Visibility.ShouldBeGreaterThan(0);
            output.SunriseUnixTime.ShouldNotBeNull();
            output.SunsetUnixTime.ShouldNotBeNull();
            output.SunriseUnixTime.Value.ShouldBeGreaterThan(utcDay.Date.ToUnixTime());
            output.SunsetUnixTime.Value.ShouldBeGreaterThan(output.SunriseUnixTime.Value);
        }

    }

}