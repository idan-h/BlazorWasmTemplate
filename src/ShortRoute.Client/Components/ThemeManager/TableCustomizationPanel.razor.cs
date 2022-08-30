using ShortRoute.Client.Infrastructure.Preferences;
using Microsoft.AspNetCore.Components;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ShortRoute.Client.Components.ThemeManager;

public partial class TableCustomizationPanel
{
    [Parameter]
    public bool IsDense { get; set; }
    [Parameter]
    public bool IsStriped { get; set; }
    [Parameter]
    public bool HasBorder { get; set; }
    [Parameter]
    public bool IsHoverable { get; set; }

    private FshTablePreference _tablePreference = new();

    protected override async Task OnInitializedAsync()
    {
        if (await ClientPreferences.GetPreference() is ClientPreference clientPreference)
        {
            _tablePreference = clientPreference.TablePreference;
        }

        IsDense = _tablePreference.IsDense;
        IsStriped = _tablePreference.IsStriped;
        HasBorder = _tablePreference.HasBorder;
        IsHoverable = _tablePreference.IsHoverable;
    }

    [Parameter]
    public EventCallback<bool> OnDenseSwitchToggled { get; set; }

    [Parameter]
    public EventCallback<bool> OnStripedSwitchToggled { get; set; }

    [Parameter]
    public EventCallback<bool> OnBorderdedSwitchToggled { get; set; }

    [Parameter]
    public EventCallback<bool> OnHoverableSwitchToggled { get; set; }

    private void PublishChange()
    {
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<FshTablePreference>(_tablePreference));
    }

    private async Task ToggleDenseSwitch()
    {
        _tablePreference.IsDense = !_tablePreference.IsDense;
        await OnDenseSwitchToggled.InvokeAsync(_tablePreference.IsDense);
        PublishChange();
    }

    private async Task ToggleStripedSwitch()
    {
        _tablePreference.IsStriped = !_tablePreference.IsStriped;
        await OnStripedSwitchToggled.InvokeAsync(_tablePreference.IsStriped);
        PublishChange();
    }

    private async Task ToggleBorderedSwitch()
    {
        _tablePreference.HasBorder = !_tablePreference.HasBorder;
        await OnBorderdedSwitchToggled.InvokeAsync(_tablePreference.HasBorder);
        PublishChange();
    }

    private async Task ToggleHoverableSwitch()
    {
        _tablePreference.IsHoverable = !_tablePreference.IsHoverable;
        await OnHoverableSwitchToggled.InvokeAsync(_tablePreference.IsHoverable);
        PublishChange();
    }
}