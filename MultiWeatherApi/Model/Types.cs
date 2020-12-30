namespace MultiWeatherApi.Model {

    /// <summary>
    ///     Units of measurement supported by the Forecast service.
    /// </summary>
    public enum Unit {

        /// <summary>
        ///     US units of measurement.
        /// <para>
        /// Summaries containing temperature or snow accumulation units have
        /// their values in degrees Fahrenheit or inches (respectively).
        /// Nearest storm distance: Miles
        /// Precipitation intensity: Inches per hour
        /// Precipitation accumulation: Inches
        /// Temperature: Fahrenheit
        /// Dew Point: Fahrenheit
        /// Wind Speed: Miles per hour
        /// Pressure: Millibars
        /// Visibility: Miles
        /// </para>
        /// </summary>
        US,

        /// <summary>
        ///     SI units of measurement.
        /// <para>
        /// Summaries containing temperature or snow accumulation units have
        /// their values in degrees Celsius or centimeters (respectively).
        /// Nearest storm distance: Kilometers
        /// Precipitation intensity: Millimeters per hour
        /// Precipitation accumulation: Centimeters
        /// Temperature: Celsius
        /// Dew Point: Celsius
        /// Wind Speed: Meters per second
        /// Pressure: Hectopascals (equivalent to millibars)
        /// Visibility: Kilometers
        /// </para>
        /// </summary>
        SI,

        /// <summary>
        ///     Automatically choose units of measurement based on geographic location.
        /// </summary>
        Auto
    }

}
