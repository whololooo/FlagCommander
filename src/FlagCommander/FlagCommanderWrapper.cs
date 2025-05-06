using Microsoft.Extensions.Options;

namespace FlagCommander;

public class FlagCommanderWrapper : IFlagCommander
{
    private readonly IFlagCommander _flagCommander;
    
    public FlagCommanderWrapper(IOptions<FlagCommanderOptions> options)
    {
        if (options.Value.Repository is null)
        {
            throw new ArgumentNullException(nameof(options), "Repository is not set in options.");
        }
        
        _flagCommander = new FlagCommander(options.Value.Repository);
    }

    public Task<bool> IsEnabledAsync(string featureName)
    {
        return _flagCommander.IsEnabledAsync(featureName);
    }

    public Task<bool> IsEnabledAsync<TActor>(string featureName, TActor actor, Func<TActor, string> actorIdPredicate)
    {
        return _flagCommander.IsEnabledAsync(featureName, actor, actorIdPredicate);
    }

    public Task<bool> IsEnabledAsync<TActor>(string featureName, TActor actor, Func<TActor, bool> actorConditionPredicate)
    {
        return _flagCommander.IsEnabledAsync(featureName, actor, actorConditionPredicate);
    }

    public Task EnableAsync(string featureName)
    {
        return _flagCommander.EnableAsync(featureName);
    }

    public Task EnableAsync<TActor>(string featureName, TActor actor, Func<TActor, string> actorIdPredicate)
    {
        return _flagCommander.EnableAsync(featureName, actor, actorIdPredicate);
    }

    public Task EnableAsyncPercentageOfTime(string featureName, int percentage)
    {
        return _flagCommander.EnableAsyncPercentageOfTime(featureName, percentage);
    }

    public Task EnableAsyncPercentageOfActors(string featureName, int percentage)
    {
        return _flagCommander.EnableAsyncPercentageOfActors(featureName, percentage);
    }

    public Task DisableAsync(string featureName)
    {
        return _flagCommander.DisableAsync(featureName);
    }

    public Task DisableAsync<TActor>(string featureName, TActor actor, Func<TActor, string> actorIdPredicate)
    {
        return _flagCommander.DisableAsync(featureName, actor, actorIdPredicate);
    }
}