using MongoDB.Bson;
using ValueObject.Core;

namespace ValueObjectTests;

public readonly partial record struct UserId(ObjectId Value) : IValueObject<ObjectId>;

public class MongoDbSerializerTests
{
    [Fact]
    public void UserId_Serializer_Should_Be_Generated()
    {
        var userId = new UserId(ObjectId.GenerateNewId());
        var serializer = new UserIdSerializer();

        Assert.NotNull(serializer);
    }

    [Fact]
    public void UserId_Should_Serialize_And_Deserialize()
    {
        var originalId = ObjectId.GenerateNewId();
        var userId = new UserId(originalId);
        var serializer = new UserIdSerializer();

        using var memoryStream = new System.IO.MemoryStream();
        using var writer = new MongoDB.Bson.IO.BsonBinaryWriter(memoryStream);
        var context = MongoDB.Bson.Serialization.BsonSerializationContext.CreateRoot(writer);

        serializer.Serialize(context, new MongoDB.Bson.Serialization.BsonSerializationArgs(), userId);
        writer.Flush();
        memoryStream.Position = 0;

        using var reader = new MongoDB.Bson.IO.BsonBinaryReader(memoryStream);
        var deserializeContext = MongoDB.Bson.Serialization.BsonDeserializationContext.CreateRoot(reader);
        var deserializedUserId = serializer.Deserialize(deserializeContext, new MongoDB.Bson.Serialization.BsonDeserializationArgs());

        Assert.Equal(userId, deserializedUserId);
        Assert.Equal(originalId, deserializedUserId.Value);
    }
}

