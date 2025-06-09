using TmaAuth.Models;

namespace TmaAuthentication.AspNetCore;

public interface ITmaJwtService
{
    Task<string> GenerateTokenAsync(string initData);
    
    Task<string> GenerateTokenAsync(InitData parsedInitData);
}