using FlagCommander.Persistence.Repositories.Sql;

namespace FlagCommander.Persistence.Test;

public class SqlLiteInMemoryRepositoryTests : SqlRepositoryBaseTests, IAsyncLifetime
{
    public Task InitializeAsync()
    {
        Repository = new SqlLiteInMemoryRepository();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await ((SqlLiteInMemoryRepository)Repository!).DisposeAsync();
    }
}
