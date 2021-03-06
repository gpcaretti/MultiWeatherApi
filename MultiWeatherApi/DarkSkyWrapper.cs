﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MultiWeatherApi.DarkSky;
using MultiWeatherApi.Model;
using Nelibur.ObjectMapper;
using DSModel = MultiWeatherApi.DarkSky.Model;

namespace MultiWeatherApi {

    internal class DarkSkyWrapper : IWeatherService {

        private readonly IDarkSkyService _service;

        /// <summary>
        ///     Initializes a new instance of the weather service using the default <see cref="HttpMessageHandler"/>
        /// </summary>
        /// <param name="key">The API key to use.</param>
        public DarkSkyWrapper(string key) : this(key, null) {
        }

        /// <summary>
        ///     Initializes a new instance of the weather service
        /// </summary>
        /// <param name="apiKey">The API key to use.</param>
        /// <param name="handler">the http message handler. If null use the default one.</param>
        public DarkSkyWrapper(string apiKey, HttpMessageHandler handler) {
            _service = new DarkSkyService(apiKey, handler);
        }

        public async Task<Weather> GetCurrentWeather(double latitude, double longitude, Unit unit = Unit.Auto, Language language = Language.English) {
            DSModel.Forecast src = await _service.GetCurrentWeather(latitude, longitude, (DSModel.DSUnit)unit, language);
            var output = TinyMapper.Map<Weather>(src);
            // now maps the hours
            if ((src.Hourly?.Data?.Count ?? 0) > 0) {
                var hourly = TinyMapper.Map<WeatherGroup>(src);
                hourly.AddRange(TinyMapper.Map<List<Weather>>(src.Hourly.Data));
                output.Hourly = hourly;
            }

            return output;
        }

        public async Task<WeatherGroup> GetForecast(double latitude, double longitude, Unit unit = Unit.Auto, Language language = Language.English) {
            DSModel.Forecast src = await _service.GetForecast(latitude, longitude, (DSModel.DSUnit)unit, language);

            var output = new WeatherGroup(src.Daily?.Data?.Count ?? 16);
            TinyMapper.Map<DSModel.Forecast, WeatherGroup>(src, output);
            foreach (var dataPoint in src.Daily.Data) {
                // convert datapoints patching/normalizing some values
                var weatherOftheDay = TinyMapper.Map<Weather>(dataPoint);
                if (string.IsNullOrEmpty(weatherOftheDay.TimeZone)) weatherOftheDay.TimeZone = output.TimeZone;
                if (weatherOftheDay.TimeZoneOffset == 0.0f) weatherOftheDay.TimeZoneOffset = output.TimeZoneOffset;
                if (weatherOftheDay.Coordinates == null) weatherOftheDay.Coordinates = output.Coordinates;
                // normalize time unix utc
                if (Math.Abs(output.TimeZoneOffset) > 0.4f) weatherOftheDay.UnixTime += (int)(3600 * output.TimeZoneOffset);

                output.Add(weatherOftheDay);
            }

            return output;
        }

        public async Task<Weather> GetWeatherByDate(double latitude, double longitude, DateTime dateTime, Unit unit = Unit.Auto, Language language = Language.English) {
            DSModel.Forecast src = await _service.GetWeatherByDate(latitude, longitude, dateTime, (DSModel.DSUnit)unit, language);
            var output = TinyMapper.Map<Weather>(src);
            // TODO Map this: output.Hourly = innerW.Hourly,
            return output;
        }
    }
}