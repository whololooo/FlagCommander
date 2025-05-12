using FlagCommander.Persistence.Repositories.Resp;
using Testcontainers.Redis;

namespace FlagCommander.Persistence.Test;

public class RedisRepositoryTests : RepositoryBaseTests, IAsyncLifetime
{
    private readonly RedisContainer _redis = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();
    
    public async Task InitializeAsync()
    {
        await _redis.StartAsync();
        Repository = new RedisRepository(_redis.GetConnectionString());
    }

    public Task DisposeAsync()
    {
        return _redis.DisposeAsync().AsTask();
    }
}