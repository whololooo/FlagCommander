using FlagCommander.Persistence;
using FlagCommander.Persistence.Models;

namespace FlagCommander;

public class FlagCommander(IRepository repository) : IFlagCommander, IFlagCommanderManagement
{
    public async Task<bool> IsEnabledAsync(string featureName)
    {
        var flag = await repository.GetAsync(featureName);
        return flag is not null && AllowedByPercentage(flag.PercentageOfTime);
    }

    public async Task<bool> IsEnabledAsync<TActor>(string featureName, TActor actor, Func<TActor, string> actorIdPredicate)
    {
        var actorId = actorIdPredicate(actor);
        var flag = await repository.GetAsync(featureName);
        if (flag is null)
            return false;
        return flag.ActorIds.Contains(actorId) && AllowedByPercentage(flag.PercentageOfActors);
    }

    public async Task<bool> IsEnabledAsync<TActor>(string featureName, TActor actor, Func<TActor, bool> actorConditionPredicate)
    {
        var actorConditionResult = actorConditionPredicate(actor);
        var flag = await repository.GetAsync(featureName);
        return flag is not null && actorConditionResult;
    }

    public async Task EnableAsync(string featureName)
    {
        var flag = await repository.GetAsync(featureName);
        if (flag is not null)
            return;

        await repository.EnableAsync(featureName);
    }

    public async Task EnableAsync<TActor>(string featureName, TActor actor, Func<TActor, string> actorIdPredicate)
    {
        var actorId = actorIdPredicate(actor);
        var flag = await repository.GetAsync(featureName);
        if (flag is null)
        {
            await repository.EnableAsync(featureName);
        }
        await repository.AddActorAsync(featureName, actorId);
    }

    public async Task EnablePercentageOfTimeAsync(string featureName, int percentage)
    {
        if (percentage is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 100");
        
        var flag = await repository.GetAsync(featureName);
        if (flag is null)
        {
            await repository.EnableAsync(featureName);
        }

        await repository.SetPercentageOfTimeAsync(featureName, percentage);
    }

    public async Task EnablePercentageOfActorsAsync(string featureName, int percentage)
    {
        if (percentage is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 100");
        
        var flag = await repository.GetAsync(featureName);
        if (flag is null)
        {
            await repository.EnableAsync(featureName);
        }

        await repository.SetPercentageOfActorsAsync(featureName, percentage);
    }

    public async Task DisableAsync(string featureName)
    {
        await repository.DisableAsync(featureName);
    }

    public async Task DisableAsync<TActor>(string featureName, TActor actor, Func<TActor, string> actorIdPredicate)
    {
        var actorId = actorIdPredicate(actor);
        var flag = await repository.GetAsync(featureName);
        if (flag is null)
            return;
        
        await repository.DeleteActorAsync(featureName, actorId);
    }

    private bool AllowedByPercentage(int percentage)
    {
        var randomValue = Random.Shared.NextDouble();
        return randomValue * 100 < percentage;
    }

    public async Task<List<Flag>> GetFlagsAsync()
    {
        return await repository.GetFlagsAsync(); 
    }

    public async Task<Flag?> GetAsync(string name)
    {
        return await repository.GetAsync(name);
    }

    public async Task DeleteFlagAsync(string name)
    {
        await repository.DeleteAsync(name);
    }
}