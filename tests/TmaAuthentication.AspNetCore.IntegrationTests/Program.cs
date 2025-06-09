using TmaAuthentication.AspNetCore;
using TmaAuthentication.AspNetCore.IntegrationTests.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Add TMA authentication services
builder.Services.AddTmaAuthentication(options =>
{
    options.BotToken = TestConstants.ValidBotToken;
    options.ExpirationInterval = TimeSpan.Zero;
});

builder.Services.AddTmaJwtAuthentication(options =>
{
    options.SecretKey = TestConstants.JwtSecretKey;
    options.BotToken = TestConstants.ValidBotToken;
    options.Issuer = "test-issuer";
    options.Audience = "test-audience";
});

// Add UserAccessor
builder.Services.AddScoped<IUserAccessor, UserAccessor>();

var app = builder.Build();

// Configure pipeline
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make the implicit Program class available for WebApplicationFactory
public partial class Program { }