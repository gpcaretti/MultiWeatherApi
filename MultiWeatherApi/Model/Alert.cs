using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MultiWeatherApi.Model {

    /// <summary>
    ///      An severe weather alert issued by a weather service for a particular location.
    /// </summary>
    [Serializable]
    public class Alert {

        /// <summary>
        ///     the moment in time at which this alert was issued.
        /// </summary>
        public DateTimeOffset StartTime {
            get => StartUnixTime.ToDateTimeOffset();
            set => StartUnixTime = value.ToUnixTime();
        }

        /// <summary>
        ///     the moment in time at which this alert is no longer valid.
        /// </summary>
        public DateTimeOffset ExpiresTime {
            get => ExpiresUnixTime.ToDateTimeOffset();
            set => ExpiresUnixTime = value.ToUnixTime();
        }


        /// <summary>
        ///     Gets the severity of this weather alert.
        /// </summary>
        [JsonIgnore]
        public Severity Severity {
            get => SeverityAsString.SeverityFromString(SeverityAsString);
            set => SeverityAsString = value.ToValue();
        }

        /// <summary>Possibile null name of the sender</summary>
        [JsonProperty("sender_name")]
        public string Sender { get; set; }

        /// <summary>
        ///     a short text summary of this alert.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     the regions covered by this alert.
        /// </summary>
        [JsonProperty("regions")]
        public IList<string> Regions { get; set; }

        /// <summary>
        ///     the text description of this alert.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        ///     a URI to the alert's details.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; }

        #region Internals

        /// <summary>
        ///     Unix time at which this alert was issued.
        /// </summary>
        [JsonProperty("time")]
        internal int StartUnixTime { get; set; }

        /// <summary>
        ///     Unix time at which this alert is no longer valid.
        /// </summary>
        [JsonProperty("expires")]
        internal int ExpiresUnixTime { get; set; }

        /// <summary>
        /// The severity of this alert, as a string.
        /// </summary>
        [JsonProperty("severity")]
        internal string SeverityAsString { get; set; }

        #endregion

    }
}
