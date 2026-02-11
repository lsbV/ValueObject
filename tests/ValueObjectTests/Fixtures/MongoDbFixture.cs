using MongoDB.Driver;
using Testcontainers.MongoDb;
using ValueObject.Core;

namespace ValueObjectTests.Fixtures;

public class MongoDbFixture : IAsyncLifetime
{
    private MongoDbContainer Container { get; set; } = null!;
    public IMongoClient Client { get; private set; } = null!;
    public string DatabaseName { get; } = "ValueObjectTestDb";

    public async ValueTask InitializeAsync()
    {
        Container = new MongoDbBuilder("mongo:8.0").Build();

        await Container.StartAsync();

        var connectionString = Container.GetConnectionString();
        Client = new MongoClient(connectionString);
        MongoClassMaps.RegisterAll();
    }

    public async ValueTask DisposeAsync()
    {
        await Container.StopAsync();
        await Container.DisposeAsync();
    }
}


