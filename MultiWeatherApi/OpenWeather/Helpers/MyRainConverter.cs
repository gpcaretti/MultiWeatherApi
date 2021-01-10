using System;
using MultiWeatherApi.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MultiWeatherApi.OpenWeather.Helpers {

    public class MyRainConverter : JsonConverter {

        public override bool CanConvert(Type objectType) {
            return true || objectType == typeof(Temperature);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            // create a temperature object and save the values on daily, min, etc. If the value is just a float, save it into Daily
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
