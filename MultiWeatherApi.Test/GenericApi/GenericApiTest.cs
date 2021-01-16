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

        private const double MumbaiLatitude = 18.975;
        private const double MumbaiLongitude = 72.825833;

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
            Check_GetCurrentWeather_Output(output);
        }

        [Fact]
        public async void GetCurrentWeather_OpenW() {
            //var owClient = _factory.Create(WeatherFactory.OpenWeatherServiceId, _openWeatherApiKey);
            var owClient = _factory.Create(WeatherFactory.OpenWeatherServiceId, _openWeatherApiKey);
            var output = await owClient.GetCurrentWeather(BolognaLatitude, BolognaLongitude, Unit.SI, Language.Italian);
            // asserts
            Check_GetCurrentWeather_Output(output);
        }

        [Fact]
        public async void GetForecast_DarkSky() {
            var dsClient = _factory.Create(WeatherFactory.DarkSkyServiceId, _darkSkyApiKey);
            var output = await dsClient.GetForecast(BolognaLatitude, BolognaLongitude, Unit.SI, Language.Italian);
            // asserts
            throw new NotImplementedException();
        }

        [Fact]
        public async void GetForecast_OpenW() {
            var owClient = _factory.Create(WeatherFactory.OpenWeatherServiceId, _openWeatherApiKey);
            var output = await owClient.GetForecast(BolognaLatitude, BolognaLongitude, Unit.SI, Language.Italian);
            // asserts
            throw new NotImplementedException();
        }

        private void Check_GetCurrentWeather_Output(Weather output) {
            output.TimeZone.ShouldNotBeNullOrEmpty();
            output.TimeZoneOffset.ShouldNotBe(0.0f);
            output.UnixTime.ShouldBeGreaterThan(DateTime.Today.Date.ToUnixTime());

            output.Summary.ShouldNotBeNullOrEmpty();
            output.Description.ShouldNotBeNullOrEmpty();
            output.Icon.ShouldNotBeNullOrEmpty();
            // TODO output.IconUrl.ShouldNotBeNullOrEmpty();
            output.Wind.ShouldNotBeNull();
            output.Coordinates.ShouldNotBeNull();
            output.Coordinates.Latitude.ShouldBeGreaterThan(0.0);
            output.Coordinates.Longitude.ShouldBeGreaterThan(0.0);
            output.Temperature.ShouldNotBeNull();
            output.Temperature.Daily.ShouldNotBeNull();
            output.Temperature.Humidity.ShouldNotBeNull();
            output.Temperature.Pressure.ShouldNotBeNull();
            output.ApparentTemperature.ShouldNotBeNull();
            output.Visibility.ShouldBeGreaterThan(0);
            output.SunriseUnixTime.ShouldNotBeNull();
            output.SunsetUnixTime.ShouldNotBeNull();
            output.SunriseUnixTime.Value.ShouldBeGreaterThan(DateTime.Today.Date.ToUnixTime());
            output.SunsetUnixTime.Value.ShouldBeGreaterThan(output.SunriseUnixTime.Value);
        }

    }

}