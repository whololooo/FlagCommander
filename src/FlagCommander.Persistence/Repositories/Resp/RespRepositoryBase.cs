using FlagCommander.Persistence.Models;
using StackExchange.Redis;

namespace FlagCommander.Persistence.Repositories.Resp;

public abstract class RespRepositoryBase : RepositoryBase, IRepository
{
    protected Task<ConnectionMultiplexer> Connection => ConnectionMultiplexer.ConnectAsync(ConnectionString);
    protected string KeyPrefix => "__flag_commander_";
    
    protected RespRepositoryBase(string connectionString) : base(connectionString)
    {
    }
    
    protected async Task<IDatabase> GetDatabaseAsync()
    {
        var connection = await Connection;
        return connection.GetDatabase();
    }

    public async Task<Flag?> GetAsync(string featureName)
    {
        featureName = $"{KeyPrefix}{featureName}";
        var database = await GetDatabaseAsync();
        var flagJson = (string?)await database.StringGetAsync(featureName);

        var flag = flagJson?.FlagFromJson();
        return flag?.IsEnabled == true ? flag : null;
    }

    public async Task<List<Flag>> GetFlagsAsync()
    {
        var endpoint = (await Connection).GetEndPoints().FirstOrDefault();
        if (endpoint is null)
            return [];
        
        var server = (await Connection).GetServer(endpoint);
        var keys = server.Keys(pattern: $"{KeyPrefix}*").ToList();
        var database = await GetDatabaseAsync();
        var result = new List<Flag>();
        foreach (var redisKey in keys)
        {
            var flagJson = (string?)await database.StringGetAsync(redisKey);
            var flag = flagJson?.FlagFromJson();
            if (flag is not null)
                result.Add(flag);
        }
        return result;
    }

    public async Task EnableAsync(string featureName)
    {
        var database = await GetDatabaseAsync();
        var featureNameWithPrefix = $"{KeyPrefix}{featureName}";
        
        var flagJson = (string?)await database.StringGetAsync(featureNameWithPrefix);
        var flag = flagJson is null 
            ? new Flag() { Name = featureName} 
            : flagJson.FlagFromJson() ?? new Flag() { Name = featureName };

        if (!flag.IsEnabled)
            flag.IsEnabled = true;
        
        var json = flag.ToJson();
        await database.StringSetAsync(featureNameWithPrefix, json);
    }

    public async Task SetPercentageOfTimeAsync(string featureName, int percentage)
    {
        featureName = $"{KeyPrefix}{featureName}";
        var database = await GetDatabaseAsync();
        var flagJson = (string?)await database.StringGetAsync(featureName);
        var flag = flagJson?.FlagFromJson();
        
        if (flag is null)
            return;

        flag.PercentageOfTime = percentage;
        await database.StringSetAsync(featureName, flag.ToJson());
    }

    public async Task SetPercentageOfActorsAsync(string featureName, int percentage)
    {
        featureName = $"{KeyPrefix}{featureName}";
        var database = await GetDatabaseAsync();
        var flagJson = (string?)await database.StringGetAsync(featureName);
        var flag = flagJson?.FlagFromJson();
        
        if (flag is null)
            return;

        flag.PercentageOfActors = percentage;
        await database.StringSetAsync(featureName, flag.ToJson());
    }

    public async Task AddActorAsync(string featureName, string actorId)
    {
        featureName = $"{KeyPrefix}{featureName}";
        var database = await GetDatabaseAsync();
        var flagJson = (string?)await database.StringGetAsync(featureName);
        var flag = flagJson?.FlagFromJson();
        
        if (flag is null)
            return;
        
        if (flag.ActorIds.Contains(actorId))
            return;
        
        flag.ActorIds.Add(actorId);
        await database.StringSetAsync(featureName, flag.ToJson());
    }

    public async Task DisableAsync(string featureName)
    {
        featureName = $"{KeyPrefix}{featureName}";
        var database = await GetDatabaseAsync();
        var flagJson = (string?)await database.StringGetAsync(featureName);
        var flag = flagJson?.FlagFromJson();
        
        if (flag is null)
            return;
        
        flag.IsEnabled = false;
        await database.StringSetAsync(featureName, flag.ToJson());
    }

    public async Task DeleteAsync(string featureName)
    {
        featureName = $"{KeyPrefix}{featureName}";
        var database = await GetDatabaseAsync();
        await database.KeyDeleteAsync(featureName);
    }

    public async Task DeleteActorAsync(string featureName, string actorId)
    {
        featureName = $"{KeyPrefix}{featureName}";
        var database = await GetDatabaseAsync();
        var flagJson = (string?)await database.StringGetAsync(featureName);
        var flag = flagJson?.FlagFromJson();
        
        if (flag is null)
            return;
        
        flag.ActorIds.Remove(actorId);
        await database.StringSetAsync(featureName, flag.ToJson());
    }
}