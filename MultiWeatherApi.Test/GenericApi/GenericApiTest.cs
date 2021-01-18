using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MultiWeatherApi;
using MultiWeatherApi.DarkSky;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather;
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


        private string _openWeatherApiKey;
        private string _darkSkyApiKey;
        private WeatherFactory _factory;

        /// <summary>
        ///     Sets up all tests by retrieving the API key from cfg file.
        /// </summary>
        public GenericApiTest() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("./xunit.config.json")
                .Build();
            _darkSkyApiKey = config["DarkSkyApiKey"];
            _openWeatherApiKey = config["OpenWeatherApiKey"];

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
            var dsClient = _factory.Create(WeatherFactory.DarkSkyServiceId, _darkSkyApiKey);
            var output = await dsClient.GetCurrentWeather(BolognaLatitude, BolognaLongitude, Unit.SI, Language.Italian);
            // asserts
            Check_CurrentWeather_Output(output, DateTime.UtcNow, 1.0f);
        }

        [Fact]
        public async void GetCurrentWeather_OpenW() {
            //var owClient = _factory.Create(WeatherFactory.OpenWeatherServiceId, _openWeatherApiKey);
            var owClient = _factory.Create(WeatherFactory.OpenWeatherServiceId, _openWeatherApiKey);
            var output = await owClient.GetCurrentWeather(BolognaLatitude, BolognaLongitude, Unit.SI, Language.Italian);
            // asserts
            Check_CurrentWeather_Output(output, DateTime.UtcNow, 1.0f);
        }

        [Fact]
        public async void GetForecast_DarkSky() {
            var dsClient = _factory.Create(WeatherFactory.DarkSkyServiceId, _darkSkyApiKey);
            var output = await dsClient.GetForecast(BolognaLatitude, BolognaLongitude, Unit.SI, Language.Italian);
            // asserts
            output.TimeZone.ShouldNotBeNullOrEmpty();
            output.TimeZoneOffset.ShouldBe(1.0f);
            output.Coordinates.ShouldNotBeNull();
            output.Coordinates.Latitude.ShouldNotBe(0.0f);
            output.Coordinates.Longitude.ShouldNotBe(0.0f);

            output.Count.ShouldBeGreaterThan(0);
            var today = DateTime.UtcNow;
            for (int i = 0; i < output.Count; i++) {
                Check_Forecast_Output(output[i], today.AddDays(i), 1.0f);
            }
        }

        [Fact]
        public async void GetForecast_OpenW() {
            var owClient = _factory.Create(WeatherFactory.OpenWeatherServiceId, _openWeatherApiKey);
            var output = await owClient.GetForecast(BolognaLatitude, BolognaLongitude, Unit.Imperial, Language.Italian);
            // asserts
            output.TimeZone.ShouldNotBeNullOrEmpty();
            output.TimeZoneOffset.ShouldBe(1.0f);
            output.Coordinates.ShouldNotBeNull();
            output.Coordinates.Latitude.ShouldNotBe(0.0f);
            output.Coordinates.Longitude.ShouldNotBe(0.0f);

            output.Count.ShouldBeGreaterThan(0);
            var today = DateTime.UtcNow;
            for (int i = 0; i < output.Count; i++) {
                Check_Forecast_Output(output[i], today.AddDays(i), 1.0f);
            }
        }

        [Fact]
        public async void GetWeatherByDate_DarkSky() {
            var dsClient = _factory.Create(WeatherFactory.DarkSkyServiceId, _darkSkyApiKey);

            // prepare for tomorrow
            var theDate = DateTime.UtcNow.AddDays(+1);
            var output = await dsClient.GetWeatherByDate(LondonLatitude, LondonLongitude, theDate, Unit.SI, Language.Italian);
            // asserts
            Check_Forecast_Output(output, theDate, 0.0f);

            // prepare for the next 4th day
            theDate = DateTime.UtcNow.AddDays(+4).Date;
            output = await dsClient.GetWeatherByDate(LondonLatitude, LondonLongitude, theDate, Unit.SI, Language.Italian);
            // asserts
            Check_Forecast_Output(output, theDate, 0.0f);

            // prepare for the next 20th day
            theDate = DateTime.UtcNow.AddDays(+30).Date;
            output = await dsClient.GetWeatherByDate(BolognaLatitude, BolognaLongitude, theDate, Unit.SI, Language.Italian);
            // asserts
            output.ShouldNotBeNull();
            //(output.Alerts?.Count ?? 0).ShouldBeGreaterThan(0);
            //output.Alerts[0].Title.ShouldNotBeNullOrEmpty();
            output.TimeZone.ShouldNotBeNullOrEmpty();
            output.TimeZoneOffset.ShouldBe(1.0f);
            output.Summary.ShouldBeNullOrEmpty();
            output.Description.ShouldBeNullOrEmpty();
        }

        [Fact]
        public async void GetWeatherByDate_OpenW() {
            var owClient = _factory.Create(WeatherFactory.OpenWeatherServiceId, _openWeatherApiKey);

            // prepare for tomorrow
            var theDate = DateTime.UtcNow.AddDays(+1);
            var output = await owClient.GetWeatherByDate(LondonLatitude, LondonLongitude, theDate, Unit.SI, Language.Italian);
            // asserts
            Check_Forecast_Output(output, theDate, 0.0f);

            // prepare for the next 4th day
            theDate = DateTime.UtcNow.AddDays(+4).Date;
            output = await owClient.GetWeatherByDate(LondonLatitude, LondonLongitude, theDate, Unit.SI, Language.Italian);
            // asserts
            Check_Forecast_Output(output, theDate, 0.0f);

            // prepare for the next 20th day
            theDate = DateTime.UtcNow.AddDays(+30).Date;
            output = await owClient.GetWeatherByDate(BolognaLatitude, BolognaLongitude, theDate, Unit.SI, Language.Italian);
            // asserts
            output.ShouldNotBeNull();
            (output.Alerts?.Count ?? 0).ShouldBeGreaterThan(0);
            output.Alerts[0].Title.ShouldNotBeNullOrEmpty();
            output.TimeZone.ShouldNotBeNullOrEmpty();
            output.TimeZoneOffset.ShouldBe(1.0f);
            output.Summary.ShouldBeNullOrEmpty();
            output.Description.ShouldBeNullOrEmpty();
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