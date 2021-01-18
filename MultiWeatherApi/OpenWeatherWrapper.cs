using System;
using System.Threading.Tasks;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather;
using OWModel = MultiWeatherApi.OpenWeather.Model;
using Nelibur.ObjectMapper;

namespace MultiWeatherApi {

    internal class OpenWeatherWrapper : IWeatherService {

        private IOpenWeatherService _service;

        /// <summary>Set the api key for the underneath weather service</summary>
        public string ApiKey { set => _service = new OpenWeatherService(value); }

        /// <summary>
        ///     Default constructor. Require to soon set the <see cref="ApiKey"/>.
        /// </summary>
        public OpenWeatherWrapper() {
        }

        /// <summary>
        ///     Constructor allowing to set the <see cref="ApiKey"/> for the underneath weather service
        /// </summary>
        /// <param name="apiKey">the API key for the underneath weather service</param>
        public OpenWeatherWrapper(string apiKey) {
            ApiKey = apiKey;
        }

        public async Task<Weather> GetCurrentWeather(double latitude, double longitude, Unit unit = Unit.Auto, Language language = Language.English) {
            var src = await _service.GetForecastDSL(latitude, longitude, (OWModel.OWUnit)unit, language);
            var output = TinyMapper.Map<Weather>(src);

            // === do some adaptation that TinyMapper does not do quickly
            // Convert time offset from seconds to hours
            if (src.TimeZoneOffset != 0) output.TimeZoneOffset = (float)Math.Round(src.TimeZoneOffset / 3600.0f, 1);

            // TODO Map this: output.Hourly = innerW.Hourly,

            // === ALTERNATIVE using GetCurrentWeather
            //var innerW = await _service.GetCurrentWeather(latitude, longitude, (OWUnit)unit, language);
            //var output = TinyMapper.Map<Model.Weather>((object)innerW);
            // ===

            return output;
        }

        public async Task<WeatherGroup> GetForecast(double latitude, double longitude, Unit unit = Unit.Auto, Language language = Language.English) {
            OWModel.ForecastDSL src = await _service.GetForecastDSL(latitude, longitude, (OWModel.OWUnit)unit, language);

            //var output_meno = await _service.GetForecastDSL(14.93122218816076, -23.519346790741995, (OWModel.OWUnit)unit, Language.Italian);
            //var output_zero = await _service.GetForecastDSL(51.53027816900633, 0.08310560054000066, (OWModel.OWUnit)unit, Language.Italian);
            //var output_piu = await _service.GetForecastDSL(44.482732, 11.352134, (OWModel.OWUnit)unit, Language.Italian);

            var output = new WeatherGroup(src.Daily?.Count ?? 16);
            output = TinyMapper.Map<OWModel.ForecastDSL, WeatherGroup>(src, output);
            // do some normalization (convert time offset from seconds to hours)
            if (src.TimeZoneOffset != 0) output.TimeZoneOffset = (float)Math.Round(output.TimeZoneOffset / 3600.0f, 1);

            foreach (var dataPoint in src.Daily) {
                // convert datapoints patching/normalizing some values
                var weatherOftheDay = TinyMapper.Map<Weather>(dataPoint);
                if (string.IsNullOrEmpty(weatherOftheDay.TimeZone)) weatherOftheDay.TimeZone = output.TimeZone;
                if (weatherOftheDay.TimeZoneOffset == 0.0f) weatherOftheDay.TimeZoneOffset = output.TimeZoneOffset;
                if (weatherOftheDay.Coordinates == null) weatherOftheDay.Coordinates = output.Coordinates;
                // normalize time unix utc
                if (src.TimeZoneOffset != 0) weatherOftheDay.UnixTime += src.TimeZoneOffset;

                output.Add(weatherOftheDay);
            }

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<Weather> GetWeatherByDate(double latitude, double longitude, DateTime date, Unit unit = Unit.Auto, Language language = Language.English) {
            // if the date is today, return current weather
            if (date.Date == DateTime.UtcNow.Date) {
                return await GetCurrentWeather(latitude, longitude, unit, language);
            }

            // get the forecast
            WeatherGroup output = await GetForecast(latitude, longitude, unit, language);

            // select the requested date
            var theRightWeather = output.Find(d =>
                                    (d.SunriseTime != null) &&
                                    (d.SunriseTime.Value.Date.Equals(date.Date)));

            if (theRightWeather == null) {
                // generate an empty forecast
                theRightWeather = new Weather {
                    Coordinates = output[0]?.Coordinates ?? new GeoCoordinates(latitude, longitude),
                    TimeZone = output[0]?.TimeZone,
                    TimeZoneOffset = output[0]?.TimeZoneOffset ?? 0.0f,
                    UnixTime = date.Date.ToUnixTime(),
                    Alerts = new[] { new Alert {
                            Title = "NO AVAILABLE FORECAST FOR THE REQUESTED DATE.",
                            Description = $"You requested a forecast for {date.Date:o}",
                            Severity = Severity.Warning,
                            StartUnixTime = DateTime.UtcNow.ToUnixTime(),
                            ExpiresUnixTime = date.Date.AddDays(-7).ToUnixTime(),
                        } },
                };
            }
            return theRightWeather;

            //if ((output.Daily?.Count ?? 0) > 0) {
            //    // if there are more info on the daily data, copy it on output.Currently
            //    var sameDay = src.Daily.FirstOrDefault(d =>
            //        (d.SunriseTime != null) &&
            //        (d.SunriseTime.Value.Date.Equals(output.Currently.Time.Date)));
            //    if (sameDay != null) src.Currently = 
            //        var currently = output.Currently;

            //        //src.Currently = 
            //        var output = TinyMapper.Map<Weather>(src);

            // === ALTERNATIVE using GetCurrentWeather
            //var innerW = await _service.GetForecast(latitude, longitude, (OWModel.OWUnit)unit, language);
            //var output = TinyMapper.Map<Model.Weather>((object)innerW);
            // ===

            // === do some adaptation that TinyMapper does not do quickly

            // TODO Map this: output.Hourly = innerW.Hourly,
        }
    }
}