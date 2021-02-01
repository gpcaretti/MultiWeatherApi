using System.IO;
using MultiWeatherApi;
using Newtonsoft.Json;

namespace Helpers {

    /// <summary>
    ///     Just a wrapper to access protected methods
    /// </summary>
    public class WeatherServiceBase_Wrapper : WeatherServiceBase {

        public WeatherServiceBase_Wrapper() : base("fake_key") {
        }

        /// <summary>Just to test protected method</summary>
        public TResult ParseJsonFromStream_Wrapper<TResult>(Stream jsonStream, params JsonConverter[] customConverters) {
            return this.ParseJsonFromStream<TResult>(jsonStream, customConverters);
        }

    }
}