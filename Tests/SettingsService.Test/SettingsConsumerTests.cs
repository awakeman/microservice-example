using System.Text.Json;
using AutoFixture.Xunit2;
using Common;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TestUtils;

namespace SettingsService.Test;

public class SettingsConsumerTests
{
    [Theory]
    [AutoMoqData]
    public async Task ConsumeSetsTenantAndCallsSave(
        [Frozen] Mock<IServiceScopeFactory> mockScopeFactory,
        Mock<ITenantProvider> mockTenantProvider,
        Mock<ISettingsRepository> mockSettingsRepo,
        Mock<IServiceScope> mockScope,
        Mock<IServiceProvider> mockServiceProvider,
        Mock<ConsumeContext<Message>> mockContext,
        SettingsModel model,
        string tenant,
        SettingsConsumer sut)
    {
        mockServiceProvider.Setup(p => p.GetService(typeof(ITenantProvider))).Returns(mockTenantProvider.Object);
        mockServiceProvider.Setup(p => p.GetService(typeof(ISettingsRepository))).Returns(mockSettingsRepo.Object);
        mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);
        mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);

        Message msg = new () { Body = JsonSerializer.Serialize(model), Tenant = tenant };

        mockContext.Setup(c => c.Message).Returns(msg);

        await sut.Consume(mockContext.Object);
        
        mockTenantProvider.VerifySet(t => t.Tenant = msg.Tenant, Times.Once);
        mockSettingsRepo.Verify(r => r.SaveSettingsAsync(model), Times.Once);
    }

    [Theory]
    [InlineAutoMoqData("")]
    [InlineAutoMoqData("null")]
    public async Task ConsumeBailsIfNull(
        string body,
        [Frozen] Mock<IServiceScopeFactory> mockScopeFactory,
        Mock<ITenantProvider> mockTenantProvider,
        Mock<ISettingsRepository> mockSettingsRepo,
        Mock<IServiceScope> mockScope,
        Mock<IServiceProvider> mockServiceProvider,
        Mock<ConsumeContext<Message>> mockContext,
        string tenant,
        SettingsConsumer sut)
    {
        mockServiceProvider.Setup(p => p.GetService(typeof(ITenantProvider))).Returns(mockTenantProvider.Object);
        mockServiceProvider.Setup(p => p.GetService(typeof(ISettingsRepository))).Returns(mockSettingsRepo.Object);
        mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);
        mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);

        Message msg = new () { Body = body, Tenant = tenant };

        mockContext.Setup(c => c.Message).Returns(msg);

        await sut.Consume(mockContext.Object);
        
        mockTenantProvider.VerifySet(t => t.Tenant = msg.Tenant, Times.Never);
        mockSettingsRepo.Verify(r => r.SaveSettingsAsync(It.IsAny<SettingsModel>()), Times.Never);
    }
}