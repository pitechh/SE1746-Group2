using Newtonsoft.Json;
using System.Globalization;

namespace Business.Extensions
{
    public class DateTimeConverter : JsonConverter
    {
        private const string Format = "yyyy-dd-M";
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null && objectType == typeof(DateTime?))
                return null;

            if (reader.TokenType == JsonToken.String)
            {
                var dateStr = (string)reader.Value;
                if (DateTime.TryParseExact(dateStr, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date;
                }
            }

            throw new JsonSerializationException($"Invalid date format. Expected format is {Format}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime date = (DateTime)value;
            writer.WriteValue(date.ToString(Format));
        }
    }
}
