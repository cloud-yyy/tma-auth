using Microsoft.Extensions.DependencyInjection;
using TmaAuthentication.AspNetCore;
using TmaAuthentication.AspNetCore.IntegrationTests.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TmaAuthentication.AspNetCore.IntegrationTests;

public class TmaJwtServiceTests
{
    [Fact]
    public async Task GenerateTokenAsync_ValidInitData_ShouldReturnValidJwt()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTmaJwtAuthentication(options =>
        {
            options.BotToken = TestConstants.ValidBotToken;
            options.InitDataExpirationInterval = TimeSpan.Zero; // No expiration for tests
            options.SecretKey = TestConstants.JwtSecretKey;
            options.TokenExpiration = TimeSpan.FromHours(1);
        });

        using var serviceProvider = services.BuildServiceProvider();
        var jwtService = serviceProvider.GetRequiredService<ITmaJwtService>();

        // Act
        var token = await jwtService.GenerateTokenAsync(TestConstants.ValidInitData);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Verify JWT structure and claims
        var handler = new JwtSecurityTokenHandler();
        Assert.True(handler.CanReadToken(token));

        var jwt = handler.ReadJwtToken(token);
        
        // Verify expected claims
        Assert.Equal(TestConstants.ExpectedUserId.ToString(), jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal(TestConstants.ExpectedFirstName, jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value);
        Assert.Equal(TestConstants.ExpectedLastName, jwt.Claims.First(c => c.Type == ClaimTypes.Surname).Value);
        Assert.Equal(TestConstants.ExpectedUsername, jwt.Claims.First(c => c.Type == "username").Value);
        Assert.Equal(TestConstants.ExpectedLanguageCode, jwt.Claims.First(c => c.Type == "language_code").Value);
        Assert.Equal(TestConstants.ExpectedIsPremium.ToString(), jwt.Claims.First(c => c.Type == "is_premium").Value);
    }

    [Fact]
    public async Task GenerateTokenAsync_InvalidInitData_ShouldThrowUnauthorizedAccessException()
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

        using var serviceProvider = services.BuildServiceProvider();
        var jwtService = serviceProvider.GetRequiredService<ITmaJwtService>();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => jwtService.GenerateTokenAsync(TestConstants.InvalidInitData));
    }

    [Fact]
    public async Task GenerateTokenAsync_TamperedInitData_ShouldThrowUnauthorizedAccessException()
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

        using var serviceProvider = services.BuildServiceProvider();
        var jwtService = serviceProvider.GetRequiredService<ITmaJwtService>();

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => jwtService.GenerateTokenAsync(TestConstants.TamperedInitData));
    }

    [Fact]
    public async Task GenerateTokenAsync_EmptyInitData_ShouldThrowUnauthorizedAccessException()
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

        using var serviceProvider = services.BuildServiceProvider();
        var jwtService = serviceProvider.GetRequiredService<ITmaJwtService>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => jwtService.GenerateTokenAsync(""));
    }

    [Fact]
    public async Task GenerateTokenAsync_TokenShouldHaveCorrectExpiration()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTmaJwtAuthentication(options =>
        {
            options.BotToken = TestConstants.ValidBotToken;
            options.InitDataExpirationInterval = TimeSpan.Zero;
            options.SecretKey = TestConstants.JwtSecretKey;
            options.TokenExpiration = TimeSpan.FromMinutes(30);
        });

        using var serviceProvider = services.BuildServiceProvider();
        var jwtService = serviceProvider.GetRequiredService<ITmaJwtService>();

        // Act
        var token = await jwtService.GenerateTokenAsync(TestConstants.ValidInitData);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var expClaim = jwt.Claims.First(c => c.Type == "exp");
        var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
        var expectedExpTime = DateTimeOffset.UtcNow.AddMinutes(30);
        
        // Allow 1 minute tolerance for test execution time
        Assert.True(Math.Abs((expTime - expectedExpTime).TotalMinutes) < 1);
    }
}