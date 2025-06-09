using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using TmaAuth;
using TmaAuth.Abstractions;

namespace TmaAuthentication.AspNetCore;

public class TmaAuthenticationHandler : AuthenticationHandler<TmaAuthenticationOptions>
{
    private readonly ITmaInitDataValidator _validator;

    public TmaAuthenticationHandler(
        IOptionsMonitor<TmaAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ITmaInitDataValidator validator)
        : base(options, logger, encoder)
    {
        _validator = validator;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers[HeaderNames.Authorization].ToString();

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!authorizationHeader.StartsWith("TAuth ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var initData = authorizationHeader.Substring("TAuth ".Length).Trim();

        if (string.IsNullOrEmpty(initData))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid TAuth header format"));
        }

        if (string.IsNullOrEmpty(Options.BotToken))
        {
            return Task.FromResult(AuthenticateResult.Fail("Bot token is not configured"));
        }

        try
        {
            var isValid = _validator.Validate(initData, Options.BotToken, Options.ExpirationInterval);

            if (!isValid)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid init data"));
            }

            var parser = new TmaInitDataParser();
            var parsedInitData = parser.Parse(initData);

            var claims = CreateClaimsFromInitData(parsedInitData);
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during TMA authentication");
            return Task.FromResult(AuthenticateResult.Fail("Authentication error"));
        }
    }

    private static List<Claim> CreateClaimsFromInitData(TmaAuth.Models.InitData initData)
    {
        var claims = new List<Claim>();

        if (initData.User != null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, initData.User.ID.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, initData.User.FirstName));
            
            if (!string.IsNullOrEmpty(initData.User.LastName))
            {
                claims.Add(new Claim(ClaimTypes.Surname, initData.User.LastName));
            }
            
            if (!string.IsNullOrEmpty(initData.User.Username))
            {
                claims.Add(new Claim("username", initData.User.Username));
            }
            
            if (!string.IsNullOrEmpty(initData.User.LanguageCode))
            {
                claims.Add(new Claim("language_code", initData.User.LanguageCode));
            }
            
            if (!string.IsNullOrEmpty(initData.User.PhotoURL))
            {
                claims.Add(new Claim("photo_url", initData.User.PhotoURL));
            }
            
            claims.Add(new Claim("is_premium", initData.User.IsPremium.ToString()));
            claims.Add(new Claim("is_bot", initData.User.IsBot.ToString()));
            claims.Add(new Claim("allows_write_to_pm", initData.User.AllowsWriteToPm.ToString()));
            claims.Add(new Claim("added_to_attachment_menu", initData.User.AddedToAttachmentMenu.ToString()));
        }

        claims.Add(new Claim("auth_date", initData.AuthDateRaw.ToString()));
        
        if (!string.IsNullOrEmpty(initData.QueryID))
        {
            claims.Add(new Claim("query_id", initData.QueryID));
        }
        
        if (!string.IsNullOrEmpty(initData.StartParam))
        {
            claims.Add(new Claim("start_param", initData.StartParam));
        }
        
        if (initData.ChatInstance != 0)
        {
            claims.Add(new Claim("chat_instance", initData.ChatInstance.ToString()));
        }
        
        if (initData.ChatType != TmaAuth.Models.ChatType.Unknown)
        {
            claims.Add(new Claim("chat_type", initData.ChatType.ToString()));
        }

        return claims;
    }
}