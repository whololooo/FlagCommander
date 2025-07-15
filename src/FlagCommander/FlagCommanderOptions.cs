using FlagCommander.Persistence;
using FlagCommander.Persistence.Repositories.Resp;
using FlagCommander.Persistence.Repositories.Sql;
using Microsoft.Extensions.Options;

namespace FlagCommander;

public class FlagCommanderOptions : IOptions<FlagCommanderOptions>
{
    // Default to in-memory repository
    internal IRepository? Repository { get; private set; } = new SqlLiteInMemoryRepository();

    public FlagCommanderOptions Value => this;

    public FlagCommanderOptions UsePostgres(string connectionString)
    {
        Repository = new PostgresRepository(connectionString);
        return this;
    }
    
    public FlagCommanderOptions UseMysql(string connectionString)
    {
        Repository = new MysqlRepository(connectionString);
        return this;
    }
    
    public FlagCommanderOptions UseMssql(string connectionString)
    {
        Repository = new MssqlRepository(connectionString);
        return this;
    }

    public FlagCommanderOptions Redis(string connectionString)
    {
        Repository = new RedisRepository(connectionString);
        return this;
    }
    
    public FlagCommanderOptions UseInMemoryRepository()
    {
        Repository = new SqlLiteInMemoryRepository();
        return this;
    }
}