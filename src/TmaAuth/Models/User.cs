using System.Text.Json.Serialization;

namespace TmaAuth.Models;

/// <summary>
/// User describes user information.
/// https://docs.telegram-mini-apps.com/launch-parameters/init-data#user
/// </summary>
public class User
{
    /// <summary>
    /// Optional. True, if this user added the bot to the attachment menu.
    /// </summary>
    [JsonPropertyName("added_to_attachment_menu")]
    public bool AddedToAttachmentMenu { get; set; }

    /// <summary>
    /// Optional. True, if this user allowed the bot to message them.
    /// </summary>
    [JsonPropertyName("allows_write_to_pm")]
    public bool AllowsWriteToPm { get; set; }

    /// <summary>
    /// First name of the user or bot.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// A unique identifier for the user or bot.
    /// </summary>
    [JsonPropertyName("id")]
    public long ID { get; set; }

    /// <summary>
    /// Optional. True, if this user is a bot. Returned in the `receiver` field only.
    /// </summary>
    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }

    /// <summary>
    /// Optional. True, if this user is a Telegram Premium user.
    /// </summary>
    [JsonPropertyName("is_premium")]
    public bool IsPremium { get; set; }

    /// <summary>
    /// Optional. Last name of the user or bot.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    /// <summary>
    /// Optional. Username of the user or bot.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// Optional. IETF language tag of the user's language. Returns in user field only.
    /// https://en.wikipedia.org/wiki/IETF_language_tag
    /// </summary>
    [JsonPropertyName("language_code")]
    public string? LanguageCode { get; set; }

    /// <summary>
    /// Optional. URL of the user's profile photo. The photo can be in .jpeg or .svg formats.
    /// Only returned for Web Apps launched from the attachment menu.
    /// </summary>
    [JsonPropertyName("photo_url")]
    public string? PhotoURL { get; set; }
} 