﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MultiWeatherApi.DarkSkyApi;
using MultiWeatherApi.DarkSkyApi.Model;
using Shouldly;
using Xunit;

namespace MultiWeatherApi.DarkSky.Test {

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
            Assert.NotNull(result.Daily.Days);
            result.Daily.Days.Count.ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Checks that a request can be made for a non-US location.
        /// Added to test GitHub issue 6.
        /// </summary>
        [Fact]
        public async Task NonUSDataCanBeRetrieved() {
            var client = new DarkSkyService(_apiKey);

            var result = await client.GetCurrentWeather(BolognaLatitude, BolognaLongitude, Unit.SI, Language.Italian);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        /// <summary>
        ///     Checks that requests can be made with Unit.SI.
        /// </summary>
        [Fact]
        public async Task UnitSIWorksCorrectly() {
            var client = new DarkSkyService(_apiKey);

            var result = await client.GetCurrentWeather(MumbaiLatitude, MumbaiLongitude, Unit.SI);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        /// <summary>
        ///     Checks that requests can be made with Unit.UK.
        /// </summary>
        [Fact]
        public async Task UnitUKWorksCorrectly() {
            var client = new DarkSkyService(_apiKey);

            var result = await client.GetCurrentWeather(MumbaiLatitude, MumbaiLongitude, Unit.UK);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        /// <summary>
        /// Checks that requests can be made with Unit.UK2.
        /// Added to test GitHub issue 18.
        /// </summary>
        [Fact]
        public async Task UnitUK2WorksCorrectly() {
            var client = new DarkSkyService(_apiKey);

            var result = await client.GetCurrentWeather(MumbaiLatitude, MumbaiLongitude, Unit.UK2);

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

            var result = await client.GetWeather(AlcatrazLatitude, AlcatrazLongitude, null, exclusionList, Unit.SI, Language.Italian);

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

            var result = await client.GetWeather(AlcatrazLatitude, AlcatrazLongitude, null, exclusionList, Unit.SI);

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

            var result = await client.GetWeather(BolognaLatitude, BolognaLongitude, null, exclusionList, Unit.Auto, Language.Italian);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
            Assert.Null(result.Minutely);
            Assert.Null(result.Hourly);

            Assert.NotNull(result.Currently);
            Assert.NotNull(result.Daily);
            Assert.NotNull(result.Daily.Days);
            Assert.NotEmpty(result.Daily.Days);

            var today = DateTime.Today;

            Assert.Equal(result.Currently.Time.LocalDateTime.Date, today);
            Assert.Equal(result.Daily.Days[0].Time.LocalDateTime.Date, today);
        }

        /// <summary>
        ///     Checks that the service returns data using the specified units of measurement.
        /// </summary>
        [Fact]
        public async Task UnitsCanBeSpecified() {
            var client = new DarkSkyService(_apiKey);

            var result = await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude, Unit.CA);

            Assert.NotNull(result);
            Assert.Equal(result.Flags.Units, Unit.CA.ToValue());
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

        /// <summary>
        /// Checks that specifying a block to be excluded from forecasts for past dates
        /// will cause it to be null in the returned forecast.
        /// </summary>
        [Fact]
        public async Task TimeMachineExclusionWorksCorrectly() {
            var client = new DarkSkyService(_apiKey);
            var date = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
            var exclusionList = new List<Exclude> { Exclude.Hourly };

            var result = await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, date, exclusionList, Unit.SI);

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

            var result = await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, date, Unit.CA);

            Assert.NotNull(result);
            Assert.Equal(result.Flags.Units, Unit.CA.ToValue());
        }

        [Fact]
        public async Task TimeMachineWorksWithCommaDecimalSeperator() {
            var client = new DarkSkyService(_apiKey);
            var date = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fr-FR");
            var result = await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, date, Unit.CA);

            Assert.NotNull(result);
            Assert.NotNull(result.Currently);
        }

        [Fact]
        public async Task TimeMachineWorksWithPeriodDecimalSeperator() {
            var client = new DarkSkyService(_apiKey);
            var date = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            var result = await client.GetWeatherByDate(AlcatrazLatitude, AlcatrazLongitude, date, Unit.CA);

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