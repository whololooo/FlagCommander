using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace FlagCommander.Persistence.Repositories.Sql;

public class SqlLiteInMemoryRepository : SqlRepositoryBase, IDisposable, IAsyncDisposable
{
    private const string SqlLiteConnectionString = "Data Source=InMemorySample;Mode=Memory;Cache=Shared";
    private DbConnection? keepAliveConnection;

    protected override DbConnection DbConnection => new SqliteConnection(ConnectionString);
    
    public SqlLiteInMemoryRepository() : base(SqlLiteConnectionString)
    {
    }

    protected override async Task Init()
    {
        keepAliveConnection = new SqliteConnection(SqlLiteConnectionString);
        await keepAliveConnection.OpenAsync();
        
        await using var connection = DbConnection;
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS __flag_commander_flags 
(name VARCHAR(255) PRIMARY KEY, percentage_of_time INTEGER DEFAULT 100, percentage_of_actors INTEGER DEFAULT 100, enabled INTEGER DEFAULT 1);

CREATE INDEX IF NOT EXISTS __flag_commander_flags_enabled_index ON __flag_commander_flags (enabled);
CREATE INDEX IF NOT EXISTS __flag_commander_flags_name_index ON __flag_commander_flags (name);
CREATE INDEX IF NOT EXISTS __flag_commander_flags_name_and_enabled_index ON __flag_commander_flags (name, enabled);

CREATE TABLE IF NOT EXISTS __flag_commander_actors 
(flag_name VARCHAR(255), actor_id VARCHAR(255), PRIMARY KEY (flag_name, actor_id), FOREIGN KEY (flag_name) REFERENCES __flag_commander_flags(name) ON DELETE CASCADE);

CREATE INDEX IF NOT EXISTS __flag_commander_actors_flag_name_and_actor_id_index ON __flag_commander_actors (flag_name, actor_id);
";
        await command.ExecuteNonQueryAsync();
    }

    public void Dispose()
    {
        keepAliveConnection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (keepAliveConnection != null) await keepAliveConnection.DisposeAsync();
    }
}