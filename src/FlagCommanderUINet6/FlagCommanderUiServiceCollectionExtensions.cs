namespace Microsoft.Extensions.DependencyInjection;

public static class FlagCommanderUiServiceCollectionExtensions
{
    public static IServiceCollection AddFlagCommanderUi(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);

        return services;
    }
}