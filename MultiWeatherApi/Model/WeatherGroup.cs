using System;
using System.Collections.Generic;

namespace MultiWeatherApi.Model {

    /// <summary>
    ///     Current and forecast weather conditions for a particular location.
    /// </summary>
    [Serializable]
    public class WeatherGroup : List<Weather>, IWeatherData {

        /// <summary>the geo coordinates of this forecast</summary>
        public GeoCoordinates Coordinates { get; set; }

        /// <summary>the IANA time zone name for this location</summary>
        public string TimeZone { get; set; }

        /// <summary>the time zone offset, in hours from GMT.</summary>
        public float TimeZoneOffset { get; set; }

        /// <summary>Any weather alerts related to this location.</summary>
        public IList<Alert> Alerts { get; set; }

        /// <summary>
        ///     Empty group
        /// </summary>
        public WeatherGroup() : base() {
        }

        /// <summary>
        ///     Empty group with a specific initial capacity
        /// </summary>
        public WeatherGroup(int capacity) : base(capacity) {
        }

        /// <summary>
        ///     Initializes a new instance that contains elements copied from the specified collection and has 
        ///     sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        public WeatherGroup(IEnumerable<Weather> weathers) : base(weathers) { 
        }

    }
}
