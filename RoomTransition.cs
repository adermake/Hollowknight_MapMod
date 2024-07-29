using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer_Map
{
    public class RoomTransition 
    {
        public string fromRoom;
        public string toRoom;
        public string fromGate;
        public string toGate;
        public float toRoomWidth;
        public float toRoomHeight;

    }

    internal class RoomTransitionConverter : JsonConverter<RoomTransition>
    {
        public override RoomTransition ReadJson(JsonReader reader, Type objectType, RoomTransition existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, RoomTransition value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("fromRoom");
            writer.WriteValue(value.fromRoom);
            writer.WritePropertyName("toRoom");
            writer.WriteValue(value.toRoom);
            writer.WritePropertyName("fromGate");
            writer.WriteValue(value.fromGate);
            writer.WritePropertyName("toGate");
            writer.WriteValue(value.toGate);
            writer.WritePropertyName("toRoomWidth");
            writer.WriteValue(value.toRoomWidth);
            writer.WritePropertyName("toRoomHeight");
            writer.WriteValue(value.toRoomHeight);
            writer.WriteEndObject();
        }
    }
}
