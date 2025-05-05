using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace FlagCommander.Persistence.Repositories.Sql;

public class MsSqlRepository : SqlRepositoryBase
{
    public MsSqlRepository(string connectionString) : base(connectionString, false)
    {
        var initTask = this.Init();
        Task.Run(() => initTask).GetAwaiter().GetResult();
    }

    protected override DbConnection DbConnection => new SqlConnection(ConnectionString);
    
    private new async Task Init()
    {
        await using var connection = DbConnection;
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = @"
IF NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__flag_commander_flags')
BEGIN
    CREATE TABLE __flag_commander_flags
    (name VARCHAR(255) PRIMARY KEY, percentage_of_time INTEGER DEFAULT 100, percentage_of_actors INTEGER DEFAULT 100, enabled INTEGER DEFAULT 1);
END

IF NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = '__flag_commander_actors')
BEGIN
    CREATE TABLE __flag_commander_actors
    (flag_name VARCHAR(255), actor_id VARCHAR(255), PRIMARY KEY (flag_name, actor_id), FOREIGN KEY (flag_name) REFERENCES __flag_commander_flags(name) ON DELETE CASCADE);
END        
";
        await command.ExecuteNonQueryAsync();
    }
}