namespace Microsoft.Extensions.DependencyInjection;

public static class FlagCommanderUiServiceCollectionExtensions
{
    public static IServiceCollection AddFlagCommanderUI(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddRazorComponents().AddInteractiveServerComponents();
        return services;
    }
}