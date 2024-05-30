using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Castle.Core.Logging;
using Common;
using Microsoft.Extensions.Logging;
using Moq;
using TestUtils;

namespace UserService.Test;

public class UserFileRepositoryTests
{
    [Theory]
    [AutoMoqData]
    public async Task WritesJsonFileInDirectory(
        Mock<IReadOnlyTenantProvider> mockTenantProvider,
        string tenant,
        UserModel model)
    {
        MockFileSystem fs = new ();
        mockTenantProvider.Setup(t => t.Tenant).Returns(tenant);

        UserFileRepository sut = new UserFileRepository(
            fs.FileSystem,
            mockTenantProvider.Object,
            Mock.Of<ILogger<UserFileRepository>>());
        
        await sut.SaveUserAsync(model);
        
        var file = await fs.File.ReadAllTextAsync($"/users/{tenant}/{model.Name}.json");
        Assert.Equal(JsonSerializer.Serialize(model), file);
    }
}