using System;
using System.Collections.Generic;
using System.Linq;
using MultiWeatherApi.Mappers;
using MultiWeatherApi.Model;
using Nelibur.ObjectMapper;
using Shouldly;
using Xunit;
using DSModel = MultiWeatherApi.DarkSky.Model;

namespace Mappings.Test {

    /// <summary>
    ///     Tests for the main ForecastApi class.
    /// </summary>
    public class DarkSky_Mappings {

        static DarkSky_Mappings() {
            TinyMappers.Maps();
        }

        [Fact]
        public void Forecast_to_Weather() {
            // prepare
            DSModel.Forecast src = PrepareForecastData();
            // execute
            Weather output = TinyMapper.Map<Weather>(src);
            // assert
            output.TimeZone.ShouldBe(src.TimeZone);
            output.TimeZoneOffset.ShouldBe(src.TimeZoneOffset);
            output.Time.ShouldBe(src.Currently.Time);
            output.SunriseTime.ShouldBe(src.Currently.SunriseTime);
            output.TimeZone.ShouldBe(src.TimeZone);
            output.Coordinates.ShouldBe(src.Coordinates);
            output.Alerts.ShouldNotBeNull();
            output.Alerts.ShouldNotBeEmpty();
            output.Alerts.Select(dest => dest.Title).ShouldBe(src.Alerts.Select(al => al.Title));
            output.Alerts.Select(dest => dest.StartTime).ShouldBe(src.Alerts.Select(al => al.StartTime));
            output.Alerts.Select(dest => dest.Severity).ShouldBe(src.Alerts.Select(al => al.Severity));
        }

        [Fact]
        public void DataPoint_to_Weather() {
            // prepare
            DSModel.DataPoint src = PrepareForecastData().Daily.Data[0];
            // execute
            Weather output = TinyMapper.Map<Weather>(src);
            // assert
            output.Visibility.ShouldBe(src.Visibility);
            output.Temperature.ShouldNotBeNull();
            output.Temperature.Daily.ShouldBe(src.Temperature.Daily);
            output.Temperature.DewPoint.ShouldBe(src.Temperature.DewPoint);
        }

        [Fact]
        public void DataPointList_to_WeatherGroup() {
            // prepare
            List<DSModel.DataPoint> src = PrepareForecastData().Daily.Data;
            // execute
            WeatherGroup output = new WeatherGroup(TinyMapper.Map<List<Weather>>(src));
            // assert
            output.Count.ShouldBe(src.Count);
            output.ShouldAllBe(dp => dp.Visibility > 0);
            output.ShouldAllBe(dp => dp.Temperature != null);
            output.ShouldAllBe(dp => dp.Temperature.Daily != 0.0f);
            output.ShouldAllBe(dp => !string.IsNullOrWhiteSpace(dp.Summary));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static DSModel.Forecast PrepareForecastData() {
            return new DSModel.Forecast {
                TimeZone = "my timezone",
                TimeZoneOffset = 1.5f,
                Coordinates = new GeoCoordinates(10.12345, -10.54321),
                Alerts = new[] {
                    new Alert {
                        Title = "my title",
                        Description = "my desc",
                        StartTime = DateTime.UtcNow.AddDays(-1),
                        ExpiresTime = DateTime.UtcNow.AddDays(+1),
                        Severity = Severity.Advisory,
                        Uri = "https://example.com/"
                    }
                }.ToList(),
                Currently = new DSModel.DataPoint {
                    Time = DateTime.UtcNow,
                    SunriseTime = DateTime.UtcNow.Date.AddHours(+7),
                    SunsetTime = DateTime.UtcNow.Date.AddHours(+17),
                    Summary = "my summary",
                    Icon = "my_current_icon",
                    Visibility = 12345.0f,
                    Temperature = new Temperature { 
                        Daily = 1.1f,
                        DewPoint = 2.2f,
                        Evening = 3.3f,
                        Max = 4.4f,
                    },
                },
                Hourly = new DSModel.ForecastDetails {
                    Icon = "my_icon",
                    Summary = "my summary",
                    Data = new[] {
                        new DSModel.DataPoint {
                            Summary = "my datapoint 1 summary",
                            Temperature = new Temperature { 
                                Daily = 11.1f,
                                DewPoint = 22.2f,
                                Evening = 33.3f,
                                Max = 44.4f,
                            },
                            Visibility = 1111.1f,
                        },
                        new DSModel.DataPoint {
                            Summary = "my datapoint 2 summary",
                            Temperature = new Temperature {
                                Daily = -11.1f,
                                DewPoint = -22.2f,
                                Evening = -33.3f,
                                Max = -44.4f,
                            },
                            Visibility = 2222.2f,
                        }
                    }.ToList(),
                },
                Daily = new DSModel.ForecastDetails {
                    Icon = "my_daily_icon",
                    Summary = "my summary",
                    Data = new[] {
                        new DSModel.DataPoint {
                            Summary = "my daily datapoint 1 summary",
                            Visibility = 1111.1f,
                        },
                        new DSModel.DataPoint {
                            Summary = "my daily datapoint 2 summary",
                            Visibility = 2222.2f,
                        }
                    }.ToList(),
                },
            };
        }
    }
}