using System;
using MultiWeatherApi.Model;
using Nelibur.ObjectMapper;
using DSModel = MultiWeatherApi.DarkSky.Model;
using OWModel = MultiWeatherApi.OpenWeather.Model;

namespace MultiWeatherApi.Mappers {
    internal static class Mappering {

        public static void Maps() {
            if (!TinyMapper.BindingExists<DSModel.Forecast, Weather>())
                TinyMapper.Bind<DSModel.Forecast, Weather>(cfg => {
                    cfg.Bind(src => src.Currently.UnixTime, trg => trg.UnixTime);
                    cfg.Bind(src => src.Currently.SunriseUnixTime, trg => trg.SunriseUnixTime);
                    cfg.Bind(src => src.Currently.SunsetUnixTime, trg => trg.SunsetUnixTime);
                    cfg.Bind(src => src.Currently.Summary, trg => trg.Summary);
                    cfg.Bind(src => src.Currently.Summary, trg => trg.Description);
                    cfg.Bind(src => src.Currently.Icon, trg => trg.Icon);
                    //TODO cfg.Bind(src => src.Currently.IconUrl, trg => trg.IconUrl);
                    cfg.Bind(src => src.Currently.Visibility, trg => trg.Visibility);
                    cfg.Bind(src => src.Currently.Wind, trg => trg.Wind);
                    cfg.Bind(src => src.Currently.Temperature, trg => trg.Temperature);
                    cfg.Bind(src => src.Currently.ApparentTemperature, trg => trg.ApparentTemperature);
                });

            if (!TinyMapper.BindingExists<OWModel.ForecastDSL, Weather>())
                TinyMapper.Bind<OWModel.ForecastDSL, Weather>(cfg => {
                    cfg.Bind(src => src.Currently.UnixTime, trg => trg.UnixTime);
                    cfg.Bind(src => src.Currently.SunriseUnixTime, trg => trg.SunriseUnixTime);
                    cfg.Bind(src => src.Currently.SunsetUnixTime, trg => trg.SunsetUnixTime);
                    cfg.Bind(src => src.Currently.Summary, trg => trg.Summary);
                    cfg.Bind(src => src.Currently.Description, trg => trg.Description);
                    cfg.Bind(src => src.Currently.Icon, trg => trg.Icon);
                    cfg.Bind(src => src.Currently.IconUrl, trg => trg.IconUrl);
                    cfg.Bind(src => src.Currently.UnixTime, trg => trg.UnixTime);
                    cfg.Bind(src => src.Currently.Visibility, trg => trg.Visibility);
                    cfg.Bind(src => src.Currently.Wind, trg => trg.Wind);
                    cfg.Bind(src => src.Currently.Temperature, trg => trg.Temperature);
                    cfg.Bind(src => src.Currently.ApparentTemperature, trg => trg.ApparentTemperature);
                });

            if (!TinyMapper.BindingExists<OWModel.WeatherConditions, Weather>())
                // TODO
                TinyMapper.Bind<OWModel.WeatherConditions, Weather>(cfg => {
                    //cfg.Bind(src => src.Currently.WeatherInfo[0].Summary, trg => trg.Summary);
                    //cfg.Bind(src => src.Currently.WeatherInfo[0].Description, trg => trg.Description);
                    //cfg.Bind(src => src.Currently.WeatherInfo[0].Icon, trg => trg.Icon);
                    //cfg.Bind(src => src.Currently.WeatherInfo[0].IconUrl, trg => trg.IconUrl);
                });

            if (!TinyMapper.BindingExists<DSModel.Forecast, WeatherGroup>())
                TinyMapper.Bind<DSModel.Forecast, WeatherGroup>(cfg => {});

            if (!TinyMapper.BindingExists<DSModel.DataPoint, Weather>())
                TinyMapper.Bind<DSModel.DataPoint, Weather>(cfg => {});
        }
    }
}
