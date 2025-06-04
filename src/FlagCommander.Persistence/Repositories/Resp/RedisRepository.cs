using StackExchange.Redis;

namespace FlagCommander.Persistence.Repositories.Resp;

public class RedisRepository : RespRepositoryBase
{
    public RedisRepository(string connectionString) : base(connectionString)
    {
    }

    protected override Task Init()
    {
        return Task.CompletedTask;
    }
}