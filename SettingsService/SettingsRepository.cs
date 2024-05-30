using System.IO.Abstractions;
using System.Text.Json;
using Common;

namespace SettingsService;

public class SettingsFileRepository(
        IFileSystem fs,
        IReadOnlyTenantProvider tenantProvider,
        ILogger<SettingsFileRepository> logger)
    : ISettingsRepository
{
    private readonly IFileSystem fs = fs;
    private readonly IReadOnlyTenantProvider tenantProvider = tenantProvider;
    private readonly ILogger logger = logger;

    public async Task SaveSettingsAsync(SettingsModel settings)
    {
        var tenant = tenantProvider.Tenant;

        var dir = fs.Path.Combine("settings", tenant);
        fs.Directory.CreateDirectory(dir);

        var file = fs.Path.Combine(dir, "settings.json");

        logger.LogInformation("Writing settings to {File}", file);

        await fs.File.WriteAllTextAsync(file, JsonSerializer.Serialize(settings));
    }
}