using System;
using MultiWeatherApi.Model;

namespace MultiWeatherApi.DarkSky.Model {

    /// <summary>
    ///     Units of measurement supported by the Forecast service.
    /// </summary>
    public enum DSUnit
    {
        /// <summary>
        ///     Imperial US units of measurement.
        /// <para>
        ///     Summaries containing temperature or snow accumulation units have
        ///     their values in degrees Fahrenheit or inches (respectively).
        ///     Nearest storm distance: Miles
        ///     Precipitation intensity: Inches/hour
        ///     Precipitation accumulation: Inches
        ///     Temperature: Fahrenheit
        ///     Dew Point: Fahrenheit
        ///     Wind Speed: Miles/hour
        ///     Pressure: Millibars
        ///     Visibility: Miles
        /// </para>
        /// </summary>
        US = Unit.Imperial,

        /// <summary>
        ///     SI units of measurement.
        /// <para>
        ///     Summaries containing temperature or snow accumulation units have
        ///     their values in degrees Celsius or centimeters (respectively).
        ///     Nearest storm distance: km
        ///     Precipitation intensity: mm/hour
        ///     Precipitation accumulation: cm
        ///     Temperature: Celsius
        ///     Dew Point: Celsius
        ///     Wind Speed: m/s
        ///     Pressure: Hectopascals (equivalent to millibars)
        ///     Visibility: km
        /// </para>
        /// </summary>
        SI = Unit.SI,

        /// <summary>
        /// Canadian units of measurement. The same as SI, but with 
        /// kilometers per hour used for Wind Speed.
        /// <para>
        /// Summaries containing temperature or snow accumulation units have
        /// their values in degrees Celsius or centimeters (respectively).
        /// Nearest storm distance: Kilometers
        /// Precipitation intensity: Millimeters per hour
        /// Precipitation accumulation: Centimeters
        /// Temperature: Celsius
        /// Dew Point: Celsius
        /// Wind Speed: Kilometers per hour
        /// Pressure: Hectopascals (equivalent to millibars)
        /// Visibility: Kilometers
        /// </para>
        /// </summary>
        CA,

        /// <summary>
        /// UK units of measurement. The same as SI, but with miles per
        /// hour used for Wind Speed.
        /// <para>
        /// Summaries containing temperature or snow accumulation units have
        /// their values in degrees Celsius or centimeters (respectively).
        /// Nearest storm distance: Kilometers
        /// Precipitation intensity: Millimeters per hour
        /// Precipitation accumulation: Centimeters
        /// Temperature: Celsius
        /// Dew Point: Celsius
        /// Wind Speed: Miles per hour
        /// Pressure: Hectopascals (equivalent to millibars)
        /// Visibility: Kilometers
        /// </para>
        /// </summary>
        UK,

        /// <summary>
        /// UK units of measurement. The same as SI, except that 
        /// nearestStormDistance and visibility are in miles and windSpeed 
        /// is in miles per hour
        /// <para>
        /// Summaries containing temperature or snow accumulation units have
        /// their values in degrees Celsius or centimeters (respectively).
        /// Nearest storm distance: Miles
        /// Precipitation intensity: Millimeters per hour
        /// Precipitation accumulation: Centimeters
        /// Temperature: Celsius
        /// Dew Point: Celsius
        /// Wind Speed: Miles per hour
        /// Pressure: Hectopascals (equivalent to millibars)
        /// Visibility: Miles
        /// </para>
        /// </summary>
        UK2,

        /// <summary>
        ///     Automatically choose units of measurement based on geographic location.
        /// </summary>
        Auto = Unit.Auto
    }

    /// <summary>
    /// Data blocks that can have their ranges extended.
    /// </summary>
    public enum Extend
    {
        /// <summary>
        /// Extends the hourly forecast block to the next seven days from just the next two.
        /// <para>Ignored when making time machine requests.</para>
        /// </summary>
        Hourly
    }

    /// <summary>
    /// Extension methods for the request parameter enumerations.
    /// </summary>
    public static class ParameterExtensions
    {
        /// <summary>
        /// Gives the Forecast Service-friendly name for
        /// this parameter.
        /// </summary>
        /// <param name="self">
        /// The parameter to convert.
        /// </param>
        /// <returns>
        /// The service-friendly <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the parameter does not have a corresponding
        /// friendly name.
        /// </exception>
        public static string ToValue(this DSUnit self)
        {
            switch (self)
            {
                case DSUnit.US:
                    return "us";
                case DSUnit.SI:
                    return "si";
                case DSUnit.CA:
                    return "ca";
                case DSUnit.UK:
                    return "uk";
                case DSUnit.UK2:
                    return "uk2";
                case DSUnit.Auto:
                    return "auto";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gives the Forecast Service-friendly name for
        /// this parameter.
        /// </summary>
        /// <param name="self">
        /// The parameter to convert.
        /// </param>
        /// <returns>
        /// The service-friendly <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the parameter does not have a corresponding
        /// friendly name.
        /// </exception>
        public static string ToValue(this Extend self)
        {
            switch (self)
            {
                case Extend.Hourly:
                    return "hourly";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}