using System.IO.Abstractions;
using System.Text.Json;
using Common;

namespace UserService;

public class UserFileRepository(
        IFileSystem fs,
        IReadOnlyTenantProvider tenantProvider,
        INotificationClient client,
        ILogger<UserFileRepository> logger)
    : IUserRepository
{
    private readonly IFileSystem fs = fs;
    private readonly IReadOnlyTenantProvider tenantProvider = tenantProvider;
    private readonly ILogger logger = logger;

    public async Task SaveUserAsync(UserModel user)
    {
        var tenant = tenantProvider.Tenant;

        var dir = fs.Path.Combine("users", tenant);
        fs.Directory.CreateDirectory(dir);

        var file = fs.Path.Combine(dir, $"{user.Name}.json");

        logger.LogInformation("Writing user to {File}", file);

        var content = JsonSerializer.Serialize(user);
        await fs.File.WriteAllTextAsync(file, content);
        
        // Notify that the model was saved
        await client.NotifyAsync(user);
    }
}