using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MultiWeatherApi.DarkSky.Model
{
    /// <summary>
    /// Metadata associated with a forecast.
    /// </summary>
    [Serializable]
    public class Flags
    {
        /// <summary>
        ///     the IDs for each data source used to provide info for this forecast.
        /// </summary>
        [JsonProperty("sources")]
        public IList<string> Sources { get; set; }

        /// <summary>
        ///     the IDs for each radar station used to provide info for this forecast.
        /// </summary>
        [JsonProperty("darksky-stations")]
        public IList<string> DarkSkyStations { get; set; } 

        /// <summary>
        ///     the IDs for each DataPoint station used to provide info for this forecast.
        /// </summary>
        [JsonProperty("datapoint-stations")]
        public IList<string> DataPointStations { get; set; } 

        /// <summary>
        ///     the IDs for each ISD station used to provide info for this forecast.
        /// </summary>
        [JsonProperty("isd-stations")]
        public IList<string> IsdStations { get; set; } 

        /// <summary>
        ///     the IDs for each LAMP station used to provide info for this forecast.
        /// </summary>
        [JsonProperty("lamp-stations")]
        public IList<string> LampStations { get; set; } 

        /// <summary>
        ///     the IDs for each METAR station used to provide info for this forecast.
        /// </summary>
        [JsonProperty("metar-stations")]
        public IList<string> MetarStations { get; set; }

        /// <summary>
        ///     the IDs for each MADIS station used to provide info for this forecast.
        /// </summary>
        [JsonProperty("madis-stations")]
        public IList<string> MadisStations { get; set; } 

        /// <summary>
        ///     the met.no license. If this is present, data from api.met.no was used to provide info for this forecast.
        /// </summary>
        [JsonProperty("metno-license")]
        public string MetnoLicense { get; set; }

        /// <summary>
        ///     the type of units that are used for the data in this forecast.
        /// </summary>
        [JsonProperty("units")]
        public string Units { get; set; }
    }
}
