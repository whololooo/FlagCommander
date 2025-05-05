using System.Data.Common;
using FlagCommander.Persistence.Models;

namespace FlagCommander.Persistence.Repositories.Sql;

public abstract class SqlRepositoryBase : IRepository
{
    protected string ConnectionString { get; }
    protected abstract DbConnection DbConnection { get; }

    protected SqlRepositoryBase(string connectionString, bool callInit = true)
    {
        ConnectionString = connectionString;
        if (callInit)
        {
            var initTask = Init();
            Task.Run(() => initTask).GetAwaiter().GetResult();
        }
    }

    protected async Task Init()
    {
        await using var connection = DbConnection;
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS __flag_commander_flags 
(name VARCHAR(255) PRIMARY KEY, percentage_of_time INTEGER DEFAULT 100, percentage_of_actors INTEGER DEFAULT 100, enabled INTEGER DEFAULT 1);

CREATE TABLE IF NOT EXISTS __flag_commander_actors 
(flag_name VARCHAR(255), actor_id VARCHAR(255), PRIMARY KEY (flag_name, actor_id), FOREIGN KEY (flag_name) REFERENCES __flag_commander_flags(name) ON DELETE CASCADE);
        ";
        await command.ExecuteNonQueryAsync();
    }

    public virtual async Task<Flag?> GetAsync(string featureName)
    {
        await using var connection = DbConnection;
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        
        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.DbType = System.Data.DbType.String;
        nameParam.Value = featureName;
        command.Parameters.Add(nameParam);
        
        command.CommandText = @"
select name, percentage_of_time, percentage_of_actors from __flag_commander_flags where name = @name and enabled = 1;
        ";

        Flag? flag = null;
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            flag = new Flag
            {
                Name = reader.GetString(0),
                PercentageOfTime = reader.GetInt32(1),
                PercentageOfActors = reader.GetInt32(2)
            };
            break;
        }

        await reader.CloseAsync();
        
        if (flag is null)
            return null;

        command.CommandText = @"
select actor_id from __flag_commander_actors where flag_name = @name
        ";
        
        var actorIds = new List<string>();
        await using var actorReader = await command.ExecuteReaderAsync();

        while (await actorReader.ReadAsync())
        {
            actorIds.Add(actorReader.GetString(0));
        }
        
        flag.ActorIds = actorIds;

        return flag;
    }

    public virtual async Task EnableAsync(string featureName)
    {
        await using var connection = DbConnection;
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();

        command.Transaction = await connection.BeginTransactionAsync();
        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.DbType = System.Data.DbType.String;
        nameParam.Value = featureName;
        command.Parameters.Add(nameParam);
        
        command.CommandText = @"
select count(*) from __flag_commander_flags where name = @name;
        ";
        
        var count = await command.ExecuteScalarAsync();
        if (count?.ToString() != null && int.Parse(count.ToString() ?? "0") != 0)
        {
            command.CommandText = @"
update __flag_commander_flags set enabled = 1 where name = @name;
        ";
        }
        else
        {
            command.CommandText = @"
insert into __flag_commander_flags (name) values (@name);
        ";
        }

        await command.ExecuteNonQueryAsync();
        await command.Transaction.CommitAsync();
    }

    public virtual async Task SetPercentageOfTimeAsync(string featureName, int percentage)
    {
        await using var connection = DbConnection;
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.Transaction = await connection.BeginTransactionAsync();
        
        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.DbType = System.Data.DbType.String;
        nameParam.Value = featureName;
        command.Parameters.Add(nameParam);
        
        var percentageParam = command.CreateParameter();
        percentageParam.ParameterName = "@percentage";
        percentageParam.DbType = System.Data.DbType.Int32;
        percentageParam.Value = percentage;
        command.Parameters.Add(percentageParam);
        
        command.CommandText = @"
update __flag_commander_flags set percentage_of_time = @percentage where name = @name;
        ";
        
        await command.ExecuteNonQueryAsync();
        await command.Transaction.CommitAsync();
    }

    public virtual async Task SetPercentageOfActorsAsync(string featureName, int percentage)
    {
        await using var connection = DbConnection;
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.Transaction = await connection.BeginTransactionAsync();
        
        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.DbType = System.Data.DbType.String;
        nameParam.Value = featureName;
        command.Parameters.Add(nameParam);
        
        var percentageParam = command.CreateParameter();
        percentageParam.ParameterName = "@percentage";
        percentageParam.DbType = System.Data.DbType.Int32;
        percentageParam.Value = percentage;
        command.Parameters.Add(percentageParam);
        
        command.CommandText = @"
update __flag_commander_flags set percentage_of_actors = @percentage where name = @name;
        ";
        
        await command.ExecuteNonQueryAsync();
        await command.Transaction.CommitAsync();
    }

    public virtual async Task AddActorAsync(string featureName, string actorId)
    {
        await using var connection = DbConnection;
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.Transaction = await connection.BeginTransactionAsync();
        
        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.DbType = System.Data.DbType.String;
        nameParam.Value = featureName;
        command.Parameters.Add(nameParam);
        
        var actorParam = command.CreateParameter();
        actorParam.ParameterName = "@actor";
        actorParam.DbType = System.Data.DbType.String;
        actorParam.Value = actorId;
        command.Parameters.Add(actorParam);
        
        command.CommandText = @"
select count(*) from __flag_commander_actors where flag_name = @name and actor_id = @actor;
        ";
        
        var count = await command.ExecuteScalarAsync();
        if (count?.ToString() != null && int.Parse(count.ToString() ?? "0") == 0)
        {
            command.CommandText = @"
insert into __flag_commander_actors (flag_name, actor_id) values (@name, @actor);
        ";
        }

        await command.ExecuteNonQueryAsync();
        await command.Transaction.CommitAsync();
    }

    public virtual async Task DisableAsync(string featureName)
    {
        await using var connection = DbConnection;
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        
        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.DbType = System.Data.DbType.String;
        nameParam.Value = featureName;
        command.Parameters.Add(nameParam);
        
        command.CommandText = @"
update  __flag_commander_flags set enabled = 0 where name = @name;
        ";
        
        await command.ExecuteNonQueryAsync();
    }

    public virtual async Task DeleteAsync(string featureName)
    {
        await using var connection = DbConnection;
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.Transaction = await connection.BeginTransactionAsync();
        
        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.DbType = System.Data.DbType.String;
        nameParam.Value = featureName;
        command.Parameters.Add(nameParam);
        
        command.CommandText = @"
delete from __flag_commander_actors where flag_name = @name;
delete from __flag_commander_flags where name = @name;
        ";
        
        await command.ExecuteNonQueryAsync();
        await command.Transaction.CommitAsync();
    }

    public virtual async Task DeleteActorAsync(string featureName, string actorId)
    {
        await using var connection = DbConnection;
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.Transaction = await connection.BeginTransactionAsync();
        
        var nameParam = command.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.DbType = System.Data.DbType.String;
        nameParam.Value = featureName;
        command.Parameters.Add(nameParam);
        
        var actorParam = command.CreateParameter();
        actorParam.ParameterName = "@actor";
        actorParam.DbType = System.Data.DbType.String;
        actorParam.Value = actorId;
        command.Parameters.Add(actorParam);
        
        command.CommandText = @"
delete from __flag_commander_actors where flag_name = @name and actor_id = @actor;
        ";
        
        await command.ExecuteNonQueryAsync();
        await command.Transaction.CommitAsync();
    }
}