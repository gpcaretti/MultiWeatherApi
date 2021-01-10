using System;
using System.Collections.Generic;
using MultiWeatherApi.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MultiWeatherApi.OpenWeather.Helpers {

    /// <summary>
    ///     Converter that handle both a single float/float value, as well as a Alert object
    /// </summary>
    public class MyAlertConverter : JsonConverter {

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Alert);
        }

        /// <summary>
        ///     Deserialize an OpenWeather Alert into a Multiweather Alert object
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            // create a temperature object and save the values on daily, min, etc. If the value is just a float, save it into Daily
            Alert alert = new Alert();
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object) {
                if (token["start"] != null) alert.StartUnixTime = (int)token["start"];
                if (token["end"] != null) alert.ExpiresUnixTime = (int)token["end"];
                if (token["sender_name"] != null) alert.Sender = (string)token["sender_name"];
                if (token["event"] != null) alert.Title = (string)token["event"];
                if (token["description"] != null) alert.Description = (string)token["description"];
                if (token["url"] != null) alert.Uri = (string)token["url"];
            //} else {
            //    alert.Daily = (float?)token;
            }
            return alert;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }

}
