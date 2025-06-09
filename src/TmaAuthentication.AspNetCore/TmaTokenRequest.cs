namespace TmaAuthentication.AspNetCore;

public class TmaTokenRequest
{
    public string InitData { get; set; } = string.Empty;
}

public class TmaTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}