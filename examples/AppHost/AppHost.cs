using Aspire.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

var efCoreExample = builder.AddProject<EfCoreExample>("ef-core-example")
    .WithReference(postgres)
    .WaitFor(postgres);


var mongoDb = builder.AddMongoDB("mongodb");
var mongoDbExample = builder.AddProject<MongoDbExample>("mongo-db-example")
    .WithReference(mongoDb)
    .WaitFor(mongoDb);

builder.Build().Run();