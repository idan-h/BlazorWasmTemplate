using System.Text.RegularExpressions;
using ShortRoute.Client.Infrastructure.Theme;
using MudBlazor;

namespace ShortRoute.Client.Infrastructure.Preferences;

public class ClientPreferenceManager : IClientPreferenceManager
{
    private readonly ILocalStorageService _localStorageService;

    public ClientPreferenceManager(
        ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task<bool> ToggleDarkModeAsync()
    {
        if (await GetPreference() is ClientPreference preference)
        {
            preference.IsDarkMode = !preference.IsDarkMode;
            await SetPreference(preference);
            return !preference.IsDarkMode;
        }

        return false;
    }

    public async Task<bool> ToggleDrawerAsync()
    {
        if (await GetPreference() is ClientPreference preference)
        {
            preference.IsDrawerOpen = !preference.IsDrawerOpen;
            await SetPreference(preference);
            return preference.IsDrawerOpen;
        }

        return false;
    }

    public async Task<bool> ToggleLayoutDirectionAsync()
    {
        if (await GetPreference() is ClientPreference preference)
        {
            preference.IsRTL = !preference.IsRTL;
            await SetPreference(preference);
            return preference.IsRTL;
        }

        return false;
    }

    public async Task<bool> ChangeLanguageAsync(string languageCode)
    {
        if (await GetPreference() is ClientPreference preference)
        {
            var language = Array.Find(LocalizationConstants.SupportedLanguages, a => a.Code == languageCode);
            if (language?.Code is not null)
            {
                preference.LanguageCode = language.Code;
                preference.IsRTL = language.IsRTL;
            }
            else
            {
                preference.LanguageCode = LocalizationConstants.DefaultLanguage.Code;
                preference.IsRTL = LocalizationConstants.DefaultLanguage.IsRTL;
            }

            await SetPreference(preference);
            return true;
        }

        return false;
    }

    public async Task<MudTheme> GetCurrentThemeAsync()
    {
        if (await GetPreference() is ClientPreference preference && preference.IsDarkMode)
        {
            return new DarkTheme();
        }

        return new LightTheme();
    }

    public async Task<string> GetPrimaryColorAsync()
    {
        if (await GetPreference() is ClientPreference preference)
        {
            string colorCode = preference.PrimaryColor;
            if (Regex.Match(colorCode, "^#(?:[0-9a-fA-F]{3,4}){1,2}$").Success)
            {
                return colorCode;
            }
            else
            {
                preference.PrimaryColor = CustomColors.Light.Primary;
                await SetPreference(preference);
                return preference.PrimaryColor;
            }
        }

        return CustomColors.Light.Primary;
    }

    public async Task<bool> IsRTL()
    {
        if (await GetPreference() is ClientPreference preference)
        {
            return preference.IsRTL;
        }

        return false;
    }

    public async Task<bool> IsDrawerOpen()
    {
        if (await GetPreference() is ClientPreference preference)
        {
            return preference.IsDrawerOpen;
        }

        return false;
    }

    public const string PerferenceStorageName = "clientPreference";

    public async Task<ClientPreference> GetPreference()
    {
        return await _localStorageService.GetItemAsync<ClientPreference>(PerferenceStorageName) ?? new ClientPreference();
    }

    public async Task SetPreference(ClientPreference preference)
    {
        await _localStorageService.SetItemAsync(PerferenceStorageName, preference);
    }
}