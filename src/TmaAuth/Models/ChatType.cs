namespace TmaAuth.Models;

/// <summary>
/// ChatType describes type of chat.
/// </summary>
public enum ChatType
{
    /// <summary>
    /// Sender chat type
    /// </summary>
    Sender,

    /// <summary>
    /// Private chat type
    /// </summary>
    Private,

    /// <summary>
    /// Group chat type
    /// </summary>
    Group,

    /// <summary>
    /// Supergroup chat type
    /// </summary>
    Supergroup,

    /// <summary>
    /// Channel chat type
    /// </summary>
    Channel,

    /// <summary>
    /// Unknown chat type
    /// </summary>
    Unknown
}
