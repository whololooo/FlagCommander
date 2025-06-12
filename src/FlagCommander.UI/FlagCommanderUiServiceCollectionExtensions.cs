using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class FlagCommanderUiServiceCollectionExtensions
{
    public static IServiceCollection AddFlagCommanderUi(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
        
        return services;
    }
}