using System.Security.Cryptography;
using System.Text;
using System.Web;
using TmaAuth.Abstractions;

namespace TmaAuth;

/// <inheritdoc/>
public class TmaInitDataSigner : ITmaInitDataSigner
{
    /// <inheritdoc/>
    public string Sign(IDictionary<string, string> payload, string token, DateTime authDate)
    {
        var pairs = new List<string>();

        // Extract all key-value pairs and add them to pairs list
        foreach (var kvp in payload)
        {
            // Skip technical fields
            if (kvp.Key == "hash" || kvp.Key == "auth_date")
            {
                continue;
            }
            
            // For the user field, make sure that it is decoded properly
            var value = kvp.Value;
            if (kvp.Key == "user" && value.StartsWith("%7B"))
            {
                // URL-decode the user field
                value = HttpUtility.UrlDecode(value);
            }
            
            // Append new pair
            pairs.Add($"{kvp.Key}={value}");
        }

        // Append sign date (convert to Unix timestamp)
        var authDateUnix = new DateTimeOffset(authDate).ToUnixTimeSeconds();
        pairs.Add($"auth_date={authDateUnix}");

        // According to docs, we sort all the pairs in alphabetical order
        pairs.Sort();

        // Perform signing
        return SignPayload(string.Join("\n", pairs), token);
    }

    /// <inheritdoc/>
    public string SignQueryString(string queryString, string token, DateTime authDate)
    {
        // Parse query string
        var queryParams = HttpUtility.ParseQueryString(queryString)
            ?? throw new ArgumentException("Failed to parse query string", nameof(queryString));

        // Convert to dictionary
        var dict = new Dictionary<string, string>();
        foreach (string? key in queryParams.AllKeys)
        {
            if (key == null) continue;
            
            var value = queryParams[key];
            if (value != null)
            {
                dict[key] = value;
            }
        }

        return Sign(dict, token, authDate);
    }

    /// <inheritdoc/>
    public string SignPayload(string payload, string token)
    {
        var webAppData = Encoding.UTF8.GetBytes("WebAppData");
        
        // Create the secret key using HMAC-SHA256
        using var hmac1 = new HMACSHA256(webAppData);
        var secretKey = hmac1.ComputeHash(Encoding.UTF8.GetBytes(token));

        // Sign the payload using the secret key
        using var hmac2 = new HMACSHA256(secretKey);
        var hash = hmac2.ComputeHash(Encoding.UTF8.GetBytes(payload));

        // Convert the hash to a hex string
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}