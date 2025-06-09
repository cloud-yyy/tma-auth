using System.Text.Json.Serialization;
using TmaAuth.Converters;

namespace TmaAuth.Models;

/// <summary>
/// InitData contains init data.
/// https://docs.telegram-mini-apps.com/launch-parameters/init-data#parameters-list
/// </summary>
public class InitData
{
    /// <summary>
    /// The date the initialization data was created. Is a number representing a
    /// Unix timestamp.
    /// </summary>
    [JsonPropertyName("auth_date")]
    public int AuthDateRaw { get; set; }

    /// <summary>
    /// Optional. The number of seconds after which a message can be sent via
    /// the method answerWebAppQuery.
    /// https://core.telegram.org/bots/api#answerwebappquery
    /// </summary>
    [JsonPropertyName("can_send_after")]
    public int CanSendAfterRaw { get; set; }

    /// <summary>
    /// Optional. An object containing information about the chat with the bot in
    /// which the Mini Apps was launched. It is returned only for Mini Apps
    /// opened through the attachment menu.
    /// </summary>
    [JsonPropertyName("chat")]
    public Chat? Chat { get; set; }

    /// <summary>
    /// Optional. The type of chat from which the Mini Apps was opened.
    /// Returned only for applications opened by direct link.
    /// </summary>
    [JsonPropertyName("chat_type")]
    [JsonConverter(typeof(ChatTypeConverter))]
    public ChatType ChatType { get; set; }

    /// <summary>
    /// Optional. A global identifier indicating the chat from which the Mini
    /// Apps was opened. Returned only for applications opened by direct link.
    /// </summary>
    [JsonPropertyName("chat_instance")]
    public long ChatInstance { get; set; }

    /// <summary>
    /// Initialization data signature.
    /// https://core.telegram.org/bots/webapps#validating-data-received-via-the-web-app
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;

    /// <summary>
    /// Optional. The unique session ID of the Mini App. Used in the process of
    /// sending a message via the method answerWebAppQuery.
    /// https://core.telegram.org/bots/api#answerwebappquery
    /// </summary>
    [JsonPropertyName("query_id")]
    public string? QueryID { get; set; }

    /// <summary>
    /// Optional. An object containing data about the chat partner of the current
    /// user in the chat where the bot was launched via the attachment menu.
    /// Returned only for private chats and only for Mini Apps launched via the
    /// attachment menu.
    /// </summary>
    [JsonPropertyName("receiver")]
    public User? Receiver { get; set; }

    /// <summary>
    /// Optional. The value of the startattach or startapp query parameter
    /// specified in the link. It is returned only for Mini Apps opened through
    /// the attachment menu.
    /// </summary>
    [JsonPropertyName("start_param")]
    public string? StartParam { get; set; }

    /// <summary>
    /// Optional. An object containing information about the current user.
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Returns AuthDateRaw as DateTime.
    /// </summary>
    /// <returns>DateTime representation of the AuthDateRaw</returns>
    public DateTime AuthDate()
    {
        return DateTimeOffset.FromUnixTimeSeconds(AuthDateRaw).DateTime;
    }

    /// <summary>
    /// Returns computed time which depends on CanSendAfterRaw and AuthDate.
    /// Originally, CanSendAfterRaw means time in seconds, after which
    /// answerWebAppQuery method can be called and that's why this value could
    /// be computed as time.
    /// </summary>
    /// <returns>DateTime when messages can be sent</returns>
    public DateTime CanSendAfter()
    {
        return AuthDate().AddSeconds(CanSendAfterRaw);
    }
} 