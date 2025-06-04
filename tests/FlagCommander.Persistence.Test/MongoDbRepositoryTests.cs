using FlagCommander.Persistence.Repositories.NoSql;
using Testcontainers.MongoDb;

namespace FlagCommander.Persistence.Test;

public class MongoDbRepositoryTests : RepositoryBaseTests, IAsyncLifetime
{
    private readonly MongoDbContainer _mongo = new MongoDbBuilder()
        .WithImage("mongo:latest")
        .Build();
    
    public async Task InitializeAsync()
    {
        await _mongo.StartAsync();
        Repository = new MongoDbRepository(_mongo.GetConnectionString());
    }

    public Task DisposeAsync()
    {
        return _mongo.DisposeAsync().AsTask();
    }
}