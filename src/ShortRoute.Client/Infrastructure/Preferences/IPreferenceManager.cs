namespace ShortRoute.Client.Infrastructure.Preferences;

public interface IPreferenceManager : IAppService
{
    Task SetPreference(ClientPreference preference);

    Task<ClientPreference> GetPreference();

    Task<bool> ChangeLanguageAsync(string languageCode);
}