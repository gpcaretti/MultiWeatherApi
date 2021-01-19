using System.Collections.Generic;
using MultiWeatherApi.Model;
using Nelibur.ObjectMapper;
using DSModel = MultiWeatherApi.DarkSky.Model;
using OWModel = MultiWeatherApi.OpenWeather.Model;

namespace MultiWeatherApi.Mappers {

    /// <summary>
    ///     'public' just for unit tests :(
    /// </summary>
    public static class TinyMappers {

        public static void Maps() {
            DarkSkyMaps();
            OpenWeatherMaps();
        }

        private static void DarkSkyMaps() {
            // main forecast object
            TinyMapper.Bind<DSModel.Forecast, Weather>(cfg => {
                cfg.Bind(src => src.Currently.UnixTime, trg => trg.UnixTime);
                cfg.Bind(src => src.Currently.SunriseUnixTime, trg => trg.SunriseUnixTime);
                cfg.Bind(src => src.Currently.SunsetUnixTime, trg => trg.SunsetUnixTime);
                cfg.Bind(src => src.Currently.Summary, trg => trg.Summary);
                cfg.Bind(src => src.Currently.Summary, trg => trg.Description); // src desc does not exist so map the summary
                cfg.Bind(src => src.Currently.Icon, trg => trg.Icon);
                //TODO cfg.Bind(src => src.Currently.IconUrl, trg => trg.IconUrl);
                cfg.Bind(src => src.Currently.Visibility, trg => trg.Visibility);
                cfg.Bind(src => src.Currently.Wind, trg => trg.Wind);
                cfg.Bind(src => src.Currently.Temperature, trg => trg.Temperature);
                cfg.Bind(src => src.Currently.ApparentTemperature, trg => trg.ApparentTemperature);
                cfg.Ignore(src => src.Hourly);  // do it manually as TM throw an ex.
            });

            // main forecast object into a weather group
            TinyMapper.Bind<DSModel.Forecast, WeatherGroup>(cfg => { });

            // a forecast data point
            TinyMapper.Bind<DSModel.DataPoint, Weather>(cfg => { });

            // a list of forecast data points into a group of weathers
            //TinyMapper.Bind<List<DSModel.DataPoint>, WeatherGroup>(cfg => { });   // <= does not work
            TinyMapper.Bind<List<DSModel.DataPoint>, List<Weather>>(cfg => { });
        }

        private static void OpenWeatherMaps() {
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
                cfg.Ignore(src => src.Hourly);  // do it manually as TM throw an ex.
            });

            // main forecast object into a weather group
            TinyMapper.Bind<OWModel.ForecastDSL, WeatherGroup>(cfg => { });

            // a forecast data point
            TinyMapper.Bind<OWModel.DataPointDSL, Weather>(cfg => { });

            // a list of forecast data points into a group of weathers
            //TinyMapper.Bind<List<OWModel.DataPointDSL>, WeatherGroup>(cfg => { });   // <= does not work
            TinyMapper.Bind<List<OWModel.DataPointDSL>, List<Weather>>(cfg => { });


            // TODO
            TinyMapper.Bind<OWModel.WeatherConditions, Weather>(cfg => {
                //cfg.Bind(src => src.Currently.WeatherInfo[0].Summary, trg => trg.Summary);
                //cfg.Bind(src => src.Currently.WeatherInfo[0].Description, trg => trg.Description);
                //cfg.Bind(src => src.Currently.WeatherInfo[0].Icon, trg => trg.Icon);
                //cfg.Bind(src => src.Currently.WeatherInfo[0].IconUrl, trg => trg.IconUrl);
            });
        }
    }
}
