using System.Data.Common;
using Npgsql;

namespace FlagCommander.Persistence.Repositories.Sql;

public class PostgresRepository : SqlRepositoryBase
{
    public PostgresRepository(string connectionString) : base(connectionString)
    {
    }

    protected override DbConnection DbConnection => new NpgsqlConnection(ConnectionString);
}