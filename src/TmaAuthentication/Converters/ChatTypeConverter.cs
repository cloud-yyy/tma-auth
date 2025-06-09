using System.Text.Json;
using System.Text.Json.Serialization;
using TmaAuth.Models;

namespace TmaAuth.Converters;

/// <summary>
/// JSON converter for the ChatType enum
/// </summary>
public class ChatTypeConverter : JsonConverter<ChatType>
{
    public override ChatType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        return value?.ToLower() switch
        {
            "sender" => ChatType.Sender,
            "private" => ChatType.Private,
            "group" => ChatType.Group,
            "supergroup" => ChatType.Supergroup,
            "channel" => ChatType.Channel,
            _ => ChatType.Unknown
        };
    }

    public override void Write(Utf8JsonWriter writer, ChatType value, JsonSerializerOptions options)
    {
        string stringValue = value switch
        {
            ChatType.Sender => "sender",
            ChatType.Private => "private",
            ChatType.Group => "group",
            ChatType.Supergroup => "supergroup",
            ChatType.Channel => "channel",
            _ => "unknown"
        };
        writer.WriteStringValue(stringValue);
    }
} 