using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace ValueObjectTests.Fixtures;

public class SqlServerFixture : IAsyncLifetime
{
    private MsSqlContainer Container { get; set; } = null!;
    public DbContextOptions<ProductDbContext> Options { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        Container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

        await Container.StartAsync();

        var connectionString = Container.GetConnectionString();

        Options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseSqlServer(connectionString, null)
            .Options;

        // Create database schema
        await using var db = new ProductDbContext(Options);
        await db.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Container.StopAsync();
        await Container.DisposeAsync();
    }
}