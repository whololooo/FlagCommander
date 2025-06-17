using FlagCommander;
using FlagCommanderUI;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFlagCommander(options =>
{
    options.UseInMemoryRepository();
});
builder.Services.AddFlagCommanderUi();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseFlagManagerUI(new FlagCommanderUiOptions());


app.MapGet("/flag-setup", async (IFlagCommander flagCommander) =>
{
    await flagCommander.EnableAsync("test-feature");
    await flagCommander.EnablePercentageOfTimeAsync("test-feature", 50);
});

app.MapGet("/flag-test", async (IFlagCommander flagCommander) =>
{
    var isEnabled = await flagCommander.IsEnabledAsync("test-feature");
    return isEnabled ? "Feature is enabled" : "Feature is disabled";
});

app.MapGet("/flag-enabled", async ([FromQuery(Name = "flag")] string flagName, IFlagCommander flagCommander) =>
{
    var isEnabled = await flagCommander.IsEnabledAsync(flagName);
    return isEnabled ? $"Feature \"{flagName}\" is enabled" : $"Feature \"{flagName}\" is disabled";
});

app.Run();
