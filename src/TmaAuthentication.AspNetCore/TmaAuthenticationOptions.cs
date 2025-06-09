using Microsoft.AspNetCore.Authentication;

namespace TmaAuthentication.AspNetCore;

public class TmaAuthenticationOptions : AuthenticationSchemeOptions
{
    public string BotToken { get; set; } = string.Empty;
    
    public TimeSpan ExpirationInterval { get; set; } = TimeSpan.FromDays(1);
}