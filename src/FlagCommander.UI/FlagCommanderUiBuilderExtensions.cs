using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using FlagCommander.UI;
using Microsoft.Extensions.FileProviders;

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
        
        var assembly = typeof(FlagCommanderUiBuilderExtensions).Assembly;
        var fileProvider = new EmbeddedFileProvider(assembly);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = fileProvider,
            RequestPath = $"/{options.RoutePrefix}"
        });

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapBlazorHub();
        });

        app.UseMiddleware<FlagCommanderUiMiddleware>(options);
        
        return app;
    }
}