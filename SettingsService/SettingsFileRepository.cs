using System.IO.Abstractions;
using System.Text.Json;
using Common;

namespace SettingsService;

public class SettingsFileRepository(
        IFileSystem fs,
        IReadOnlyTenantProvider tenantProvider,
        INotificationClient client,
        ILogger<SettingsFileRepository> logger)
    : ISettingsRepository
{
    private readonly IFileSystem fs = fs;
    private readonly IReadOnlyTenantProvider tenantProvider = tenantProvider;
    private readonly ILogger logger = logger;

    public async Task SaveSettingsAsync(SettingsModel settings)
    {
        var tenant = tenantProvider.Tenant;

        // Create the directory (noop if it already exists)
        var dir = fs.Path.Combine("settings", tenant);
        fs.Directory.CreateDirectory(dir);

        var file = fs.Path.Combine(dir, "settings.json");

        logger.LogInformation("Writing settings to {File}", file);

        // Save the model to the file
        var content = JsonSerializer.Serialize(settings);
        await fs.File.WriteAllTextAsync(file, content);
        
        // Notify that the model was saved
        await client.NotifyAsync(settings);
    }
}
