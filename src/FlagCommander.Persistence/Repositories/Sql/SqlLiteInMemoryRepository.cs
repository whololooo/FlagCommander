using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace FlagCommander.Persistence.Repositories.Sql;

public class SqlLiteInMemoryRepository : SqlRepositoryBase, IDisposable, IAsyncDisposable
{
    private const string SqlLiteConnectionString = "Data Source=InMemorySample;Mode=Memory;Cache=Shared";
    private DbConnection? keepAliveConnection;

    protected override DbConnection DbConnection => new SqliteConnection(ConnectionString);
    
    public SqlLiteInMemoryRepository() : base(SqlLiteConnectionString, false)
    {
        keepAliveConnection = new SqliteConnection(SqlLiteConnectionString);
        keepAliveConnection.Open();
        var initTask = Init();
        Task.Run(() => initTask).GetAwaiter().GetResult();
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