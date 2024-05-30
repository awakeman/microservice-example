namespace SettingsService;

public interface ISettingsRepository
{
    Task SaveSettingsAsync(SettingsModel settings);
}