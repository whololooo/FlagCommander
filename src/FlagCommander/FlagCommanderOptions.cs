using FlagCommander.Persistence;
using Microsoft.Extensions.Options;

namespace FlagCommander;

public class FlagCommanderOptions : IOptions<FlagCommanderOptions>
{
    // Default to in-memory repository
    internal IRepository? Repository { get; private set; } = new Persistence.Repositories.Sql.SqlLiteInMemoryRepository();

    public FlagCommanderOptions Value => this;

    public FlagCommanderOptions UsePostgres(string connectionString)
    {
        Repository = new Persistence.Repositories.Sql.PostgresRepository(connectionString);
        return this;
    }
    
    public FlagCommanderOptions UseMysql(string connectionString)
    {
        Repository = new Persistence.Repositories.Sql.MysqlRepository(connectionString);
        return this;
    }
    
    public FlagCommanderOptions UseMssql(string connectionString)
    {
        Repository = new Persistence.Repositories.Sql.MssqlRepository(connectionString);
        return this;
    }
    
    public FlagCommanderOptions UseInMemoryRepository()
    {
        Repository = new Persistence.Repositories.Sql.SqlLiteInMemoryRepository();
        return this;
    }
}