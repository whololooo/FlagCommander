namespace FlagCommander.Persistence.Repositories.NoSql;

public abstract class NoSqlRepositoryBase : RepositoryBase
{
    protected NoSqlRepositoryBase(string connectionString) : base(connectionString)
    {
    }
}