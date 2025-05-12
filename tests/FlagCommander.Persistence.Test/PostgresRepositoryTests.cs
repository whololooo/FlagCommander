using FlagCommander.Persistence.Repositories.Sql;
using Testcontainers.PostgreSql;

namespace FlagCommander.Persistence.Test;

public class PostgresRepositoryTests : RepositoryBaseTests, IAsyncLifetime
{
    private const string DbName = "testdb";
    private const string UserName = "postgres";
    private const string Password = "postgres";
    
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase(DbName)
        .WithUsername(UserName)
        .WithPassword(Password)
        .Build();
    
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Repository = new PostgresRepository(_postgres.GetConnectionString());
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }
}
