using System;
using Newtonsoft.Json;

namespace MultiWeatherApi.Model {

    /// <summary>
    ///     Temperature of the time frame they relate to
    /// </summary>
    [Obsolete]
    [Serializable]
    public class TemperatureOLD {
        private double? _min;
        private double? _max;
        private double? _apparentDay;
        private double? _apparentMax;
        private double? _apparentMin;
        private double? _apparentNight;
        private double? _apparentEvening;
        private double? _apparentMorning;

        /// <summary>Daily temperature</summary>
        [JsonProperty("day")]
        public double? Daily { get; set; }

        /// <summary>Morning temperature</summary>
        [JsonProperty("morn")]
        public double? Morning { get; set; }

        /// <summary>Evening temperature</summary>
        [JsonProperty("eve")]
        public double? Evening { get; set; }

        /// <summary>Night temperature</summary>
        [JsonProperty("night")]
        public double? Night { get; set; }

        /// <summary>
        ///     Min temperature of the day. If not set, returns the min between night and daily temp
        /// </summary>
        [JsonProperty("min")]
        public double? Min {
            get => _min.HasValue ? _min : (Daily.HasValue || Night.HasValue) ? Math.Min(Daily.GetValueOrDefault(+1000), Night.GetValueOrDefault(+1000)) : (double?)null;
            set => _min = value;
        }

        /// <summary>
        ///     Max temperature of the day. If not set, returns the max between night and daily temp
        /// </summary>
        [JsonProperty("max")]
        public double? Max { 
            get => _max.HasValue ? _max : (Daily.HasValue || Night.HasValue) ? Math.Max(Daily.GetValueOrDefault(-273), Night.GetValueOrDefault(-273)) : (double?)null; 
            set => _max = value; 
        }

        /// <summary>Apparent daily temperature. If not set, returns the daily temperature.</summary>
        public double? ApparentDaily { get => _apparentDay ?? Daily; set => _apparentDay = value; }

        /// <summary>Apparent min temperature. If not set, returns the min temperature.</summary>
        public double? ApparentMin { get => _apparentMin ?? Min; set => _apparentMin = value; }

        /// <summary>Apparent max temperature. If not set, returns the max temperature.</summary>
        public double? ApparentMax { get => _apparentMax ?? Max; set => _apparentMax = value; }

        /// <summary>Apparent night temperature. If not set, returns the night temperature.</summary>
        public double? ApparentNight { get => _apparentNight ?? Night; set => _apparentNight = value; }

        /// <summary>Apparent evening temperature. If not set, returns the evening temperature.</summary>
        public double? ApparentEvening { get => _apparentEvening ?? Evening; set => _apparentEvening = value; }

        /// <summary>Apparent morning temperature. If not set, returns the morning temperature.</summary>
        public double? ApparentMorning { get => _apparentMorning ?? Morning; set => _apparentMorning = value; }

        /// <summary>Dew point</summary>
        public double DewPoint { get; set; }

        /// <summary>Humidity (percentage)</summary>
        public int Humidity { get; set; }
    }

}
