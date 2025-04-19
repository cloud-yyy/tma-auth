using System.Text.Json;
using System.Web;
using TmaAuth.Abstractions;
using TmaAuth.Models;

namespace TmaAuth;

/// <inheritdoc/>
public class TmaInitDataParser : ITmaInitDataParser
{
    /// <summary>
    /// List of properties which should always be interpreted as strings
    /// </summary>
    private static readonly HashSet<string> StringProps = ["start_param"];

    /// <inheritdoc/>
    public InitData Parse(string initData)
    {
        try
        {
            // Check for empty string first
            if (string.IsNullOrEmpty(initData))
            {
                throw new UnexpectedFormatException();
            }
            
            // Check for invalid format
            if (initData.Contains(';'))
            {
                throw new UnexpectedFormatException();
            }

            // Parse passed init data as query string
            var queryParams = HttpUtility.ParseQueryString(initData)
                ?? throw new UnexpectedFormatException();

            // Build JSON object from query parameters
            var jsonElements = new List<string>();
            foreach (var key in queryParams.AllKeys)
            {
                if (key == null)
                {
                    continue;
                }

                // Get the value
                var value = queryParams[key];
                if (value == null)
                {
                    continue;
                }

                // Format the value properly
                if (StringProps.Contains(key) || !IsValidJson(value))
                {
                    // Treat as a quoted string if it's in StringProps or not valid JSON
                    jsonElements.Add($"\"{key}\":\"{JsonEncode(value)}\"");
                }
                else
                {
                    // Use value as-is if it's valid JSON
                    jsonElements.Add($"\"{key}\":{value}");
                }
            }

            // Create JSON string and deserialize
            var jsonStr = "{" + string.Join(",", jsonElements) + "}";
            var initDataObj = JsonSerializer.Deserialize<InitData>(jsonStr);
            
            if (initDataObj == null)
            {
                throw new UnexpectedFormatException();
            }
            
            return initDataObj;
        }
        catch (Exception ex) when (ex is not TmaAuthException)
        {
            throw new UnexpectedFormatException(ex);
        }
    }

    /// <summary>
    /// Check if a string is a valid JSON value
    /// </summary>
    private static bool IsValidJson(string value)
    {
        try
        {
            using (JsonDocument.Parse(value))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Encode a string for JSON
    /// </summary>
    private static string JsonEncode(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}