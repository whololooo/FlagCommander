# FlagCommander - Feature flags for .NET

A powerful and flexible feature flag solution for .NET applications, enabling seamless feature toggling, gradual rollouts, and A/B testing. This library helps teams control feature availability without redeploying code, improving agility and experimentation.

## Features
- Easy-to-use API for managing feature flags
- Support for dynamic flag evaluation
- Actor-based and percentage-based targeting
- Integration-friendly with existing .NET applications

Get started with feature flagging today!

## Usage

Register with the dependency injection container in your .NET application:
```csharp

builder.Services.AddFlagCommander(options =>
{
    // Currenntly supports in-memory storage (SqlLite), Postgres, MSSQL, 
    // MySQL, MongoDB and Redis as flag storage options
    options.UseInMemoryRepository();
});

.....
    
// Optional FlagCommanderUI for management
builder.Services.AddFlagCommanderUi();

app.UseFlagCommanderUI(options =>
{
    options
        .WithAuthorizationRequired()
        .WithRoutePrefix("flag-commander");
});
    

```
FlagCommander UI can be accessed by default at {YOUR_BASE_URL}/flag-commander, but it can be customized using the `WithRoutePrefix` method. This page does not require authorization by default, but you can enable it using `WithAuthorizationRequired()`.

Then in your code, you can inject `IFlagCommander` to manage feature flags:

```csharp
using FlagCommander;

public class FeatureDemo
{
    private readonly IFlagCommander _flagCommander;

    public FeatureDemo(IFlagCommander flagCommander)
    {
        _flagCommander = flagCommander;
    }

    public async Task RunAsync()
    {
        // Enable a feature flag
        await _flagCommander.EnableAsync("NewFeature");

        // Check if a feature is enabled
        bool isEnabled = await _flagCommander.IsEnabledAsync("NewFeature");
        if (isEnabled)
        {
            // Feature-specific logic
        }

        // Enable for a specific actor
        var user = new User { Id = "user123" };
        await _flagCommander.EnableAsync("BetaFeature", user, u => u.Id);

        // Check for actor-based flag
        bool isBetaEnabled = await _flagCommander.IsEnabledAsync("BetaFeature", user, u => u.Id);

        // Disable a feature flag
        await _flagCommander.DisableAsync("NewFeature");
    }

    private class User
    {
        public string Id { get; set; }
    }
}
```

## Contributions
Suggestions and contributions are welcome! Please open an issue or submit a pull request on GitHub.
