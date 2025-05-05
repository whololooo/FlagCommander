using FlagCommander.Persistence.Repositories.Sql;
using Testcontainers.MySql;

namespace FlagCommander.Persistence.Test;

public class MysqlRepositoryTests : SqlRepositoryBaseTests, IAsyncLifetime
{
    private const string DbName = "testdb";
    private const string UserName = "postgres";
    private const string Password = "postgres";
    
    private readonly MySqlContainer _mysql = new MySqlBuilder()
        .WithImage("mysql:latest")
        .WithDatabase(DbName)
        .WithUsername(UserName)
        .WithPassword(Password)
        .Build();
    
    public async Task InitializeAsync()
    {
        await _mysql.StartAsync();
        Repository = new MysqlRepository(_mysql.GetConnectionString());
    }

    public Task DisposeAsync()
    {
        return _mysql.DisposeAsync().AsTask();
    }
}
