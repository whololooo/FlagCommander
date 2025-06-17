using FlagCommanderUI;
using FlagCommanderUI.Components;

namespace Microsoft.AspNetCore.Builder;

public static class FlagCommanderUiBuilderExtensions
{
    /// <summary>
    /// Register the FlagManagerUI middleware with provided options
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static IApplicationBuilder UseFlagManagerUI(this IApplicationBuilder app, FlagCommanderUiOptions options)
    {
        app.UseStaticFiles();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapRazorComponents<ControlCenter>().AddInteractiveServerRenderMode();
        });

        app.UseMiddleware<FlagCommanderUiMiddleware>(options);
        
        return app;
    }
}