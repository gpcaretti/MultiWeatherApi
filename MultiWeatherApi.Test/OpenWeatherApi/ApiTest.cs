using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather;
using MultiWeatherApi.OpenWeather.Model;
using Shouldly;
using Xunit;

namespace OpenWeatherApi.Test {

    /// <summary>
    /// Tests for the main ForecastApi class.
    /// </summary>
    public class ApiTests {
        // These coordinates came from the Forecast API documentation, and should return forecasts with all blocks.
        private const double AlcatrazLatitude = 37.8267;
        private const double AlcatrazLongitude = -122.423;

        private const double BolognaLatitude = 44.47432;
        private const double BolognaLongitude = 11.430134;
        private const string BolognaCityName = "Bologna";

        /// <summary>
        ///     API key to be used for testing. This should be specified in the test project's app.config file.
        /// </summary>
        private string _apiKey;

        /// <summary>
        ///     Sets up all tests by retrieving the API key from app.config.
        /// </summary>
        public ApiTests() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("xunit.config.json")
                .Build();
            _apiKey = config["OpenWeatherApiKey"];
        }

        /// <summary>
        ///     Checks that attempting to retrieve data with a null API key throws an exception.
        /// </summary>
        [Fact]
        public void NullKeyThrowsException() {
            var client = new OpenWeatherService(null);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetForecast(AlcatrazLatitude, AlcatrazLongitude));
            //Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, DateTime.Today.AddDays(+1)));
        }

        /// <summary>
        ///     Checks that attempting to retrieve data with an empty string as the API key throws an exception.
        /// </summary>
        [Fact]
        public void EmptyKeyThrowsException() {
            var client = new OpenWeatherService("");
            Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetForecast(AlcatrazLatitude, AlcatrazLongitude));
            //Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, DateTime.Today.AddDays(+1)));
        }

        [Fact()]
        public async Task GetCurrentWeatherByCityName_Test() {
            // prepare
            var client = new OpenWeatherService(_apiKey);
            var output = await client.GetCurrentWeather(BolognaCityName, OWUnit.Metric, Language.Italian);

            // assert
            output.CityName.ShouldNotBeNullOrWhiteSpace();
            output.CityName.ShouldBe(BolognaCityName, StringCompareShould.IgnoreCase);
            output.Coordinates.Latitude.ShouldBe(Math.Round(BolognaLatitude, 2));
            output.Coordinates.Longitude.ShouldBe(Math.Round(BolognaLongitude, 2));
            output.Time.LocalDateTime.Date.ShouldBe(DateTime.Today);
            output.Main.ShouldNotBeNull();
            output.Temperature.ShouldBeGreaterThan(double.MinValue);
            output.ApparentTemperature.ShouldBeGreaterThan(double.MinValue);
            output.Weather.ShouldNotBeNull();
            output.Weather.Count.ShouldBeGreaterThan(0);
            output.Weather[0].Icon.ShouldNotBeNullOrWhiteSpace();
            output.Weather[0].IconUrl.ShouldStartWith("https://");
            output.Weather[0].IconUrl.ShouldContain(output.Weather[0].Icon);
            output.System.ShouldNotBeNull();

            // prepare imperial
            var outputImperial = await client.GetCurrentWeather(BolognaCityName, OWUnit.Imperial, Language.English);
            // assert
            outputImperial.Id.ShouldBe(output.Id);
            outputImperial.CityName.ShouldBe(output.CityName);
            outputImperial.Coordinates.Latitude.ShouldBe(output.Coordinates.Latitude);
            outputImperial.Coordinates.Longitude.ShouldBe(output.Coordinates.Longitude);
            outputImperial.WindSpeed.ShouldNotBe(output.WindSpeed);
            outputImperial.WindDegree.ShouldBe(output.WindDegree);
            outputImperial.Main.Temperature.ShouldBeGreaterThan(output.Main.Temperature);
            outputImperial.Main.ApparentTemperature.ShouldBeGreaterThan(output.Main.ApparentTemperature);
        }


    }
}