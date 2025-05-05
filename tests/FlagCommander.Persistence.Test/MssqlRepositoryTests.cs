using FlagCommander.Persistence.Repositories.Sql;
using Testcontainers.MsSql;

namespace FlagCommander.Persistence.Test;

public class MssqlRepositoryTests : SqlRepositoryBaseTests, IAsyncLifetime
{
    private readonly MsSqlContainer _mssql = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();
    
    public async Task InitializeAsync()
    {
        await _mssql.StartAsync();
        Repository = new MsSqlRepository(_mssql.GetConnectionString());
    }

    public Task DisposeAsync()
    {
        return _mssql.DisposeAsync().AsTask();
    }
}
