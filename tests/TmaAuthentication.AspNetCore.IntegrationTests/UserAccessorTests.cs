using System.Net;
using System.Net.Http.Json;
using TmaAuthentication.AspNetCore.IntegrationTests.Infrastructure;
using TmaAuth.Models;

namespace TmaAuthentication.AspNetCore.IntegrationTests;

public class UserAccessorTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UserAccessorTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task UserAccessor_WithAuthenticatedTAuthUser_ShouldReturnCorrectValues()
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

        // Test auth date conversion
        var expectedAuthDate = DateTimeOffset.FromUnixTimeSeconds(1662771648).DateTime;
        Assert.Equal(expectedAuthDate, userInfo.AuthDate);
    }

    [Fact]
    public async Task UserAccessor_WithUnauthenticatedUser_ShouldReturnDefaultValues()
    {
        // Act - call public endpoint without authentication
        var response = await _client.GetAsync("/api/testtma/public");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(result);
        // The public endpoint should show not authenticated
    }

    [Fact]
    public async Task UserAccessor_WithInvalidTAuthData_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", $"TAuth {TestConstants.InvalidInitData}");

        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UserAccessor_WithTamperedData_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", $"TAuth {TestConstants.TamperedInitData}");

        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UserAccessor_WithValidTAuthData_ProtectedEndpointReturnsData()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", $"TAuth {TestConstants.ValidInitData}");

        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ProtectedResponse>();
        Assert.NotNull(result);
        Assert.True(result.Authenticated);
        Assert.Equal(TestConstants.ExpectedUserId, result.UserId);
        Assert.Equal(TestConstants.ExpectedFirstName, result.FirstName);
        Assert.Equal(TmaAuthenticationDefaults.AuthenticationScheme, result.Scheme);
    }

    [Fact]
    public async Task UserAccessor_CaseInsensitiveAuthScheme_ShouldWork()
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
    public async Task UserAccessor_WithWhitespaceInAuthHeader_ShouldWork()
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

    [Fact]
    public async Task UserAccessor_EmptyAuthHeader_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", "TAuth ");

        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UserAccessor_WrongAuthScheme_ShouldReturnUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {TestConstants.ValidInitData}");

        // Act
        var response = await _client.GetAsync("/api/testtma/protected");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}