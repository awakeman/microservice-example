using System.IO.Abstractions;
using System.Text.Json;
using Common;

namespace SettingsService;

public class SettingsFileRepository : ISettingsRepository
{
    readonly IFileSystem fs;
    readonly IReadOnlyTenantProvider tenantProvider;

    public SettingsFileRepository(IFileSystem fs, IReadOnlyTenantProvider tenantProvider)
    {
        this.fs = fs;
        this.tenantProvider = tenantProvider;
    }

    public async Task SaveSettingsAsync(SettingsModel settings)
    {
        var tenant = tenantProvider.Tenant;

        var dir = fs.Path.Combine("settings", tenant);
        fs.Directory.CreateDirectory(dir);

        await fs.File.WriteAllTextAsync(fs.Path.Combine(dir, "settings.json"), JsonSerializer.Serialize(settings));
    }
}