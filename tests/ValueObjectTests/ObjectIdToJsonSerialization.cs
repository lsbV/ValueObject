using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using ValueObject.Core;

namespace ValueObjectTests;

public class ObjectIdToJsonSerialization
{
    [Fact]
    public void ObjectId_Should_Serialize_To_Json_String()
    {
        // Arrange
        var originalObjectId = ObjectId.GenerateNewId();
        var cityId = new CityId(originalObjectId);
        var options = new JsonSerializerOptions() { Converters = { new ObjectIdJsonConverter() } };

        // Act
        var json = JsonSerializer.Serialize(cityId, options);

        // Assert
        Assert.NotNull(json);
        Assert.Contains(originalObjectId.ToString(), json);
    }
}

public partial record CityId(ObjectId Value) : IValueObject<ObjectId>;

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