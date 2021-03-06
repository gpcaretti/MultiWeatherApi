﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiWeatherApi.DarkSky.Model;
using MultiWeatherApi.Model;

namespace MultiWeatherApi.DarkSky {

    /// <summary>
    ///     The interface for the Dark Sky service. 
    /// </summary>
    public interface IDarkSkyService {

        /// <summary>
        ///     Gets the number of API calls made today using the given API key.
        ///     <para>This property will be null until a request has been made.</para>
        /// </summary>
        int? ApiCallsMade { get; }

        /// <summary>
        ///    Returns the current weather conditions, included an hour-by-hour forecast for the next 48 hours and possible alerts
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="unit">Default is <see cref="Model.DSUnit.Auto"/></param>
        /// <param name="language">Default is <see cref="Language.English"/></param>
        /// <returns>A <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="WeatherException">Thrown if the response did not have a status code indicating success.</exception>
        Task<Forecast> GetCurrentWeather(double latitude, double longitude, Model.DSUnit unit = Model.DSUnit.Auto, Language language = Language.English);

        /// <summary>
        ///    Returns the current weather conditions, included
        ///     * an hour-by-hour forecast for the next 48 hours, 
        ///     * the daily forecast for the next 7 days and 
        ///     * possible alerts
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="unit">Default is <see cref="Model.DSUnit.Auto"/></param>
        /// <param name="language">Default is <see cref="Language.English"/></param>
        /// <returns>A <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="WeatherException">Thrown if the response did not have a status code indicating success.</exception>
        Task<Forecast> GetForecast(double latitude, double longitude, Model.DSUnit unit = Model.DSUnit.Auto, Language language = Language.English);

        /// <summary>
        ///    The most general method to retrieves weather data and forecast for a particular latitude and longitude (full optional).
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="excludes">Any blocks that should be excluded from the request.</param>
        /// <param name="extends">The type of forecast to retrieve extended results for. Currently limited to hourly blocks.</param>
        /// <param name="unit">Default is <see cref="Model.DSUnit.Auto"/></param>
        /// <param name="language">Default is <see cref="Language.English"/></param>
        /// <returns>A <see cref="Task"/> for a <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="WeatherException">Thrown if the response did not have a status code indicating success.</exception>
        Task<Forecast> GetWeather(double latitude, double longitude, IList<Extend> extends, IList<Exclude> excludes, Model.DSUnit unit = Model.DSUnit.Auto, Language language = Language.English);

        /// <summary>
        ///     Retrieve the observed (in the past) or forecasted (in the future) hour-by-hour weather and daily weather 
        ///     conditions for a particular date. A Time Machine request is identical in structure to a Forecast Request, except:
        ///     * The 'currently' data point will refer to the time provided, rather than the current time.
        ///     * The 'minutely' data block will be omitted, unless you are requesting a time within an hour of the present.
        ///     * The 'hourly data block will contain data points starting at midnight (local time) of the day requested, and continuing 
        ///       until midnight (local time) of the following day.
        ///     * The 'daily' data block will contain a single data point referring to the requested date.
        ///     * The 'alerts' data block will be omitted.
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="date">Requested date</param>
        /// <param name="unit">Default <see cref="Model.DSUnit.Auto"/></param>
        /// <param name="language">Default <see cref="Language.English"/></param>
        /// <returns>A <see cref="Task"/> for a <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="WeatherException">Thrown if the response did not have a status code indicating success.</exception>
        Task<Forecast> GetWeatherByDate(double latitude, double longitude, DateTimeOffset date, Model.DSUnit unit = Model.DSUnit.Auto, Language language = Language.English);

        /// <summary>
        ///     Retrieve the observed (in the past) or forecasted (in the future) hour-by-hour weather and daily weather 
        ///     conditions for a particular date. A Time Machine request is identical in structure to a Forecast Request, except:
        ///     * The 'currently' data point will refer to the time provided, rather than the current time.
        ///     * The 'minutely' data block will be omitted, unless you are requesting a time within an hour of the present.
        ///     * The 'hourly data block will contain data points starting at midnight (local time) of the day requested, and continuing 
        ///       until midnight (local time) of the following day.
        ///     * The 'daily' data block will contain a single data point referring to the requested date.
        ///     * The 'alerts' data block will be omitted.
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="date">Requested date</param>
        /// <param name="excludes">Any blocks that should be excluded from the request.</param>
        /// <param name="unit">Default <see cref="Model.DSUnit.Auto"/></param>
        /// <param name="language">Default <see cref="Language.English"/></param>
        /// <returns>A <see cref="Task"/> for a <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="WeatherException">Thrown if the response did not have a status code indicating success.</exception>
        Task<Forecast> GetWeatherByDate(double latitude, double longitude, DateTime date, IList<Exclude> excludes, Model.DSUnit unit = Model.DSUnit.Auto, Language language = Language.English);

        /// <summary>
        ///     The most general method to retrieve the observed (in the past) or forecasted (in the future) hour-by-hour weather and daily weather 
        ///     conditions for a particular date. A Time Machine request is identical in structure to a Forecast Request, except:
        ///     * The 'currently' data point will refer to the time provided, rather than the current time.
        ///     * The 'minutely' data block will be omitted, unless you are requesting a time within an hour of the present.
        ///     * The 'hourly data block will contain data points starting at midnight (local time) of the day requested, and continuing 
        ///       until midnight (local time) of the following day.
        ///     * The 'daily' data block will contain a single data point referring to the requested date.
        ///     * The 'alerts' data block will be omitted.
        /// </summary>
        /// <param name="latitude">The latitude to retrieve data for.</param>
        /// <param name="longitude">The longitude to retrieve data for.</param>
        /// <param name="date">Requested date</param>
        /// <param name="excludes">Any blocks that should be excluded from the request.</param>
        /// <param name="extends">The type of forecast to retrieve extended results for. Currently limited to hourly blocks.</param>
        /// <param name="unit">Default <see cref="Model.DSUnit.Auto"/></param>
        /// <param name="language">Default <see cref="Language.English"/></param>
        /// <returns>A <see cref="Task"/> for a <see cref="Forecast"/> with the requested data, or null if the data was corrupted.</returns>
        /// <exception cref="WeatherException">Thrown if the response did not have a status code indicating success.</exception>
        Task<Forecast> GetWeatherByDate(double latitude, double longitude, DateTimeOffset date, IList<Extend> extends, IList<Exclude> excludes, Model.DSUnit unit = Model.DSUnit.Auto, Language language = Language.English);
    }
}