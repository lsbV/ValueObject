using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace MongoDbExample;

public class ObjectIdJsonConverter : JsonConverter<ObjectId>
{
    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();
        return stringValue is not null
            ? ObjectId.Parse(stringValue)
            : throw new JsonException("Expected a string value for ObjectId.");
    }

    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}