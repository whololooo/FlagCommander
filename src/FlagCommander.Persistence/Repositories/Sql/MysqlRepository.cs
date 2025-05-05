using System.Data.Common;
using MySql.Data.MySqlClient;

namespace FlagCommander.Persistence.Repositories.Sql;

public class MysqlRepository : SqlRepositoryBase
{
    public MysqlRepository(string connectionString) : base(connectionString)
    {
    }

    protected override DbConnection DbConnection => new MySqlConnection(ConnectionString);
}