using FlagCommander.Persistence.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FlagCommander.Persistence.Repositories.NoSql;

public class MongoDbRepository : NoSqlRepositoryBase, IRepository
{
    private MongoClient mongoClient;
    private IMongoCollection<Flag> flagsCollection;
    private const string CollectionName = "__flag_commander_flags";
    
    public MongoDbRepository(string connectionString) : base(connectionString)
    {
    }

    protected override async Task Init()
    {
        mongoClient = new MongoClient(ConnectionString);
        var dbName = GetDbNameFromConnectionString();
        var database = mongoClient.GetDatabase(dbName);
        await CreateCollectionIfNotExists(database);
        flagsCollection = database.GetCollection<Flag>("__flag_commander_flags");
    }

    private async Task CreateCollectionIfNotExists(IMongoDatabase database)
    {
        var filter = new BsonDocument("name", CollectionName);
        var options = new ListCollectionNamesOptions
        {
            Filter = filter
        };
        var collectionNames = await database.ListCollectionNamesAsync(options);
        var exists = await collectionNames.AnyAsync();
        
        if (!exists)
        {
            await database.CreateCollectionAsync(CollectionName);
        }
    }

    public async Task<Flag?> GetAsync(string featureName)
    {
        var filter = Builders<Flag>.Filter.Eq(f => f.Name, featureName) & Builders<Flag>.Filter.Eq(f => f.IsEnabled, true);
        var result = await flagsCollection.Find(filter).FirstOrDefaultAsync();
        return result;
    }

    public async Task<List<Flag>> GetFlagsAsync()
    {
        var result = await (await flagsCollection.FindAsync(_ => true)).ToListAsync();
        return result ?? [];
    }

    public async Task EnableAsync(string featureName)
    {
        var filter = Builders<Flag>.Filter.Eq(f => f.Name, featureName);
        var flag = await flagsCollection.Find(filter).FirstOrDefaultAsync()!;
        
        if (flag is null)
        {
            flag = new Flag { Name = featureName, IsEnabled = true };
            await flagsCollection.InsertOneAsync(flag);
        }
        else if (!flag.IsEnabled)
        {
            flag.IsEnabled = true;
            await flagsCollection.ReplaceOneAsync(filter, flag);
        }
    }

    public async Task SetPercentageOfTimeAsync(string featureName, int percentage)
    {
        var filter = Builders<Flag>.Filter.Eq(f => f.Name, featureName);
        var flag = await flagsCollection.Find(filter).FirstOrDefaultAsync()!;
        
        if (flag is null)
            return;
        
        flag.PercentageOfTime = percentage;
        await flagsCollection.ReplaceOneAsync(filter, flag);
    }

    public async Task SetPercentageOfActorsAsync(string featureName, int percentage)
    {
        var filter = Builders<Flag>.Filter.Eq(f => f.Name, featureName);
        var flag = await flagsCollection.Find(filter).FirstOrDefaultAsync()!;
        
        if (flag is null)
            return;
        
        flag.PercentageOfActors = percentage;
        await flagsCollection.ReplaceOneAsync(filter, flag);
    }

    public async Task AddActorAsync(string featureName, string actorId)
    {
        var filter = Builders<Flag>.Filter.Eq(f => f.Name, featureName);
        var flag = await flagsCollection.Find(filter).FirstOrDefaultAsync()!;
        
        if (flag is null)
            return;
    
        if (!flag.ActorIds.Contains(actorId))
        {
            flag.ActorIds.Add(actorId);
            await flagsCollection.ReplaceOneAsync(filter, flag);
        }
    }

    public async Task DisableAsync(string featureName)
    {
        var filter = Builders<Flag>.Filter.Eq(f => f.Name, featureName);
        var flag = await flagsCollection.Find(filter).FirstOrDefaultAsync()!;
        
        if (flag is null)
            return;
        
        flag.IsEnabled = false;
        await flagsCollection.ReplaceOneAsync(filter, flag);
    }

    public async Task DeleteAsync(string featureName)
    {
        var filter = Builders<Flag>.Filter.Eq(f => f.Name, featureName);
        await flagsCollection.DeleteOneAsync(filter);
    }

    public async Task DeleteActorAsync(string featureName, string actorId)
    {
        var filter = Builders<Flag>.Filter.Eq(f => f.Name, featureName);
        var flag = await flagsCollection.Find(filter).FirstOrDefaultAsync()!;
        
        if (flag is null)
            return;
    
        if (flag.ActorIds.Contains(actorId))
        {
            flag.ActorIds.Remove(actorId);
            await flagsCollection.ReplaceOneAsync(filter, flag);
        }
    }

    private string GetDbNameFromConnectionString()
    {
        var dbName = ConnectionString.Split('/').LastOrDefault()?.Split('?').FirstOrDefault();
        return string.IsNullOrWhiteSpace(dbName) ? "__FlagCommander" : dbName;
    }
}