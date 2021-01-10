using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MultiWeatherApi.DarkSky;
using MultiWeatherApi.DarkSky.Model;
using MultiWeatherApi.Model;
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
        ///     API key to be used for testing. This should be specified in the test project's app.config file.
        /// </summary>
        private string _apiKey;

        /// <summary>
        ///     Sets up all tests by retrieving the API key from app.config.
        /// </summary>
        public DarkSky_Api() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("xunit.config.json")
                .Build();
            _apiKey = config["DarkSkyApiKey"];
        }

        /// <summary>
        ///     Checks that attempting to retrieve data with a null API key throws an exception.
        /// </summary>
        [Fact]
        public void NullKeyThrowsException() {
            var client = new DarkSkyService(null);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetForecast(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, DateTime.Today.AddDays(+1)));
        }

        /// <summary>
        ///     Checks that attempting to retrieve data with an empty string as the API key throws an exception.
        /// </summary>
        [Fact]
        public void EmptyKeyThrowsException() {
            var client = new DarkSkyService(string.Empty);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetForecast(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, DateTime.Today.AddDays(+1)));
        }

        /// <summary>
        ///     Checks that using a valid API key will allow requests to be made.
        ///     <para>An API key can be specified in the project's app.config file.</para>
        /// </summary>
        [Fact]
        public async Task ValidKeyRetrievesData() {
            var client = new DarkSkyService(_apiKey);

            var result = await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude);
            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
            Assert.Null(result.Daily);

            result = await client.GetForecast(AlcatrazLatitude, AlcatrazLongitude);
            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
            Assert.NotNull(result.Daily);
            Assert.NotNull(result.Daily.Data);
            result.Daily.Data.Count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        ///     Checks that a request can be made for a non-US location. Added to test GitHub issue 6.
        /// </summary>
        [Fact]
        public async Task NonUSDataCanBeRetrieved() {
            var client = new DarkSkyService(_apiKey);

            var result = await client.GetCurrentWeather(BolognaLatitude, BolognaLongitude, DSUnit.SI, Language.Italian);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        /// <summary>
        ///     Checks that requests can be made with DSUnit.SI.
        /// </summary>
        [Fact]
        public async Task UnitSIWorksCorrectly() {
            var client = new DarkSkyService(_apiKey);

            var result = await client.GetCurrentWeather(MumbaiLatitude, MumbaiLongitude, DSUnit.SI);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        /// <summary>
        ///     Checks that requests can be made with DSUnit.UK.
        /// </summary>
        [Fact]
        public async Task UnitUKWorksCorrectly() {
            var client = new DarkSkyService(_apiKey);

            var result = await client.GetCurrentWeather(MumbaiLatitude, MumbaiLongitude, DSUnit.UK);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        /// <summary>
        /// Checks that requests can be made with DSUnit.UK2.
        /// Added to test GitHub issue 18.
        /// </summary>
        [Fact]
        public async Task UnitUK2WorksCorrectly() {
            var client = new DarkSkyService(_apiKey);

            var result = await client.GetCurrentWeather(MumbaiLatitude, MumbaiLongitude, DSUnit.UK2);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        /// <summary>
        /// Checks that specifying a block to be excluded from the results
        /// will cause it to be null in the returned forecast.
        /// </summary>
        [Fact]
        public async Task ExclusionWorksCorrectly() {
            var client = new DarkSkyService(_apiKey);
            var exclusionList = new List<Exclude> { Exclude.Minutely };

            var result = await client.GetWeather(AlcatrazLatitude, AlcatrazLongitude, null, exclusionList, DSUnit.SI, Language.Italian);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
            Assert.Null(result.Minutely);
        }

        /// <summary>
        /// Checks that specifying multiple blocks to be excluded causes
        /// them to left out.
        /// </summary>
        [Fact]
        public async Task MultipleExclusionWorksCorrectly() {
            var client = new DarkSkyService(_apiKey);
            var exclusionList = new List<Exclude> { Exclude.Minutely, Exclude.Hourly, Exclude.Daily };

            var result = await client.GetWeather(AlcatrazLatitude, AlcatrazLongitude, null, exclusionList, DSUnit.SI);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
            Assert.Null(result.Minutely);
            Assert.Null(result.Hourly);
            Assert.Null(result.Daily);
        }

        /// <summary>
        ///     Checks that specifying multiple blocks to be excluded causes them to left out.
        /// </summary>
        [Fact]
        public async Task CurrentAnd7DaysForecastWorksCorrectly() {
            var client = new DarkSkyService(_apiKey);
            var exclusionList = new List<Exclude> { Exclude.Hourly, Exclude.Minutely, Exclude.Alerts, Exclude.Flags };

            var result = await client.GetWeather(BolognaLatitude, BolognaLongitude, null, exclusionList, DSUnit.Auto, Language.Italian);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
            Assert.Null(result.Minutely);
            Assert.Null(result.Hourly);

            Assert.NotNull(result.Currently);
            Assert.NotNull(result.Daily);
            Assert.NotNull(result.Daily.Data);
            Assert.NotEmpty(result.Daily.Data);

            DateTimeOffset today =  DateTime.Today;

            Assert.Equal(result.Currently.Time.LocalDateTime.Date, today);
            Assert.Equal(result.Daily.Data[0].Time.LocalDateTime.Date, today);
        }

        /// <summary>
        ///     Checks that the service returns data using the specified units of measurement.
        /// </summary>
        [Fact]
        public async Task UnitsCanBeSpecified() {
            var client = new DarkSkyService(_apiKey);

            var result = await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude, DSUnit.CA);

            Assert.NotNull(result);
            Assert.Equal(result.Flags.Units, DSUnit.CA.ToValue());
        }

        /// <summary>
        /// Checks that retrieving data for a past date works correctly.
        /// </summary>
        [Fact]
        public async Task CanRetrieveForThePast() {
            var client = new DarkSkyService(_apiKey);
            var date = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));

            var result = await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, date);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        [Fact]
        public async Task GetForecast_ByCoordinates_Test() {
            // prepare
            var client = new DarkSkyService(_apiKey);
            var output = await client.GetForecast(BolognaLatitude, BolognaLongitude, DSUnit.SI, Language.Italian);
            // assert
            Assert.NotNull(output);
            Assert.NotNull(output.Currently);
            Assert.NotNull(output.Daily);
            output.Coordinates.Latitude.ShouldNotBe(0.0);
            output.Coordinates.Longitude.ShouldNotBe(0.0);
            output.Coordinates.Latitude.ShouldBe(BolognaLatitude);
            output.Coordinates.Longitude.ShouldBe(BolognaLongitude);
            output.Currently.Temperature.Daily.ShouldNotBeNull();
            output.Currently.Temperature.DewPoint.ShouldNotBe(0.0f);
            output.Currently.Temperature.Humidity.ShouldNotBeNull();
            output.Currently.Temperature.Humidity.Value.ShouldBeInRange(1, 100);
            output.Daily.Summary.ShouldNotBeNullOrWhiteSpace();
            output.Daily.Data.Count.ShouldBeGreaterThan(0);
            var yesterday = DateTime.Today.AddDays(-1);
            output.Daily.Data[0].Time.ShouldBeGreaterThan(yesterday);
            output.Daily.Data[0].Temperature.Pressure.ShouldNotBeNull();
            output.Daily.Data[0].Temperature.Pressure.Value.ShouldBeGreaterThan(0);
            output.Daily.Data[0].Temperature.Min.ShouldNotBeNull();
            output.Daily.Data[0].Temperature.Max.ShouldNotBeNull();
            output.Daily.Data[0].Temperature.Min.Value.ShouldBeLessThan(output.Daily.Data[0].Temperature.Max.Value);

            // prepare imperial
            var outputImperial = await client.GetForecast(BolognaLatitude, BolognaLongitude, DSUnit.US, Language.English);
            // assert
            Assert.NotNull(outputImperial);
            Assert.NotNull(outputImperial.Currently);
            Assert.NotNull(outputImperial.Daily);
            outputImperial.Coordinates.Latitude.ShouldBe(output.Coordinates.Latitude);
            outputImperial.Coordinates.Longitude.ShouldBe(output.Coordinates.Longitude);
            outputImperial.Currently.Temperature.Daily.ShouldNotBeNull();
            outputImperial.Currently.Temperature.Daily.Value.ShouldBeGreaterThan(output.Currently.Temperature.Daily.Value);
            outputImperial.Currently.Temperature.DewPoint.ShouldNotBe(output.Currently.Temperature.DewPoint);
            outputImperial.Currently.Temperature.Humidity.ShouldNotBeNull();
            outputImperial.Currently.Temperature.Humidity.Value.ShouldBeInRange(1, 100);
            outputImperial.Daily.Data[0].Temperature.Min.ShouldNotBeNull();
            outputImperial.Daily.Data[0].Temperature.Max.ShouldNotBeNull();
            outputImperial.Daily.Data[0].Temperature.Min.Value.ShouldBeLessThan(outputImperial.Daily.Data[0].Temperature.Max.Value);
            outputImperial.Daily.Data[0].ApparentTemperature.Min.Value.ShouldBeLessThan(outputImperial.Daily.Data[0].ApparentTemperature.Max.Value);
            outputImperial.Daily.Data[0].Temperature.Min.Value.ShouldBeGreaterThan(output.Daily.Data[0].Temperature.Min.Value);
            outputImperial.Daily.Data[0].Temperature.Max.Value.ShouldBeGreaterThan(output.Daily.Data[0].Temperature.Max.Value);
            outputImperial.Daily.Data[0].Temperature.DewPoint.ShouldNotBe(output.Daily.Data[0].Temperature.DewPoint);
            outputImperial.Daily.Data[0].Temperature.Humidity.ShouldNotBeNull();
            outputImperial.Daily.Data[0].Temperature.Humidity.Value.ShouldBeInRange(1, 100);
        }

        /// <summary>
        /// Checks that specifying a block to be excluded from forecasts for past dates
        /// will cause it to be null in the returned forecast.
        /// </summary>
        [Fact]
        public async Task TimeMachineExclusionWorksCorrectly() {
            var client = new DarkSkyService(_apiKey);
            var date = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
            var exclusionList = new List<Exclude> { Exclude.Hourly };

            var result = await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, date, exclusionList, DSUnit.SI);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
            Assert.Null(result.Hourly);
        }

        /// <summary>
        /// Checks that the service returns data using the specified units of measurement.
        /// </summary>
        [Fact]
        public async Task TimeMachineUnitsCanBeSpecified() {
            var client = new DarkSkyService(_apiKey);
            var date = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));

            var result = await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, date, DSUnit.CA);

            Assert.NotNull(result);
            Assert.Equal(result.Flags.Units, DSUnit.CA.ToValue());
        }

        [Fact]
        public async Task TimeMachineWorksWithCommaDecimalSeperator() {
            var client = new DarkSkyService(_apiKey);
            var date = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
            var result = await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, date, DSUnit.CA);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        [Fact]
        public async Task TimeMachineWorksWithPeriodDecimalSeperator() {
            var client = new DarkSkyService(_apiKey);
            var date = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            var result = await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, date, DSUnit.CA);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        [Fact]
        public async Task WorksWithCommaDecimalSeperator() {
            var client = new DarkSkyService(_apiKey);

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
            var result = await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        [Fact]
        public async Task WorksWithPeriodDecimalSeperator() {
            var client = new DarkSkyService(_apiKey);

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            var result = await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }
    }
}