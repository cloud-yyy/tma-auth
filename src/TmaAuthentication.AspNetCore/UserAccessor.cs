using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TmaAuth.Models;

namespace TmaAuthentication.AspNetCore;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal? _user;

    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _user ??= _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public long? UserId => GetClaimValue<long?>(ClaimTypes.NameIdentifier, value => 
        long.TryParse(value, out var result) ? result : null);

    public string? FirstName => GetClaimValue(ClaimTypes.Name);

    public string? LastName => GetClaimValue(ClaimTypes.Surname);

    public string? Username => GetClaimValue("username");

    public string? LanguageCode => GetClaimValue("language_code");

    public string? PhotoUrl => GetClaimValue("photo_url");

    public bool? IsPremium => GetClaimValue<bool?>("is_premium", value => 
        bool.TryParse(value, out var result) ? result : null);

    public bool? IsBot => GetClaimValue<bool?>("is_bot", value => 
        bool.TryParse(value, out var result) ? result : null);

    public bool? AllowsWriteToPm => GetClaimValue<bool?>("allows_write_to_pm", value => 
        bool.TryParse(value, out var result) ? result : null);

    public bool? AddedToAttachmentMenu => GetClaimValue<bool?>("added_to_attachment_menu", value => 
        bool.TryParse(value, out var result) ? result : null);

    public DateTime? AuthDate => GetClaimValue<DateTime?>("auth_date", value => 
        int.TryParse(value, out var unixTime) ? DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime : null);

    public string? QueryId => GetClaimValue("query_id");

    public string? StartParam => GetClaimValue("start_param");

    public long? ChatInstance => GetClaimValue<long?>("chat_instance", value => 
        long.TryParse(value, out var result) ? result : null);

    public ChatType? ChatType => GetClaimValue<ChatType?>("chat_type", value => 
        Enum.TryParse<ChatType>(value, out var result) ? result : null);

    public string? AuthenticationScheme => User?.Identity?.AuthenticationType;

    private string? GetClaimValue(string claimType)
    {
        return User?.FindFirst(claimType)?.Value;
    }

    private T GetClaimValue<T>(string claimType, Func<string, T> converter)
    {
        var claimValue = GetClaimValue(claimType);
        if (string.IsNullOrEmpty(claimValue))
        {
            return default(T)!;
        }

        try
        {
            return converter(claimValue);
        }
        catch
        {
            return default(T)!;
        }
    }
}