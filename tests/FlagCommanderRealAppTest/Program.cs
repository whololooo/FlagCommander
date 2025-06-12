using FlagCommander;
using FlagCommander.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddFlagCommander(options =>
{
    options.UseInMemoryRepository();
});
builder.Services.AddFlagCommanderUi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

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
