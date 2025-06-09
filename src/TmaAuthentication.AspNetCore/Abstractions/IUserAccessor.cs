using TmaAuth.Models;

namespace TmaAuthentication.AspNetCore.Abstractions;

public interface IUserAccessor
{
    bool IsAuthenticated { get; }
    
    long? UserId { get; }
    
    string? FirstName { get; }
    
    string? LastName { get; }
    
    string? Username { get; }
    
    string? LanguageCode { get; }
    
    string? PhotoUrl { get; }
    
    bool? IsPremium { get; }
    
    bool? IsBot { get; }
    
    bool? AllowsWriteToPm { get; }
    
    bool? AddedToAttachmentMenu { get; }
    
    DateTime? AuthDate { get; }
    
    string? QueryId { get; }
    
    string? StartParam { get; }
    
    long? ChatInstance { get; }
    
    ChatType? ChatType { get; }
    
    string? AuthenticationScheme { get; }
}