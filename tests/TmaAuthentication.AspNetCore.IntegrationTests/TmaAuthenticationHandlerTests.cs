using System.Net;
using System.Net.Http.Json;
using TmaAuthentication.AspNetCore.IntegrationTests.Infrastructure;
using TmaAuth.Models;
using TmaAuthentication.AspNetCore.Defaults;

namespace TmaAuthentication.AspNetCore.IntegrationTests;

public record UserInfoResponse(
    long? UserId,
    string? FirstName,
    string? LastName,
    string? Username,
    string? LanguageCode,
    string? PhotoUrl,
    bool? IsPremium,
    bool? IsBot,
    bool? AllowsWriteToPm,
    bool? AddedToAttachmentMenu,
    DateTime? AuthDate,
    string? QueryId,
    string? StartParam,
    long? ChatInstance,
    string? ChatType,
    string? AuthenticationScheme
);

public record ProtectedResponse(
    string Message,
    long? UserId,
    string? FirstName,
    bool Authenticated,
    string? Scheme
);

public class TmaAuthenticationHandlerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TmaAuthenticationHandlerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task AuthenticateAsync_ValidTAuthHeader_ShouldReturnUserInfo()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", $"TAuth {TestConstants.ValidInitData}");

        // Act
        var response = await _client.GetAsync("/api/testtma/user-info");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var userInfo = await response.Content.ReadFromJsonAsync<UserInfoResponse>();
        Assert.NotNull(userInfo);
        
        Assert.Equal(TestConstants.ExpectedUserId, userInfo.UserId);
        Assert.Equal(TestConstants.ExpectedFirstName, userInfo.FirstName);
        Assert.Equal(TestConstants.ExpectedLastName, userInfo.LastName);
        Assert.Equal(TestConstants.ExpectedUsername, userInfo.Username);
        Assert.Equal(TestConstants.ExpectedLanguageCode, userInfo.LanguageCode);
        Assert.Equal(TestConstants.ExpectedIsPremium, userInfo.IsPremium);
        Assert.Equal("AAHdF6IQAAAAAN0XohDhrOrc", userInfo.QueryId);
        Assert.Equal(TmaAuthenticationDefaults.AuthenticationScheme, userInfo.AuthenticationScheme);
    }

    [Fact]
    public async Task AuthenticateAsync_NoAuthorizationHeader_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateAsync_WrongScheme_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {TestConstants.ValidInitData}");

        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateAsync_EmptyTAuthValue_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", "TAuth ");

        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidInitData_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", $"TAuth {TestConstants.InvalidInitData}");

        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateAsync_PublicEndpoint_ShouldAlwaysWork()
    {
        // Act
        var response = await _client.GetAsync("/api/testtma/public");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AuthenticateAsync_TamperedInitData_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", $"TAuth {TestConstants.TamperedInitData}");

        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticateAsync_CaseInsensitiveScheme_ShouldWork()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", $"tauth {TestConstants.ValidInitData}");

        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ProtectedResponse>();
        Assert.NotNull(result);
        Assert.True(result.Authenticated);
        Assert.Equal(TestConstants.ExpectedUserId, result.UserId);
    }

    [Fact]
    public async Task AuthenticateAsync_TrimmedInitData_ShouldWork()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", $"TAuth  {TestConstants.ValidInitData}  ");

        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ProtectedResponse>();
        Assert.NotNull(result);
        Assert.True(result.Authenticated);
        Assert.Equal(TestConstants.ExpectedUserId, result.UserId);
    }
}