using Microsoft.AspNetCore.Authentication;

namespace TmaAuthentication.AspNetCore;

public class TmaJwtOptions : AuthenticationSchemeOptions
{
    public string BotToken { get; set; } = string.Empty;
    
    public TimeSpan InitDataExpirationInterval { get; set; } = TimeSpan.FromDays(1);
    
    public string SecretKey { get; set; } = string.Empty;
    
    public TimeSpan TokenExpiration { get; set; } = TimeSpan.Zero;
    
    public string Issuer { get; set; } = "TmaAuthentication";
    
    public string Audience { get; set; } = "TmaClient";
    
    public bool EnableBuiltInEndpoint { get; set; } = false;
    
    public string TokenEndpoint { get; set; } = TmaJwtDefaults.TokenEndpoint;
}