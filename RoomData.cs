using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer_Map
{
    public class RoomData
    {
        public string roomName;
        public float width;
        public float height;
        public float mapPosX = -1;
        public float mapPosY = -1;
        public List<GateData> gates = new List<GateData>();

    }

    internal class RoomConverter : JsonConverter<RoomData>
    {
   

        public override RoomData ReadJson(JsonReader reader, Type objectType, RoomData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }


        public override void WriteJson(JsonWriter writer, RoomData value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("roomName");
            writer.WriteValue(value.roomName);
            writer.WritePropertyName("width");
            writer.WriteValue(value.width);
            writer.WritePropertyName("height");
            writer.WriteValue(value.height);
            writer.WritePropertyName("mapPosX");
            writer.WriteValue(value.mapPosX);
            writer.WritePropertyName("mapPosY");
            writer.WriteValue(value.mapPosY);


            writer.WriteEndObject();
        }
    }
}
