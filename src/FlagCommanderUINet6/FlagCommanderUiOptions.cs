using Microsoft.Extensions.Options;

namespace FlagCommanderUINet6;

public class FlagCommanderUiOptions : IOptions<FlagCommanderUiOptions>
{
    public FlagCommanderUiOptions Value => this;
    
    internal string RoutePrefix { get; set; } = "flag-commander";

    internal bool RequireAuthorization { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a route prefix for accessing the FlagCommander UI.
    /// </summary>
    public FlagCommanderUiOptions WithRoutePrefix(string routePrefix)
    {
        RoutePrefix = routePrefix;
        return this;
    }
    
    /// <summary>
    /// Gets or sets the requirement of authorization in order to access FlagCommander UI.
    /// </summary>
    public FlagCommanderUiOptions WithAuthorizationRequired()
    {
        RequireAuthorization = true;
        return this;
    }
}