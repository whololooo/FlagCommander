using FlagCommander.Persistence.Repositories.Sql;

namespace FlagCommander.Persistence.Test;

public class SqlLiteInMemoryRepositoryTests : SqlRepositoryBaseTests, IAsyncDisposable
{
    public SqlLiteInMemoryRepositoryTests()
    {
        Repository = new SqlLiteInMemoryRepository();
    }

    public async ValueTask DisposeAsync()
    {
        await ((SqlLiteInMemoryRepository)Repository!).DisposeAsync();
    }
}
