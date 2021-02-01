using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Helpers;
using Microsoft.Extensions.Configuration;
using MultiWeatherApi;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather;
using MultiWeatherApi.OpenWeather.Helpers;
using MultiWeatherApi.OpenWeather.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RichardSzalay.MockHttp;
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
        ///     Sets up all tests by retrieving the API key from app.config.
        /// </summary>
        public OpenWeather_Api() {
            //var config = new ConfigurationBuilder()
            //    .AddJsonFile("xunit.config.json")
            //    .Build();
            //_apiKey = config["OpenWeatherApiKey"];
        }

        /// <summary>
        ///     Checks that attempting to retrieve data with a null/wrong API key throws an exception.
        /// </summary>
        [Fact]
        public void InvalidKeyThrowsException() {
            // prepare
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(OpenWeatherService.EndPointRoot + "*")
                .Respond(HttpStatusCode.Unauthorized);
            // assert
            IOpenWeatherService client = new OpenWeatherService(null, mockHttp);
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetCurrentWeather(AlcatrazLatitude, AlcatrazLongitude));
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetForecast(AlcatrazLatitude, AlcatrazLongitude));

            client = new OpenWeatherService("", mockHttp);
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetForecastDSL(AlcatrazLatitude, AlcatrazLongitude, OWUnit.Imperial));
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetForecast(AlcatrazLatitude, AlcatrazLongitude));

            client = new OpenWeatherService("fake_key", mockHttp);
            Assert.ThrowsAsync<WeatherException>(async () => await client.GetCurrentWeather(BolognaCityName, OWUnit.Standard, Language.Catalan));
        }

        [Fact]
        public async void GetCurrentWeatherByCoordinates_Test() {
            // prepare
            WeatherConditions output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/OpenW_currentweather_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(OpenWeatherService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                IOpenWeatherService client = new OpenWeatherService("a_valid_key", mockHttp);

                output = await client.GetCurrentWeather(BolognaLatitude, BolognaLongitude, OWUnit.Metric, Language.Italian);
                stream.Close();
            }

            // assert
            output.ShouldNotBeNull();
            output.City.Name.ShouldNotBeNullOrWhiteSpace();
            output.City.Name.ShouldBe(BolognaCityName, StringCompareShould.IgnoreCase);
        }

        [Fact]
        public async Task GetCurrentWeatherByCityName_Test() {
            // prepare SI
            WeatherConditions output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/OpenW_currentweather_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(OpenWeatherService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                IOpenWeatherService client = new OpenWeatherService("12345", mockHttp);

                output = await client.GetCurrentWeather(BolognaLatitude, BolognaLongitude, OWUnit.Metric, Language.Italian);
                stream.Close();
            }
            // assert
            output.ShouldNotBeNull();
            output.Coordinates.Latitude.ShouldBe(44.4667);
            output.Coordinates.Longitude.ShouldBe(11.4333);
            output.WeatherInfo[0].Summary.ShouldBe("Clouds");
            output.WeatherInfo[0].Description.ShouldBe("nubi sparse");
            output.WeatherInfo[0].Icon.ShouldBe("03d");
            output.City.Name.ShouldNotBeNullOrWhiteSpace();
            output.City.Name.ShouldBe(BolognaCityName, StringCompareShould.IgnoreCase);
            output.City.CountryCode.ShouldBe("IT");
            output.City.SunriseTime.ToUnixTimeSeconds().ShouldBe(1610174975);

            // prepare imperial
            WeatherConditions outputImperial = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/OpenW_currentweather_Imperial.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(OpenWeatherService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                IOpenWeatherService client = new OpenWeatherService("12345", mockHttp);

                outputImperial = await client.GetCurrentWeather(BolognaCityName, OWUnit.Imperial, Language.English);
                stream.Close();
            }
            // assert
            output.ShouldNotBeNull();
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

        //[Fact]
        //public async void GetForecast_ByCoordinates_Test() {
        //    // prepare SI
        //    MultiWeatherConditions output = null;
        //    using (var stream = new BufferedStream(File.OpenRead("./Resources/OpenW_forecast_SI_French.json"), 8192)) {
        //        var mockHttp = new MockHttpMessageHandler();
        //        mockHttp
        //            .When(OpenWeatherService.EndPointRoot + "*")
        //            .Respond("application/json", stream);
        //        IOpenWeatherService client = new OpenWeatherService("a_valid_key", mockHttp);

        //        output = await client.GetForecast(BolognaLatitude, BolognaLongitude, OWUnit.Metric, Language.French);
        //        stream.Close();
        //    }
        //    // assert
        //    output.ShouldNotBeNull();
        //    output.Coordinates.ShouldNotBeNull();
        //    output.Coordinates.Latitude.ShouldBe(44.4743);
        //    output.Coordinates.Longitude.ShouldBe(11.4301);
        //    output.City.ShouldNotBeNull();
        //    output.City.Country.ShouldBe("IT");
        //    output.City.CountryCode.ShouldBe("IT");
        //    output.City.Name.ShouldBe("Bologne");
        //    output.City.TimeZone.ShouldBeNullOrEmpty();
        //    output.City.SunriseTime.ShouldBe(1611470448.ToDateTimeOffset());
        //    output.City.SunsetTime.ShouldBe(1611504699.ToDateTimeOffset());
        //}

        [Fact]
        public async void GetForecastDSL_ByCoordinates_Test() {
            // prepare SI
            ForecastDSL output = null;
            using (var stream = new BufferedStream(File.OpenRead("./Resources/OpenW_onecall_SI.json"), 8192)) {
                var mockHttp = new MockHttpMessageHandler();
                mockHttp
                    .When(OpenWeatherService.EndPointRoot + "*")
                    .Respond("application/json", stream);
                IOpenWeatherService client = new OpenWeatherService("12345", mockHttp);

                output = await client.GetForecastDSL(BolognaLatitude, BolognaLongitude, OWUnit.Metric, Language.Italian);
                stream.Close();
            }
            // assert
            output.ShouldNotBeNull();
            output.Coordinates.Latitude.ShouldBe(44.47);
            output.Coordinates.Longitude.ShouldBe(11.43);
            output.TimeZone.ShouldBe("Europe/Rome");
            output.TimeZoneOffset.ShouldBe(3600);

            output.Currently.ShouldNotBeNull();
            output.Currently.Time.ToUnixTimeSeconds().ShouldBe(1609711216L);
            output.Currently.Wind.Speed.ShouldBe(1.5f);
            output.Currently.Wind.Bearing.ShouldBe(260);
            output.Currently.Visibility.ShouldBe(10000);
            output.Currently.Rain.ShouldBe(0.24f);
            output.Currently.Snow.ShouldBe(1.01f);
            output.Currently.ProbOfPrecipitation.ShouldBeNull();
            output.Currently.SunriseTime.Value.ToUnixTimeSeconds().ShouldBe(1609656638);
            output.Currently.SunsetTime.Value.ToUnixTimeSeconds().ShouldBe(1609688798);
            //output.Current.SunriseTime.ShouldBe(((int)1609656638).ToDateTimeOffset());
            output.Currently.SunriseTime.ShouldBe(new DateTime(2021, 01, 03, 6, 50, 38, DateTimeKind.Utc)); ;
            output.Currently.SunsetTime.Value.ShouldBeGreaterThan(output.Currently.SunriseTime.Value);
            output.Currently.Temperature.Daily.ShouldBe(4.14f);
            output.Currently.Temperature.DewPoint.ShouldBe(4.11f);
            output.Currently.ApparentTemperature.Daily.ShouldBe(1.8f);
            output.Currently.Temperature.Humidity.ShouldBe(100);
            output.Currently.Temperature.Pressure.ShouldBe(1010);

            output.Hourly.Count.ShouldBe(48);
            output.Hourly[0].Visibility.ShouldBe(10000);
            output.Hourly[0].Rain.ShouldBe(0.24f);
            output.Hourly[0].ProbOfPrecipitation.ShouldBe(0.3f);
            output.Hourly[0].Temperature.Daily.ShouldBe(4.14f);
            output.Hourly[0].Temperature.DewPoint.ShouldBe(4.11f);
            output.Hourly[0].Temperature.Min.ShouldBeNull();
            output.Hourly[0].Temperature.Max.ShouldBeNull();
            output.Hourly[0].Temperature.Night.ShouldBeNull();
            output.Hourly[0].Temperature.Morning.ShouldBeNull();
            output.Hourly[0].Temperature.Humidity.ShouldBe(100);
            output.Hourly[0].Temperature.Pressure.ShouldBe(1010);
            output.Hourly[0].ApparentTemperature.Daily.ShouldBe(1.7f);
            output.Hourly[0].ApparentTemperature.Min.ShouldBeNull();
            output.Hourly[0].ApparentTemperature.Max.ShouldBeNull();
            output.Hourly[0].ApparentTemperature.Night.ShouldBeNull();
            output.Hourly[0].ApparentTemperature.Morning.ShouldBeNull();
            output.Hourly[0].Temperature.Humidity.ShouldBe(100);

            output.Daily.Count.ShouldBe(8);
            output.Daily[0].Visibility.ShouldBeNull();
            output.Daily[0].Rain.ShouldBe(0.58f);
            output.Daily[0].Snow.ShouldBe(0.99f);
            output.Daily[0].ProbOfPrecipitation.ShouldBe(0.6f);
            output.Daily[0].UVIndex.ShouldBe(0.94f);
            output.Daily[0].Temperature.Daily.ShouldBe(9.5f);
            output.Daily[0].Temperature.Night.ShouldBe(4.16f);
            output.Daily[0].Temperature.Evening.ShouldBe(6.87f);
            output.Daily[0].Temperature.Morning.ShouldBe(5.93f);
            output.Daily[0].Temperature.Min.ShouldBe(4.14f);
            output.Daily[0].Temperature.Max.ShouldBe(9.5f);
            output.Daily[0].Temperature.Humidity.ShouldBe(74);
            output.Daily[0].Temperature.Pressure.ShouldBe(1009);
            output.Daily[0].Temperature.DewPoint.ShouldBe(4.83f);
            output.Daily[0].ApparentTemperature.Daily.ShouldBe(7.3f);
            output.Daily[0].ApparentTemperature.Night.ShouldBe(1.7f);
            output.Daily[0].ApparentTemperature.Evening.ShouldBe(5.18f);
            output.Daily[0].ApparentTemperature.Morning.ShouldBe(2.8f);
            output.Daily[0].ApparentTemperature.Min.ShouldBeNull();
            output.Daily[0].ApparentTemperature.Max.ShouldBeNull();

            output.Alerts.ShouldNotBeNull();
            output.Alerts.Count.ShouldBe(2);
            output.Alerts[0].Sender.ShouldBe("NWS Tulsa (Eastern Oklahoma)");
            output.Alerts[0].Title.ShouldBe("Heat Advisory");
            output.Alerts[0].StartTime.ToUnixTimeSeconds().ShouldBe(1597341600);
            output.Alerts[0].ExpiresTime.ToUnixTimeSeconds().ShouldBe(1597366800);
            output.Alerts[0].Description.ShouldStartWith("...HEAT ADVISORY REMAINS IN EFFECT");
            output.Alerts[0].Severity.ShouldBe(Severity.Unknown);

            output.Alerts[1].Sender.ShouldBe("Second sender name");
            output.Alerts[1].Title.ShouldBe("Cold Advisory");
            output.Alerts[1].Description.ShouldBe("...Test\nWeather\nCondition.");

            //// prepare imperial
            //var outputImperial = await client.GetForecastDSL(BolognaLatitude, BolognaLongitude, OWUnit.Imperial, Language.English);
            //// assert
            //outputImperial.Coordinates.Latitude.ShouldBe(output.Coordinates.Latitude);
            //outputImperial.Coordinates.Longitude.ShouldBe(output.Coordinates.Longitude);
            //outputImperial.Currently.Temperature.Daily.ShouldNotBeNull();
            //outputImperial.Currently.Temperature.Daily.Value.ShouldBeGreaterThan(output.Currently.Temperature.Daily.Value);
            //outputImperial.Currently.Wind.Speed.ShouldNotBe(output.Currently.Wind.Speed);
            //outputImperial.Currently.Wind.Bearing.ShouldBe(output.Currently.Wind.Bearing);
        }

        [Fact]
        public void Serialize_OpenW_currentWeather() {
            var filename = "./Resources/OpenW_currentweather_SI.json";
            var client = new WeatherServiceBase_Wrapper();

            MultiWeatherApi.OpenWeather.Model.WeatherConditions input = null;
            using (var jsonStream = File.OpenRead(filename)) {
                input = client.ParseJsonFromStream_Wrapper<MultiWeatherApi.OpenWeather.Model.WeatherConditions>(jsonStream);
            }

            string outputJson = null;
            using (var sw = new StringWriter())
            using (var jw = new JsonTextWriter(sw)) {
                var serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
                //serializer.Formatting = Formatting.Indented;
                serializer.Serialize(jw, input);
                outputJson = sw.ToString();
            }
            outputJson.ShouldNotBeNullOrWhiteSpace();
            outputJson[0].ShouldBe('{');
            outputJson[outputJson.Length - 1].ShouldBe('}');
        }

        [Fact]
        public void Serialize_OpenW_onecall() {
            var filename = "./Resources/OpenW_onecall_SI.json";
            var client = new WeatherServiceBase_Wrapper();

            ForecastDSL input = null;
            using (var jsonStream = File.OpenRead(filename)) {
                input = client.ParseJsonFromStream_Wrapper<ForecastDSL>(jsonStream, new MyAlertConverter());
            }

            string outputJson = null;
            using (var sw = new StringWriter())
            using (var jw = new JsonTextWriter(sw)) {
                var serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
                //serializer.Formatting = Formatting.Indented;
                serializer.Converters.Add(new MyAlertConverter());
                serializer.Serialize(jw, input);
                outputJson = sw.ToString();
            }
            outputJson.ShouldNotBeNullOrWhiteSpace();
            outputJson[0].ShouldBe('{');
            outputJson[outputJson.Length - 1].ShouldBe('}');
        }

    }
}