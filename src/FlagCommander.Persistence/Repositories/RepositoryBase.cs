namespace FlagCommander.Persistence.Repositories;

public abstract class RepositoryBase
{
    protected string ConnectionString { get; }
    
    protected abstract Task Init();
    
    protected RepositoryBase(string connectionString)
    {
        ConnectionString = connectionString;
        // ReSharper disable once VirtualMemberCallInConstructor
        var initTask = Init();
        Task.Run(() => initTask).GetAwaiter().GetResult();
    }
}