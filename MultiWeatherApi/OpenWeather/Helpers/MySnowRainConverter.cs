using System;
using MultiWeatherApi.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MultiWeatherApi.OpenWeather.Helpers {

    /// <summary>
    ///     Manage snow/rain in the format of a float number of an object in the form {"1h": "float value"}
    /// </summary>
    public class MySnowRainConverter : JsonConverter {

        public override bool CanConvert(Type objectType) {
            return true || objectType == typeof(float);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            float? rain = null;
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object) {
                if (token["1h"] != null) rain = (float?)token["1h"];
            } else {
                rain = (float?)token;
            }
            return rain;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}
