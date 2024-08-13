using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer_Map
{
    public class WebCommandData
    {
        public string command;
        public Object data;

        public WebCommandData(string command, Object data)
        {
            this.command = command;
            this.data = data;
        }
    }

    internal class WebCommandConverter : JsonConverter<WebCommandData>
    {
     

        public override WebCommandData ReadJson(JsonReader reader, Type objectType, WebCommandData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

     
        public override void WriteJson(JsonWriter writer, WebCommandData value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("command");
            writer.WriteValue(value.command);
            writer.WritePropertyName("data");
            serializer.Serialize(writer, value.data);
            writer.WriteEndObject();
        }
    }
}
