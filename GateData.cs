using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer_Map
{
    public class GateData
    {
        public string gateName;
        public float gateDirX;
        public float gateDirY;
    }
    internal class GateConverter : JsonConverter<GateData>
    {


    
        public override GateData ReadJson(JsonReader reader, Type objectType, GateData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }


        public override void WriteJson(JsonWriter writer, GateData value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("gateName");
            writer.WriteValue(value.gateName);
            writer.WritePropertyName("gateDirX");
            writer.WriteValue(value.gateDirX);
            writer.WritePropertyName("gateDirY");
            writer.WriteValue(value.gateDirY);


            writer.WriteEndObject();
        }
    }
}
