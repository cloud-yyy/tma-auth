using System.Text.Json.Serialization;
using TmaAuth.Converters;

namespace TmaAuth.Models;

/// <summary>
/// Chat describes chat information:
/// https://docs.telegram-mini-apps.com/launch-parameters/init-data#chat
/// </summary>
public class Chat
{
    /// <summary>
    /// Unique identifier for this chat.
    /// </summary>
    [JsonPropertyName("id")]
    public long ID { get; set; }

    /// <summary>
    /// Type of chat.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(ChatTypeConverter))]
    public ChatType Type { get; set; }

    /// <summary>
    /// Title of the chat.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional. URL of the chat's photo. The photo can be in .jpeg or .svg
    /// formats. Only returned for Web Apps launched from the attachment menu.
    /// </summary>
    [JsonPropertyName("photo_url")]
    public string? PhotoURL { get; set; }

    /// <summary>
    /// Optional. Username of the chat.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }
}
