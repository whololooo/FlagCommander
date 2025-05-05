using FlagCommander.Persistence.Repositories.Sql;

namespace FlagCommander.Persistence.Test;

public class SqlLiteInMemoryRepositoryTests : SqlRepositoryBaseTests
{
    public SqlLiteInMemoryRepositoryTests()
    {
        Repository = new SqlLiteInMemoryRepository();
    }
}
