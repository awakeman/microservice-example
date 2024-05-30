using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Common;
using Moq;
using TestUtils;

namespace SettingsService.Test;

public class SettingsFileRepositoryTests
{
    [Theory]
    [AutoMoqData]
    public async Task WritesJsonFileInDirectory(
        Mock<IReadOnlyTenantProvider> mockTenantProvider,
        string tenant,
        SettingsModel model)
    {
        MockFileSystem fs = new ();
        mockTenantProvider.Setup(t => t.Tenant).Returns(tenant);

        SettingsFileRepository sut = new SettingsFileRepository(fs.FileSystem, mockTenantProvider.Object);
        
        await sut.SaveSettingsAsync(model);
        
        var file = await fs.File.ReadAllTextAsync($"/settings/{tenant}/settings.json");
        Assert.Equal(JsonSerializer.Serialize(model), file);
    }
}