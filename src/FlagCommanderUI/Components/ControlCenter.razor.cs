using FlagCommander;
using FlagCommander.Persistence.Models;
using Microsoft.AspNetCore.Components;

namespace FlagCommanderUI.Components;

public partial class ControlCenter(IFlagCommanderManagement flagCommanderManagement)
    : ComponentBase
{
    private List<Flag> flags = [];
    
    private string statusMessage = string.Empty;
    private uint isHiding;
    private string statusMessageTypeClass = "alert-info";
    private int progress;

    private const int HideDelay = 4000; // 4 seconds

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await RefreshRows();
    }

    private void AddNewEntry()
    {
        var newEntryInList = new List<Flag>
        {
            new()
        };
        newEntryInList.AddRange(flags);
        flags = newEntryInList;
    }

    private async Task RefreshRows()
    {
        flags = await flagCommanderManagement.GetFlagsAsync();
    }
    private async Task RowUpdated((string status, MessageType type) message)
    {
        ShowStatusMessage(message.status, message.type);
        if (message.type != MessageType.Error)
            await RefreshRows();
    }
    
    private void UnsavedRowDeleted(Flag flag)
    {
        flags.Remove(flag);
        StateHasChanged();
    }

    private async Task RowDeleted((string status, MessageType type) message)
    {
        ShowStatusMessage(message.status, message.type);
        if (message.type != MessageType.Error)
            await RefreshRows();
    }
    
    private void ShowStatusMessage(string message, MessageType type)
    {
        statusMessage = message;
        statusMessageTypeClass = type switch
        {
            MessageType.Success => "alert-success",
            MessageType.Error => "alert-danger",
            MessageType.Info => "alert-info",
            _ => "alert-info"
        };
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        HideStatusMessage();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    private async Task HideStatusMessage()
    {
        isHiding++;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        StartProgress();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        await Task.Delay(HideDelay); // Wait for 4 seconds before hiding
        if (isHiding > 1)
        {
            isHiding--;
        }
        else
        {
            isHiding = 0;
            statusMessage = string.Empty;
            StateHasChanged();
        }
    }

    private async Task StartProgress()
    {
        progress = HideDelay;
        while (progress > 0)
        {
            await Task.Delay(50);
            progress -= 50;
            StateHasChanged();
        }
    }
}

public enum MessageType
{
    Success,
    Error,
    Info
}