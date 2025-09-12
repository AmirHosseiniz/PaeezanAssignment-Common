using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaeezanAssignment_Server.Common.Socket.Room.Commands.Base;

namespace PaeezanAssignment_Server.Common.Socket;

public class GameCommandSerializer : JsonConverter
{
    private readonly string[] m_SubTypes;

    public GameCommandSerializer(string[] subTypes) : base()
    {
        m_SubTypes = subTypes;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer){}

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);
        if (jo.ContainsKey("$type"))
        {
            var value = jo["$type"]?.ToString(Formatting.Indented);
            if (value != null)
            {
                for (int i = 0; i < m_SubTypes.Length; i++)
                {
                    if (value.Contains(m_SubTypes[i]))
                    {
                        var type = Type.GetType(m_SubTypes[i]);
                        if (type != null)
                        {
                            return jo.ToObject(type, serializer);
                        }
                    }
                }
            }
        }

        throw new Exception("json does not have the $type property, TypeNameHandling must be enabled in serializer");
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(GameCommand);
    }

    public override bool CanWrite => false;
}