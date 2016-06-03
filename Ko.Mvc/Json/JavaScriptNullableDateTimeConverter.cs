using Newtonsoft.Json.Converters;

namespace Ko.Mvc.Json
{
    public class JavaScriptNullableDateTimeConverter : JavaScriptDateTimeConverter
    {
        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                base.WriteJson(writer, value, serializer);                
            }
        }
    }
}