using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MultiWeatherApi.DarkSky.Model
{
    /// <summary>
    /// A day-by-day forecast.
    /// </summary>
    [Serializable, DataContract]
    public class DailyForecast
    {
        /// <summary>
        /// Gets or sets a human-readable summary of the forecast.
        /// </summary>
        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets machine-readable text that can be used to select an icon to display.
        /// </summary>
        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the individual days that make up this forecast.
        /// </summary>
        [DataMember(Name = "data")]
        public IList<DayDataPoint> Days { get; set; }
    }
}
