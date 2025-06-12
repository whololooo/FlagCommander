using FlagCommander;
using Microsoft.AspNetCore.Components;

namespace FlagCommanderUI.Components;

public partial class ControlCenter : ComponentBase
{
    [Inject] public IFlagCommander FlagCommander { get; set; }

    private bool flagEnabled;
    private const string FlagName = "test-feature";
    
    private string StatusText => $"{FlagName} is {(flagEnabled ? "enabled" : "disabled")}";
    
    protected override async Task OnInitializedAsync()
    {
        flagEnabled = await FlagCommander.IsEnabledAsync(FlagName);
        await base.OnInitializedAsync();
    }

    private async Task ToggleFlag()
    {
        if (flagEnabled)
        {
            await FlagCommander.DisableAsync(FlagName);
        }
        else
        {
            await FlagCommander.EnableAsync(FlagName);
        }
        flagEnabled = await FlagCommander.IsEnabledAsync(FlagName);
    }
}