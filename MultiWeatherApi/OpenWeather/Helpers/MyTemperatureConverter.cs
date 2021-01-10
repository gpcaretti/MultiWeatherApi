using System;
using MultiWeatherApi.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MultiWeatherApi.OpenWeather.Helpers {

    /// <summary>
    ///     Converter that handle both a single float/float value, as well as a Temperature object
    /// </summary>
    public class MyTemperatureConverter : JsonConverter {

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Temperature);
        }

        /// <summary>
        ///     Deserialize a float, float or object {min,max, day,night,morn,eve} into a temperature
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            // create a temperature object and save the values on daily, min, etc. If the value is just a float, save it into Daily
            Temperature temp = new Temperature();
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object) {
                if (token["day"] != null) temp.Daily = (float?)token["day"];
                if (token["night"] != null) temp.Night = (float?)token["night"];
                if (token["eve"] != null) temp.Evening = (float?)token["eve"];
                if (token["morn"] != null) temp.Morning = (float?)token["morn"];
                if (token["min"] != null) temp.Min = (float?)token["min"];
                if (token["max"] != null) temp.Max = (float?)token["max"];
            } else {
                temp.Daily = (float?)token;
            }
            return temp;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}
