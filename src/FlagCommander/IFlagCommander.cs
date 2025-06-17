namespace FlagCommander;

public interface IFlagCommander
{
    /// <summary>
    /// Checks whether the feature is enabled
    /// </summary>
    /// <param name="featureName">Feature name</param>
    /// <returns>Return true if the feature is enabled, otherwise false</returns>
    Task<bool> IsEnabledAsync(string featureName);

    /// <summary>
    /// Checks whether the feature is enabled for given actor
    /// </summary>
    /// <param name="featureName">Feature name</param>
    /// <param name="actor">The actor object</param>
    /// <param name="actorIdPredicate">Function that accepts an actor object and returns its unique string identification</param>
    /// <typeparam name="TActor">Type of the actor</typeparam>
    /// <returns>Return true if the feature is enabled for given actor, otherwise false</returns>
    Task<bool> IsEnabledAsync<TActor>(string featureName, TActor actor, Func<TActor, string> actorIdPredicate);
    
    /// <summary>
    /// Checks whether the feature is enabled for given actor
    /// </summary>
    /// <param name="featureName">Feature name</param>
    /// <param name="actor">The actor object</param>
    /// <param name="actorConditionPredicate">Function that accepts an actor object and evaluates it whether it passes given condition</param>
    /// <typeparam name="TActor">Type of the actor</typeparam>
    /// <returns>Return true if the feature is enabled for given actor, otherwise false</returns>
    Task<bool> IsEnabledAsync<TActor>(string featureName, TActor actor, Func<TActor, bool> actorConditionPredicate);
    
    /// <summary>
    /// Enables the feature for everyone
    /// </summary>
    /// <param name="featureName">Feature name</param>
    Task EnableAsync(string featureName);
    
    /// <summary>
    /// Enables the feature for given actor
    /// </summary>
    /// <param name="featureName">Feature name</param>
    /// <param name="actor">The actor object</param>
    /// <param name="actorIdPredicate">Function that accepts an actor object and returns its unique string identification</param>
    /// <typeparam name="TActor">Type of the actor</typeparam>
    Task EnableAsync<TActor>(string featureName, TActor actor, Func<TActor, string> actorIdPredicate);
    
    /// <summary>
    /// Enables the feature for percentage of time
    /// </summary>
    /// <param name="featureName">Feature name</param>
    /// <param name="percentage">The percentage of time that the feature is enabled</param>
    Task EnablePercentageOfTimeAsync(string featureName, int percentage);
    
    /// <summary>
    /// Enables the feature for percentage of actors
    /// </summary>
    /// <param name="featureName">Feature name</param>
    /// <param name="percentage">The percentage of actors that the feature is enabled for</param>
    Task EnablePercentageOfActorsAsync(string featureName, int percentage);
    
    /// <summary>
    /// Disables the feature for everyone
    /// </summary>
    /// <param name="featureName">Feature name</param>
    Task DisableAsync(string featureName);
    
    /// <summary>
    /// Disables the feature for given actor
    /// </summary>
    /// <param name="featureName">Feature name</param>
    /// <param name="actor">The actor object</param>
    /// <param name="actorIdPredicate">Function that accepts an actor object and returns its unique string identification</param>
    /// <typeparam name="TActor">Type of the actor</typeparam>
    Task DisableAsync<TActor>(string featureName, TActor actor, Func<TActor, string> actorIdPredicate);
}