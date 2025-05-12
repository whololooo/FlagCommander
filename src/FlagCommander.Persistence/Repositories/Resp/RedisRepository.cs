using StackExchange.Redis;

namespace FlagCommander.Persistence.Repositories.Resp;

public class RedisRepository : RespRepositoryBase
{
    public RedisRepository(string connectionString) : base(connectionString)
    {
    }
}