using FlagCommanderUINet6;

namespace Microsoft.AspNetCore.Builder;

public static class FlagCommanderUiBuilderExtensions
{
    /// <summary>
    /// Register the FlagManagerUI middleware with provided options
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static IApplicationBuilder UseFlagCommanderUI(this IApplicationBuilder app, FlagCommanderUiOptions? options)
    {
        options ??= new FlagCommanderUiOptions();
        
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            if (options.RequireAuthorization)
            {
                endpoints.MapRazorPages().RequireAuthorization();
            }
            else
            {
                endpoints.MapRazorPages();
            }
            endpoints.MapBlazorHub();
        });

        app.UseMiddleware<FlagCommanderUiMiddleware>(options);
        
        return app;
    }
}