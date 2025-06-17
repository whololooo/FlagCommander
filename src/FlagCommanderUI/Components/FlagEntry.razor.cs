using FlagCommander;
using FlagCommander.Persistence.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FlagCommanderUI.Components;

public partial class FlagEntry(IFlagCommander flagCommander, IFlagCommanderManagement flagCommanderManagement, IJSRuntime jsRuntime) 
    : ComponentBase
{
    [Parameter] public required Flag Flag { get; set; }
    [Parameter] public EventCallback<(string, MessageType)> OnUpdated { get; set; }
    [Parameter] public EventCallback<(string, MessageType)> OnDeleted { get; set; }
    [Parameter] public EventCallback<Flag> OnDeletedUnsaved { get; set; }

    private string editModeName = string.Empty;
    private bool editModeIsEnabled = true;
    private int editModePercentageOfActors = 100;
    private int editModePercentageOfTime = 100;
    private List<string> editModeActorIds = [];

    private string EditModeActorIdsString
    {
        get => string.Join(", ", editModeActorIds);
        set => editModeActorIds = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(item => item.Trim()).ToList();
    }
    
    private bool isEditing;
    private bool isBusy;
    
    
    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(Flag.Name))
            StartEdit();
        
        base.OnParametersSet();
    }

    private void StartEdit()
    {
        editModeName = Flag.Name;
        editModeIsEnabled = Flag.IsEnabled;
        editModePercentageOfActors = Flag.PercentageOfActors;
        editModePercentageOfTime = Flag.PercentageOfTime;
        editModeActorIds = Flag.ActorIds.ToList();
        
        isEditing = true;
    }
    
    private void StopEdit()
    {
        isEditing = false;
    }

    private async Task Save()
    {
        try
        {
            isBusy = true;
            
            if (!await ValidateInputs())
            {
                return;
            }

            await flagCommander.EnableAsync(editModeName);
            await flagCommander.EnablePercentageOfTimeAsync(editModeName, editModePercentageOfTime);
            await flagCommander.EnablePercentageOfActorsAsync(editModeName, editModePercentageOfActors);
            foreach (var editModeActorId in editModeActorIds)
            {
                await flagCommander.EnableAsync(editModeName, editModeActorId, actor => actor);
            }

            if (!editModeIsEnabled)
            {
                await flagCommander.DisableAsync(editModeName);
            }

            await OnUpdated.InvokeAsync(($"Flag {editModeName} updated successfully", MessageType.Success));
            StopEdit();
        }
        catch (Exception ex)
        {
            await OnUpdated.InvokeAsync(($"Oops, something went wrong: {ex.Message}", MessageType.Error));
        }
        finally
        {
            isBusy = false;
        }
    }

    private async Task<bool> ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(editModeName))
        {
            await OnUpdated.InvokeAsync(("Flag name cannot be empty", MessageType.Error));
            return false;
        }
        
        if (editModeName.Length > 255)
        {
            await OnUpdated.InvokeAsync(("Flag name cannot be longer than 255 characters", MessageType.Error));
            return false;
        }
        
        var allFlagNames = (await flagCommanderManagement.GetFlagsAsync()).Select(item => item.Name).ToList();
        if (allFlagNames.Contains(editModeName) && Flag.Name != editModeName)
        {
            await OnUpdated.InvokeAsync(($"Flag with the name \"{editModeName}\" already exists", MessageType.Error));
            return false;
        }
        
        if (editModePercentageOfActors is < 0 or > 100)
        {
            await OnUpdated.InvokeAsync(("Percentage of actors must be between 0 and 100", MessageType.Error));
            return false;
        }
        
        if (editModePercentageOfTime is < 0 or > 100)
        {
            await OnUpdated.InvokeAsync(("Percentage of time must be between 0 and 100", MessageType.Error));
            return false;
        }
        
        if (editModeActorIds.Any(actorId => actorId.Length > 255))
        {
            await OnUpdated.InvokeAsync(("Actor IDs cannot be longer than 255 characters", MessageType.Error));
            return false;
        }
        return true;
    }

    private async Task Delete()
    {
        try
        {
            isBusy = true;
            if (string.IsNullOrWhiteSpace(Flag.Name))
            {
                await OnDeletedUnsaved.InvokeAsync(Flag);
                return;
            }
            
            var confirmed = await jsRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to permanently delete this flag?");

            if (confirmed)
            {
                await flagCommanderManagement.DeleteFlagAsync(Flag.Name);
                await OnDeleted.InvokeAsync(($"Flag {Flag.Name} deleted successfully", MessageType.Success));
            }
        }
        catch (Exception ex)
        {
            await OnDeleted.InvokeAsync(($"Oops, something went wrong: {ex.Message}", MessageType.Error));
        }
        finally
        {
            isBusy = false;
        }
    }
}