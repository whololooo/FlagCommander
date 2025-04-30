using FlagCommander.Persistence;

namespace FlagCommander;

public class FlagManager(IRepository repository) : IFlagManager
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

    public async Task EnableAsyncPercentageOfTime(string featureName, int percentage)
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

    public async Task EnableAsyncPercentageOfActors(string featureName, int percentage)
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
        var randomValue = new Random().NextDouble();
        return randomValue * 100 < percentage;
    }
}