using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Castle.Core.Logging;
using Common;
using Microsoft.Extensions.Logging;
using Moq;
using TestUtils;

namespace SettingsService.Test;

public class SettingsFileRepositoryTests
{
    [Theory]
    [AutoMoqData]
    public async Task WritesJsonFileInDirectory(
        Mock<IReadOnlyTenantProvider> mockTenantProvider,
        Mock<INotificationClient> mockNotificationClient,
        string tenant,
        SettingsModel model)
    {
        MockFileSystem fs = new ();
        mockTenantProvider.Setup(t => t.Tenant).Returns(tenant);

        SettingsFileRepository sut = new SettingsFileRepository(
            fs.FileSystem,
            mockTenantProvider.Object,
            mockNotificationClient.Object,
            Mock.Of<ILogger<SettingsFileRepository>>());
        
        await sut.SaveSettingsAsync(model);
        
        var file = await fs.File.ReadAllTextAsync($"/settings/{tenant}/settings.json");
        Assert.Equal(JsonSerializer.Serialize(model), file);
        
        mockNotificationClient.Verify(c => c.NotifyAsync(model), Times.Once);
    }
}