using FlagCommander.Persistence.Models;
using Microsoft.Extensions.Options;

namespace FlagCommander;

public class FlagCommanderWrapper : IFlagCommander, IFlagCommanderManagement
{
    private readonly IFlagCommander _flagCommander;
    private readonly IFlagCommanderManagement _flagCommanderManagement;
    
    public FlagCommanderWrapper(IOptions<FlagCommanderOptions> options)
    {
        if (options.Value.Repository is null)
        {
            throw new ArgumentNullException(nameof(options), "Repository is not set in options.");
        }
        
        _flagCommander = new FlagCommander(options.Value.Repository);
        _flagCommanderManagement = (_flagCommander as IFlagCommanderManagement)!;
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

    public Task EnablePercentageOfTimeAsync(string featureName, int percentage)
    {
        return _flagCommander.EnablePercentageOfTimeAsync(featureName, percentage);
    }

    public Task EnablePercentageOfActorsAsync(string featureName, int percentage)
    {
        return _flagCommander.EnablePercentageOfActorsAsync(featureName, percentage);
    }

    public Task DisableAsync(string featureName)
    {
        return _flagCommander.DisableAsync(featureName);
    }

    public Task DisableAsync<TActor>(string featureName, TActor actor, Func<TActor, string> actorIdPredicate)
    {
        return _flagCommander.DisableAsync(featureName, actor, actorIdPredicate);
    }

    public Task<List<Flag>> GetFlagsAsync()
    {
        return _flagCommanderManagement.GetFlagsAsync();
    }

    public Task<Flag?> GetAsync(string name)
    {
        return _flagCommanderManagement.GetAsync(name);
    }

    public Task DeleteFlagAsync(string name)
    {
        return _flagCommanderManagement.DeleteFlagAsync(name);
    }
}