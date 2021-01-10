using System;
using System.Net;
using System.Runtime.Serialization;

namespace MultiWeatherApi {

    /// <summary>
    ///     TODO
    /// </summary>
    [Serializable]
    public class WeatherException : Exception {

        /// <summary>The api is empty</summary>
        public static readonly int EmptyApiKey = 100;

        /// <summary>Json content is not valid</summary>
        public static readonly int JsonUnsuccessful = 200;
        /// <summary>Error parsing or missing fields</summary>
        public static readonly int JsonParsingError = 201;

        /// <summary>Generic http error</summary>
        public static readonly int HttpError = (int)HttpStatusCode.BadRequest;
        /// <summary>Unauthorized http error</summary>
        public static readonly int HttpUnauthorized = (int)HttpStatusCode.Unauthorized;

        public int Code { get; }

        public WeatherException() {
        }

        public WeatherException(string message) : this(0, message) {
        }

        public WeatherException(int code, string message) : this(code, message, null) {
        }

        public WeatherException(string message, Exception innerException) : this(0, message, innerException) {
        }

        public WeatherException(int code, string message, Exception innerException) : base(message, innerException) {
            Code = code;
        }

        protected WeatherException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
