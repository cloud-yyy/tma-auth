using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TmaAuthentication.AspNetCore;

[ApiController]
public class TmaTokenController : ControllerBase
{
    private readonly ITmaJwtService _jwtService;
    private readonly TmaJwtOptions _options;

    public TmaTokenController(ITmaJwtService jwtService, IOptionsMonitor<TmaJwtOptions> options)
    {
        _jwtService = jwtService;
        _options = options.CurrentValue;
    }

    [HttpPost]
    [Route("/auth/tma-token")]
    public async Task<IActionResult> GenerateToken([FromBody] TmaTokenRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.InitData))
            {
                return BadRequest(new { error = "Init data is required" });
            }

            var token = await _jwtService.GenerateTokenAsync(request.InitData);
            var expiresAt = DateTime.UtcNow.Add(_options.TokenExpiration);

            return Ok(new TmaTokenResponse
            {
                Token = token,
                ExpiresAt = expiresAt
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Invalid init data" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}