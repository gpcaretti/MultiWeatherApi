﻿using System.IO;
using MultiWeatherApi;
using Newtonsoft.Json;

namespace GenericApi.Test {

    /// <summary>
    ///     Just a wrapper to access protected methods
    /// </summary>
    public class WrapperClassForTest : WeatherServiceBase {

        public WrapperClassForTest(string key) : base(key) {
        }

        /// <summary>Just to test protected method</summary>
        public TResult ParseJsonFromStream_Wrapper<TResult>(Stream jsonStream, params JsonConverter[] customConverters) {
            return this.ParseJsonFromStream<TResult>(jsonStream, customConverters);
        }

    }
}