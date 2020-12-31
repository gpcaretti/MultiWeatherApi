using System;
using Microsoft.Extensions.Configuration;
using MultiWeatherApi;
using MultiWeatherApi.DarkSky;
using MultiWeatherApi.OpenWeather;
using Shouldly;
using Xunit;

namespace GenericApi.Test {

    /// <summary>
    /// Tests for the main ForecastApi class.
    /// </summary>
    public class ApiTests {
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

        /// <summary>
        ///     Sets up all tests by retrieving the API key from cfg file.
        /// </summary>
        public ApiTests() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("xunit.config.json")
                .Build();
            _darkSkyApiKey = config["DarkSkyApiKey"];
            _openWeatherApiKey = config["OpenWeatherApiKey"];
        }

        /// <summary>
        ///     Checks that attempting to retrieve data with a null API key throws an exception.
        /// </summary>
        [Fact]
        public void NullKeyThrowsException() {
            var dsClient = CreateService<DarkSkyService>(null);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await dsClient.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await dsClient.GetForecast(AlcatrazLatitude, AlcatrazLongitude));
            Should.ThrowAsync<InvalidOperationException>(async () => await dsClient.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, DateTime.Today.AddDays(+1)));

            var owClient = CreateService<OpenWeatherService>(null);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await owClient.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await owClient.GetForecast(AlcatrazLatitude, AlcatrazLongitude));
            Should.ThrowAsync<InvalidOperationException>(async () => await owClient.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, DateTime.Today.AddDays(+1)));
        }

        private IWeatherService CreateService<T>(string apiKey) where T : WeatherServiceBase {
            return new WeatherFactory().Create<T>(apiKey);
        }

    }

}