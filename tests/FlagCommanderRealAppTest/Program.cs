using FlagCommander;
using FlagCommanderUI;

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
    await flagCommander.EnableAsyncPercentageOfTime("test-feature", 50);
});

app.MapGet("/flag-test", async (IFlagCommander flagCommander) =>
{
    var isEnabled = await flagCommander.IsEnabledAsync("test-feature");
    return isEnabled ? "Feature is enabled" : "Feature is disabled";
});

app.Run();
