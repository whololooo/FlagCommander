using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace FlagCommander.UI;

public class FlagCommanderUiMiddleware
{
    private readonly FlagCommanderUiOptions _options;
    private readonly RequestDelegate _next;

    public FlagCommanderUiMiddleware(
        RequestDelegate next,
        FlagCommanderUiOptions options)
    {
        _options = options;
        _next = next;
    }
    
    public async Task Invoke(HttpContext httpContext)
    {
        var httpMethod = httpContext.Request.Method;
    
        if (HttpMethods.IsGet(httpMethod))
        {
            var path = httpContext.Request.Path.Value;
    
            // If the RoutePrefix is requested (with or without trailing slash), redirect to index URL
            if (path is not null && Regex.IsMatch(path, $"^/?{Regex.Escape(_options.RoutePrefix)}/?$", RegexOptions.IgnoreCase))
            {
                // Forward to Razor page handler
                //httpContext.Request.Path = "/FlagCommander/Index";
                RespondWithRedirect(httpContext.Response, "/FlagCommander/Index");
                return;
            }
        }
    
        await _next.Invoke(httpContext);
    }

    private static void RespondWithRedirect(HttpResponse response, string location)
    {
        response.StatusCode = StatusCodes.Status301MovedPermanently;
        response.Headers["Location"] = location;
    }
}