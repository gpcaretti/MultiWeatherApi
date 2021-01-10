using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather;
using MultiWeatherApi.OpenWeather.Model;
using Shouldly;
using Xunit;

namespace OpenWeather.Test {


    /// <summary>
    /// Tests for the API.
    /// </summary>
    public class OpenWeather_Api {

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
        public OpenWeather_Api() {
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

        [Fact]
        public async Task GetCurrentWeatherByCoordinates_Test() {
            // prepare
            var client = new OpenWeatherService(_apiKey);
            var output = await client.GetCurrentWeather(BolognaLatitude, BolognaLongitude, OWUnit.Metric, Language.Italian);
            // assert
            output.City.Name.ShouldNotBeNullOrWhiteSpace();
            output.City.Name.ShouldBe(BolognaCityName, StringCompareShould.IgnoreCase);
        }

        [Fact]
        public async Task GetCurrentWeatherByCityName_Test() {
            // prepare
            var client = new OpenWeatherService(_apiKey);
            var output = await client.GetCurrentWeather(BolognaCityName, OWUnit.Metric, Language.Italian);
            // assert
            output.City.Name.ShouldNotBeNullOrWhiteSpace();
            output.City.Name.ShouldBe(BolognaCityName, StringCompareShould.IgnoreCase);
            output.Coordinates.ShouldBe(output.Coordinates);
            output.City.CountryCode.ShouldNotBeNullOrWhiteSpace();
            output.City.SunriseTime.ShouldBeGreaterThan(DateTime.Today);
            output.City.SunsetTime.ShouldBeGreaterThan(output.City.SunriseTime);
            output.Coordinates.Latitude.ShouldBeGreaterThan(0.0);
            output.Coordinates.Longitude.ShouldBeGreaterThan(0.0);
            output.Temperature.Daily.ShouldNotBeNull();
            output.ApparentTemperature.Daily.ShouldNotBeNull();
            output.Temperature.Min.ShouldNotBeNull();
            output.Temperature.Min.Value.ShouldBeLessThanOrEqualTo(output.Temperature.Daily.Value);
            output.Temperature.Max.ShouldNotBeNull();
            output.Temperature.Max.Value.ShouldBeGreaterThanOrEqualTo(output.Temperature.Daily.Value);
            output.Temperature.Humidity.ShouldNotBeNull();
            output.Temperature.Humidity.Value.ShouldBeGreaterThan(0);
            output.Temperature.Pressure.ShouldNotBeNull();
            output.Temperature.Pressure.Value.ShouldBeGreaterThan(0);
            output.Time.ShouldBeGreaterThanOrEqualTo(DateTime.Today);
            output.WeatherInfo.ShouldNotBeNull();
            output.WeatherInfo.Count.ShouldBeGreaterThan(0);
            output.WeatherInfo[0].Icon.ShouldNotBeNullOrWhiteSpace();
            output.WeatherInfo[0].IconUrl.ShouldStartWith("https://");
            output.WeatherInfo[0].IconUrl.ShouldContain(output.WeatherInfo[0].Icon);
            output.Wind.Bearing.ShouldBeGreaterThanOrEqualTo(0);
            output.Wind.Speed.ShouldBeGreaterThanOrEqualTo(0.0f);

            // prepare imperial
            var outputImperial = await client.GetCurrentWeather(BolognaCityName, OWUnit.Imperial, Language.English);
            // assert
            outputImperial.City.Name.ShouldBe(output.City.Name);
            outputImperial.City.CountryCode.ShouldBe(output.City.CountryCode);
            outputImperial.Coordinates.Latitude.ShouldBe(output.Coordinates.Latitude);
            outputImperial.Coordinates.Longitude.ShouldBe(output.Coordinates.Longitude);
            outputImperial.Wind.Speed.ShouldNotBe(output.Wind.Speed);
            outputImperial.Temperature.Daily.ShouldNotBeNull();
            outputImperial.Temperature.Daily.Value.ShouldBeGreaterThan(output.Temperature.Daily.Value);
            outputImperial.ApparentTemperature.Daily.ShouldNotBeNull();
            outputImperial.ApparentTemperature.Daily.Value.ShouldBeGreaterThan(output.ApparentTemperature.Daily.Value);
        }

        [Fact]
        public async Task GetDailyForecast_ByCoordinates_Test() {
            // prepare
            IOpenWeatherService client = new OpenWeatherService(_apiKey);
            ForecastDSL output = await client.GetForecastDSL(BolognaLatitude, BolognaLongitude, OWUnit.Metric, Language.Italian);
            // assert
            output.Coordinates.Latitude.ShouldBeGreaterThan(0.0);
            output.Coordinates.Longitude.ShouldBeGreaterThan(0.0);
            output.TimeZone.ShouldNotBeNullOrWhiteSpace();
            output.TimeZoneOffset.ShouldBeGreaterThan(0);
            output.Current.ShouldNotBeNull();
            output.Current.Time.Date.ShouldBe(DateTime.Today);
            output.Current.SunriseTime.Value.Date.ShouldBe(DateTime.Today);
            output.Current.SunriseTime.Value.ShouldBeGreaterThan(DateTime.Today);
            output.Current.SunsetTime.Value.ShouldBeGreaterThan(output.Current.SunriseTime.Value);
            output.Current.Time.ShouldBeGreaterThan(DateTime.Today);
            output.Current.Temperature.Daily.ShouldNotBeNull();
            output.Current.Temperature.Daily.ShouldNotBe(0.0f);
            output.Current.Temperature.Pressure.ShouldNotBeNull();
            output.Current.Temperature.Pressure.Value.ShouldBeGreaterThan(0);
            output.Current.ApparentTemperature.Daily.ShouldNotBeNull();

            output.Daily.Count.ShouldBeGreaterThan(0);
            output.Daily[0].Time.Date.ShouldBe(DateTime.Today);
            output.Daily[0].WeatherInfo.Count.ShouldBeGreaterThan(0);
            output.Daily[0].ApparentTemperature.Daily.ShouldNotBeNull();
            output.Daily[0].ApparentTemperature.Daily.ShouldNotBeNull();
            output.Daily[1].Time.Date.ShouldBe(DateTime.Today.AddDays(+1));

            // prepare imperial
            var outputImperial = await client.GetForecastDSL(BolognaLatitude, BolognaLongitude, OWUnit.Imperial, Language.English);
            // assert
            outputImperial.Coordinates.Latitude.ShouldBe(output.Coordinates.Latitude);
            outputImperial.Coordinates.Longitude.ShouldBe(output.Coordinates.Longitude);
            outputImperial.Current.Temperature.Daily.ShouldNotBeNull();
            outputImperial.Current.Temperature.Daily.Value.ShouldBeGreaterThan(output.Current.Temperature.Daily.Value);
            outputImperial.Current.Wind.Speed.ShouldNotBe(output.Current.Wind.Speed);
            outputImperial.Current.Wind.Bearing.ShouldBe(output.Current.Wind.Bearing);
        }

    }
}