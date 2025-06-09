using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TmaAuthentication.AspNetCore;
using TmaAuthentication.AspNetCore.IntegrationTests.Infrastructure;
using TmaAuth.Abstractions;

namespace TmaAuthentication.AspNetCore.IntegrationTests;

public class BasicServiceTests
{
    [Fact]
    public void AddTmaAuthentication_ShouldRegisterAllRequiredServices()
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

        // Assert - Core TMA services
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataValidator>());
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataParser>());
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataSigner>());
        
        // Assert - ASP.NET Core integration services
        Assert.NotNull(serviceProvider.GetService<IUserAccessor>());
        
        // Assert - Options configuration
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<TmaAuthenticationOptions>>();
        var tmaOptions = options.Get(TmaAuthenticationDefaults.AuthenticationScheme);
        Assert.Equal(TestConstants.ValidBotToken, tmaOptions.BotToken);
        Assert.Equal(TimeSpan.FromHours(24), tmaOptions.ExpirationInterval);
    }

    [Fact]
    public void AddTmaJwtAuthentication_ShouldRegisterAllRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTmaJwtAuthentication(options =>
        {
            options.BotToken = TestConstants.ValidBotToken;
            options.SecretKey = TestConstants.JwtSecretKey;
            options.TokenExpiration = TimeSpan.FromHours(1);
            options.InitDataExpirationInterval = TimeSpan.FromDays(7);
            options.EnableBuiltInEndpoint = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert - Core TMA services
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataValidator>());
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataParser>());
        Assert.NotNull(serviceProvider.GetService<ITmaInitDataSigner>());
        
        // Assert - ASP.NET Core integration services
        Assert.NotNull(serviceProvider.GetService<IUserAccessor>());
        Assert.NotNull(serviceProvider.GetService<ITmaJwtService>());
        
        // Assert - Options configuration
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<TmaJwtOptions>>();
        var jwtOptions = options.Get(TmaJwtDefaults.AuthenticationScheme);
        Assert.Equal(TestConstants.ValidBotToken, jwtOptions.BotToken);
        Assert.Equal(TestConstants.JwtSecretKey, jwtOptions.SecretKey);
        Assert.Equal(TimeSpan.FromHours(1), jwtOptions.TokenExpiration);
        Assert.Equal(TimeSpan.FromDays(7), jwtOptions.InitDataExpirationInterval);
        Assert.True(jwtOptions.EnableBuiltInEndpoint);
    }

    [Fact]
    public void ServiceLifetimes_ShouldBeCorrect()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddTmaJwtAuthentication(options =>
        {
            options.BotToken = TestConstants.ValidBotToken;
            options.SecretKey = TestConstants.JwtSecretKey;
        });

        // Assert
        var validatorDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITmaInitDataValidator));
        var parserDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITmaInitDataParser));
        var signerDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITmaInitDataSigner));
        var userAccessorDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserAccessor));
        var jwtServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ITmaJwtService));

        Assert.Equal(ServiceLifetime.Singleton, validatorDescriptor?.Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, parserDescriptor?.Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, signerDescriptor?.Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, userAccessorDescriptor?.Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, jwtServiceDescriptor?.Lifetime);
    }
}