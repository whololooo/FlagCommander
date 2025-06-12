using Microsoft.Extensions.Options;

namespace FlagCommander.UI;

public class FlagCommanderUiOptions : IOptions<FlagCommanderUiOptions>
{
    public FlagCommanderUiOptions Value => this;
    
    /// <summary>
    /// Gets or sets a route prefix for accessing the FlagCommander UI.
    /// </summary>
    public string RoutePrefix { get; set; } = "flag-commander";
    
    /// <summary>
    /// FlagCommander instance to use for managing flags.
    /// </summary>
    public IFlagCommander? FlagCommander { get; set; }
}