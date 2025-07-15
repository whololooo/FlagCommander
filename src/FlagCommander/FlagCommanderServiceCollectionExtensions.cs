using Microsoft.Extensions.DependencyInjection.Extensions;
using FlagCommander;

namespace Microsoft.Extensions.DependencyInjection;

public static class FlagCommanderServiceCollectionExtensions
{
    public static IServiceCollection AddFlagCommander(this IServiceCollection services, 
        Action<FlagCommanderOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(setupAction);

        services.AddFlagCommander();
        services.Configure(setupAction);
        return services;
    }
    
    public static IServiceCollection AddFlagCommander(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions();
        services.TryAdd(ServiceDescriptor.Singleton<IFlagCommander, FlagCommanderWrapper>());
        services.TryAdd(ServiceDescriptor.Singleton<IFlagCommanderManagement, FlagCommanderWrapper>());
        return services;
    }
}