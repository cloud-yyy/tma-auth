using Microsoft.Extensions.DependencyInjection;
using TmaAuthentication.AspNetCore;
using TmaAuthentication.AspNetCore.IntegrationTests.Infrastructure;
using TmaAuth.Abstractions;

namespace TmaAuthentication.AspNetCore.IntegrationTests;

public class SimpleIntegrationTests
{
    [Fact]
    public void AddTmaAuthentication_ShouldRegisterRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTmaAuthentication(options =>
        {
            options.BotToken = TestConstants.ValidBotToken;
            options.ExpirationInterval = TimeSpan.FromHours(24);
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataValidator>());
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataParser>());
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataSigner>());
        Assert.NotNull(serviceProvider.GetService<IUserAccessor>());
    }

    [Fact]
    public void AddTmaJwtAuthentication_ShouldRegisterRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTmaJwtAuthentication(options =>
        {
            options.BotToken = TestConstants.ValidBotToken;
            options.SecretKey = TestConstants.JwtSecretKey;
            options.TokenExpiration = TimeSpan.FromHours(1);
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataValidator>());
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataParser>());
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataSigner>());
        Assert.NotNull(serviceProvider.GetService<IUserAccessor>());
        Assert.NotNull(serviceProvider.GetService<ITmaJwtService>());
    }

    [Fact]
    public async Task TmaJwtService_ValidInitData_ShouldGenerateToken()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTmaJwtAuthentication(options =>
        {
            options.BotToken = TestConstants.ValidBotToken;
            options.InitDataExpirationInterval = TimeSpan.Zero;
            options.SecretKey = TestConstants.JwtSecretKey;
            options.TokenExpiration = TimeSpan.FromHours(1);
        });

        var serviceProvider = services.BuildServiceProvider();
        var jwtService = serviceProvider.GetRequiredService<ITmaJwtService>();

        // Act
        var token = await jwtService.GenerateTokenAsync(TestConstants.ValidInitData);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task TmaJwtService_InvalidInitData_ShouldThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTmaJwtAuthentication(options =>
        {
            options.BotToken = TestConstants.ValidBotToken;
            options.InitDataExpirationInterval = TimeSpan.Zero;
            options.SecretKey = TestConstants.JwtSecretKey;
            options.TokenExpiration = TimeSpan.FromHours(1);
        });

        var serviceProvider = services.BuildServiceProvider();
        var jwtService = serviceProvider.GetRequiredService<ITmaJwtService>();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => jwtService.GenerateTokenAsync(TestConstants.InvalidInitData));
    }
}