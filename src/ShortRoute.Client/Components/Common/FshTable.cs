using ShortRoute.Client.Infrastructure.Preferences;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ShortRoute.Client.Components.Common;

public class FshTable<T> : MudTable<T>
{
    [Inject]
    private IClientPreferenceManager ClientPreferences { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (await ClientPreferences.GetPreference() is ClientPreference clientPreference)
        {
            SetTablePreference(clientPreference.TablePreference);
        }

        WeakReferenceMessenger.Default.Register<ValueChangedMessage<FshTablePreference>>(this, (recipient, message) =>
        {
            if (recipient is not FshTable<T> table)
            {
                return;
            }

            table.SetTablePreference(message.Value);
            table.StateHasChanged();
        });

        await base.OnInitializedAsync();
    }

    private void SetTablePreference(FshTablePreference tablePreference)
    {
        Dense = tablePreference.IsDense;
        Striped = tablePreference.IsStriped;
        Bordered = tablePreference.HasBorder;
        Hover = tablePreference.IsHoverable;
    }
}