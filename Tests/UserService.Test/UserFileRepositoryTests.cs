using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
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
        Mock<INotificationClient> mockNotificationClient,
        string tenant,
        UserModel model)
    {
        MockFileSystem fs = new ();
        mockTenantProvider.Setup(t => t.Tenant).Returns(tenant);

        UserFileRepository sut = new UserFileRepository(
            fs.FileSystem,
            mockTenantProvider.Object,
            mockNotificationClient.Object,
            Mock.Of<ILogger<UserFileRepository>>());
        
        await sut.SaveUserAsync(model);
        
        var file = await fs.File.ReadAllTextAsync($"/users/{tenant}/{model.Name}.json");
        Assert.Equal(JsonSerializer.Serialize(model), file);
        
        mockNotificationClient.Verify(c => c.NotifyAsync(model), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task WriteFailsWontNotifySuccess(
        Mock<IReadOnlyTenantProvider> mockTenantProvider,
        Mock<INotificationClient> mockNotificationClient,
        Mock<IFileSystem> mockFileSystem,
        Mock<IFile> mockFileInterface,
        string tenant,
        UserModel model)
    {
        mockFileSystem.Setup(f => f.File).Returns(mockFileInterface.Object);
        mockFileInterface.Setup(f => f.WriteAllTextAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromException(new IOException()));
        

        mockTenantProvider.Setup(t => t.Tenant).Returns(tenant);

        UserFileRepository sut = new UserFileRepository(
            mockFileSystem.Object,
            mockTenantProvider.Object,
            mockNotificationClient.Object,
            Mock.Of<ILogger<UserFileRepository>>());
        
        await Assert.ThrowsAsync<IOException>(() => sut.SaveUserAsync(model));
        
        mockNotificationClient.Verify(c => c.NotifyAsync(model), Times.Never);
    }
}