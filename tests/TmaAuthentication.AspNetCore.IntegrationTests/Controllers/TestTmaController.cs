using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TmaAuthentication.AspNetCore.Abstractions;
using TmaAuthentication.AspNetCore.Defaults;

namespace TmaAuthentication.AspNetCore.IntegrationTests.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestTmaController : ControllerBase
{
    private readonly IUserAccessor _userAccessor;

    public TestTmaController(IUserAccessor userAccessor)
    {
        _userAccessor = userAccessor;
    }

    [HttpGet("public")]
    public IActionResult GetPublic()
    {
        return Ok(new { message = "Public endpoint", authenticated = _userAccessor.IsAuthenticated });
    }

    [HttpGet("protected")]
    [Authorize(AuthenticationSchemes = TmaAuthenticationDefaults.AuthenticationScheme)]
    public IActionResult GetProtected()
    {
        return Ok(new 
        { 
            message = "Protected endpoint",
            userId = _userAccessor.UserId,
            firstName = _userAccessor.FirstName,
            authenticated = _userAccessor.IsAuthenticated,
            scheme = _userAccessor.AuthenticationScheme
        });
    }

    [HttpGet("jwt-protected")]
    [Authorize(AuthenticationSchemes = TmaJwtDefaults.AuthenticationScheme)]
    public IActionResult GetJwtProtected()
    {
        return Ok(new 
        { 
            message = "JWT Protected endpoint",
            userId = _userAccessor.UserId,
            firstName = _userAccessor.FirstName,
            authenticated = _userAccessor.IsAuthenticated,
            scheme = _userAccessor.AuthenticationScheme
        });
    }

    [HttpGet("user-info")]
    [Authorize(AuthenticationSchemes = TmaAuthenticationDefaults.AuthenticationScheme)]
    public IActionResult GetUserInfo()
    {
        return Ok(new 
        { 
            userId = _userAccessor.UserId,
            firstName = _userAccessor.FirstName,
            lastName = _userAccessor.LastName,
            username = _userAccessor.Username,
            languageCode = _userAccessor.LanguageCode,
            photoUrl = _userAccessor.PhotoUrl,
            isPremium = _userAccessor.IsPremium,
            isBot = _userAccessor.IsBot,
            allowsWriteToPm = _userAccessor.AllowsWriteToPm,
            addedToAttachmentMenu = _userAccessor.AddedToAttachmentMenu,
            authDate = _userAccessor.AuthDate,
            queryId = _userAccessor.QueryId,
            startParam = _userAccessor.StartParam,
            chatInstance = _userAccessor.ChatInstance,
            chatType = _userAccessor.ChatType?.ToString(),
            authenticationScheme = _userAccessor.AuthenticationScheme
        });
    }
}