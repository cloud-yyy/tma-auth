using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TmaAuth;
using TmaAuth.Abstractions;
using TmaAuth.Models;
using TmaAuthentication.AspNetCore.Abstractions;
using TmaAuthentication.AspNetCore.Options;

namespace TmaAuthentication.AspNetCore;

public class TmaJwtService : ITmaJwtService
{
    private readonly TmaJwtOptions _options;
    private readonly ITmaInitDataValidator _validator;
    private readonly ITmaInitDataParser _parser;

    public TmaJwtService(
        IOptionsMonitor<TmaJwtOptions> options,
        ITmaInitDataValidator validator,
        ITmaInitDataParser parser)
    {
        _options = options.Get(TmaJwtDefaults.AuthenticationScheme);
        _validator = validator;
        _parser = parser;
    }

    public async Task<string> GenerateTokenAsync(string initData)
    {
        if (string.IsNullOrEmpty(initData))
            throw new ArgumentException("Init data cannot be null or empty", nameof(initData));

        if (string.IsNullOrEmpty(_options.BotToken))
            throw new InvalidOperationException("Bot token is not configured");

        if (string.IsNullOrEmpty(_options.SecretKey))
            throw new InvalidOperationException("JWT secret key is not configured");

        if (_options.TokenExpiration <= TimeSpan.Zero)
            throw new InvalidOperationException("JWT token expiration must be configured and greater than zero");

        var isValid = _validator.Validate(initData, _options.BotToken, _options.InitDataExpirationInterval);
        if (!isValid)
            throw new UnauthorizedAccessException("Invalid init data");

        var parsedInitData = _parser.Parse(initData);
        
        return await GenerateTokenAsync(parsedInitData);
    }

    public async Task<string> GenerateTokenAsync(InitData parsedInitData)
    {
        if (parsedInitData == null)
            throw new ArgumentNullException(nameof(parsedInitData));

        if (string.IsNullOrEmpty(_options.SecretKey))
            throw new InvalidOperationException("JWT secret key is not configured");

        if (_options.TokenExpiration <= TimeSpan.Zero)
            throw new InvalidOperationException("JWT token expiration must be configured and greater than zero");

        var claims = CreateClaimsFromInitData(parsedInitData);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_options.TokenExpiration),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return await Task.FromResult(tokenHandler.WriteToken(token));
    }

    private static List<Claim> CreateClaimsFromInitData(InitData initData)
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
        
        if (initData.ChatType != ChatType.Unknown)
        {
            claims.Add(new Claim("chat_type", initData.ChatType.ToString()));
        }

        return claims;
    }
}