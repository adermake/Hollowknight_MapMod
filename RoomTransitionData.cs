using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer_Map
{
    public class RoomTransitionData 
    {
        public RoomData fromRoom;
        public RoomData toRoom;
        public GateData fromGate;
        public GateData toGate;

    }

    internal class RoomTransitionConverter : JsonConverter<RoomTransitionData>
    {
        public override RoomTransitionData ReadJson(JsonReader reader, Type objectType, RoomTransitionData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, RoomTransitionData value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("fromRoom");
            serializer.Serialize(writer, value.fromRoom);

            writer.WritePropertyName("toRoom");
            serializer.Serialize(writer, value.toRoom);

            writer.WritePropertyName("fromGate");
            serializer.Serialize(writer, value.fromGate);

            writer.WritePropertyName("toGate");
            serializer.Serialize(writer, value.toGate);

            writer.WriteEndObject();
        }
    }
}
