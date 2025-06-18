using Microsoft.Extensions.Options;

namespace FlagCommanderUINet6;

public class FlagCommanderUiOptions : IOptions<FlagCommanderUiOptions>
{
    public FlagCommanderUiOptions Value => this;
    
    /// <summary>
    /// Gets or sets a route prefix for accessing the FlagCommander UI.
    /// </summary>
    public string RoutePrefix { get; set; } = "flag-commander";
    
    /// <summary>
    /// Gets or sets the requirement of authorization in order to access FlagCommander UI.
    /// </summary>
    public bool RequireAuthorization { get; set; } = false;
}