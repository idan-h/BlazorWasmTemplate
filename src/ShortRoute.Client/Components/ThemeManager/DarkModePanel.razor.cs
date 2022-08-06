using ShortRoute.Client.Infrastructure.Preferences;
using Microsoft.AspNetCore.Components;

namespace ShortRoute.Client.Components.ThemeManager;

public partial class DarkModePanel
{
    private bool _isDarkMode;
    private bool _isRTL;
    private bool _isExpanded;

    protected override async Task OnInitializedAsync()
    {
        if (await ClientPreferences.GetPreference() is not ClientPreference themePreference) themePreference = new ClientPreference();
        _isDarkMode = themePreference.IsDarkMode;
        _isRTL = themePreference.IsRTL;
    }

    [Parameter]
    public EventCallback<bool> OnIconClicked { get; set; }

    private async Task ToggleDarkMode()
    {
        _isDarkMode = !_isDarkMode;
        await OnIconClicked.InvokeAsync(_isDarkMode);
    }

    private string GetButtonStyleByDir()
    {
        return $"{(_isRTL ? "right" : "left")}:8px!important";
    }

    private void OnIsExpandedChanged()
    {
        _isExpanded = false;
    }
}