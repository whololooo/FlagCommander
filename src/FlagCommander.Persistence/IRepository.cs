using FlagCommander.Persistence.Models;

namespace FlagCommander.Persistence;

public interface IRepository
{
    Task<Flag?> GetAsync(string featureName);
    Task EnableAsync(string featureName);
    Task SetPercentageOfTimeAsync(string featureName, int percentage);
    Task SetPercentageOfActorsAsync(string featureName, int percentage);
    Task AddActorAsync(string featureName, string actorId);
    Task DisableAsync(string featureName);
    Task DeleteAsync(string featureName);
    Task DeleteActorAsync(string featureName, string actorId);
}