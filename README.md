# TmaAuthentication

[![NuGet](https://img.shields.io/nuget/v/TmaAuthentication.svg)](https://www.nuget.org/packages/TmaAuthentication/)
[![GitHub](https://img.shields.io/github/license/cloud-yyy/tma-authentication)](https://github.com/cloud-yyy/tma-authentication)

A C# library for validating Telegram Mini App initialization data.

## Overview

TmaAuthentication is a C# library for working with Telegram Mini App initialization data. It provides functionality to:

- Parse initialization data from a query string format
- Validate the initialization data using the bot token
- Sign initialization data for testing or other purposes
- ASP.NET Core authentication infrastructure with JWT support

## Installation

Add the NuGet package to your project:

```bash
# Core library
dotnet add package TmaAuthentication

# ASP.NET Core integration
dotnet add package TmaAuthentication.AspNetCore
```

## Usage

### Validating Initialization Data

The validator helps you verify if the initialization data received from Telegram is authentic and hasn't been tampered with.

```csharp
// Create validator instance
var validator = new TmaInitDataValidator();

// Validate initialization data with 24-hour expiration
var initData = "query_id=AAHdF6IQAAAAAN0XohDhrOrc&user=%7B%22id%22%3A279058397%2C%22first_name%22%3A%22John%22%2C%22last_name%22%3A%22Doe%22%7D&auth_date=1713542400&hash=abc123...";
var isValid = validator.Validate(initData, "YOUR_BOT_TOKEN", TimeSpan.FromHours(24));

if (isValid)
{
    Console.WriteLine("Initialization data is valid");
}
else
{
    Console.WriteLine("Initialization data is invalid");
}
```

### Creating Signatures

The signer helps you create valid signatures for testing or when you need to generate initialization data programmatically.

```csharp
// Create signer instance
var signer = new TmaInitDataSigner();

// Create a signature from parameters
var parameters = new Dictionary<string, string>
{
    { "user", "{\"id\":123456789,\"first_name\":\"John\",\"last_name\":\"Doe\"}" },
    { "query_id", "AAHdF6IQAAAAAN0XohDhrOrc" },
    { "auth_date", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
};
var hash = signer.Sign(parameters, "YOUR_BOT_TOKEN");
```

## ASP.NET Core Integration

The `TmaAuthentication.AspNetCore` package provides seamless integration with ASP.NET Core authentication system, supporting both direct init data validation and JWT-based authentication.

### Direct Init Data Authentication

Configure direct init data authentication in your ASP.NET Core application:

```csharp
builder.Services.AddTmaAuthentication(options =>
{
    options.BotToken = "YOUR_BOT_TOKEN";
    options.ExpirationInterval = TimeSpan.FromDays(7);
});

// Use the authentication
app.UseAuthentication();
app.UseAuthorization();
```

Clients send requests with the `TAuth` scheme:

```
Authorization: TAuth query_id=AAHdF6IQAAAAAN0XohDhrOrc&user=%7B%22id%22%3A279058397...
```

### JWT-Based Authentication

Configure JWT authentication for better scalability:

```csharp
builder.Services.AddTmaJwtAuthentication(options =>
{
    options.BotToken = "YOUR_BOT_TOKEN";
    options.InitDataExpirationInterval = TimeSpan.FromDays(7);
    options.SecretKey = "YOUR_JWT_SECRET_KEY";
    options.TokenExpiration = TimeSpan.FromHours(24);
    options.EnableBuiltInEndpoint = true; // Optional built-in token endpoint
});
```

The JWT workflow involves two steps:

1. **Token Generation**: POST `/auth/tma-token` with init data to get JWT
2. **Authentication**: Use standard Bearer token authentication

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Accessing Current User

Use `IUserAccessor` to access current user information in controllers or services:

```csharp
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserAccessor _userAccessor;
    
    public UserController(IUserAccessor userAccessor)
    {
        _userAccessor = userAccessor;
    }
    
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        if (!_userAccessor.IsAuthenticated)
            return Unauthorized();
            
        return Ok(new {
            UserId = _userAccessor.UserId,
            Name = _userAccessor.FirstName,
            Username = _userAccessor.Username,
            IsPremium = _userAccessor.IsPremium,
            AuthScheme = _userAccessor.AuthenticationScheme
        });
    }
}
```

### Custom JWT Token Generation

You can also use `ITmaJwtService` for custom token generation:

```csharp
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITmaJwtService _jwtService;
    
    [HttpPost("custom-token")]
    public async Task<IActionResult> GenerateToken([FromBody] string initData)
    {
        try
        {
            var token = await _jwtService.GenerateTokenAsync(initData);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid init data");
        }
    }
}
```

## License

This library is open source and available under the MIT license.