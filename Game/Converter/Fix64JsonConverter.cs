using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using PaeezanAssignment_Server.Common.Game.Physics;

public sealed class Fix64JsonConverter : JsonConverter<Fix64>
{
    public override Fix64 ReadJson(JsonReader reader, Type objectType, Fix64 existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return Fix64.Zero;
        if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
        {
            var d = Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture);
            return Fix64.FromDouble(d);
        }

        if (reader.TokenType == JsonToken.String)
        {
            var s = (string)reader.Value;
            if (string.IsNullOrWhiteSpace(s)) return Fix64.Zero;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                return Fix64.FromDouble(d);
        }

        // Fallback: try JValue to double
        var jv = JToken.ReadFrom(reader);
        if (jv.Type == JTokenType.Float || jv.Type == JTokenType.Integer)
            return Fix64.FromDouble(jv.Value<double>());
        if (jv.Type == JTokenType.String && double.TryParse(jv.Value<string>(), NumberStyles.Float,
                CultureInfo.InvariantCulture, out var ds))
            return Fix64.FromDouble(ds);
        throw new JsonSerializationException($"Cannot convert token {reader.TokenType} to Fix64");
    }


    public override void WriteJson(JsonWriter writer, Fix64 value, JsonSerializer serializer)
    {
        // Convert Fix64 to a double via raw ratio to preserve scale without reflection
        double d = (double)value.ToRaw() / (double)Fix64.One.ToRaw();
        writer.WriteValue(d);
    }
}