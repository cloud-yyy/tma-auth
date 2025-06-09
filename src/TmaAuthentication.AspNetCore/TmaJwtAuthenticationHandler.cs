using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace TmaAuthentication.AspNetCore;

public class TmaJwtAuthenticationHandler : AuthenticationHandler<TmaJwtOptions>
{
    public TmaJwtAuthenticationHandler(
        IOptionsMonitor<TmaJwtOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers[HeaderNames.Authorization].ToString();

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        if (string.IsNullOrEmpty(token))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Bearer token format"));
        }

        if (string.IsNullOrEmpty(Options.SecretKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("JWT secret key is not configured"));
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.SecretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = Options.Issuer,
                ValidateAudience = true,
                ValidAudience = Options.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken || 
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid token algorithm"));
            }

            var identity = new ClaimsIdentity(principal.Claims, Scheme.Name);
            var newPrincipal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(newPrincipal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (SecurityTokenExpiredException)
        {
            return Task.FromResult(AuthenticateResult.Fail("Token has expired"));
        }
        catch (SecurityTokenException ex)
        {
            Logger.LogError(ex, "JWT token validation failed");
            return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during JWT authentication");
            return Task.FromResult(AuthenticateResult.Fail("Authentication error"));
        }
    }
}