using System.Web;
using TmaAuth.Abstractions;
using TmaAuth.Models;

namespace TmaAuth;

/// <inheritdoc/>
public class TmaInitDataValidator : ITmaInitDataValidator
{
    private readonly ITmaInitDataSigner _signer;
    private readonly ITmaInitDataParser _parser;

    /// <summary>
    /// Creates a new instance of TmaInitDataValidator
    /// </summary>
    public TmaInitDataValidator()
    {
        _signer = new TmaInitDataSigner();
        _parser = new TmaInitDataParser();
    }

    /// <summary>
    /// Creates a new instance of TmaInitDataValidator
    /// </summary>
    /// <param name="signer">Signer to use for validating signatures</param>
    /// <param name="parser">Parser to use for parsing init data</param>
    public TmaInitDataValidator(ITmaInitDataSigner signer, ITmaInitDataParser parser)
    {
        _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    /// <inheritdoc/>
    public bool Validate(string initData, string token, TimeSpan expIn)
    {
        try
        {
            // Parse init data using our parser
            InitData parsedData;
            try
            {
                parsedData = _parser.Parse(initData);
            }
            catch
            {
                return false;
            }

            if (string.IsNullOrEmpty(parsedData.Hash))
            {
                return false;
            }

            // Process expiration time check
            if (expIn > TimeSpan.Zero)
            {
                try
                {
                    DateTime authDate = parsedData.AuthDate();
                    if (authDate.Add(expIn) < DateTime.UtcNow)
                    {
                        return false;
                    }
                }
                catch
                {
                    // Issues with auth_date
                    return false;
                }
            }

            // Parse query string again to get the raw values for signing
            var queryParams = HttpUtility.ParseQueryString(initData);
            if (queryParams == null)
            {
                return false;
            }

            var pairs = new List<string>();

            // Iterate over all key-value pairs of parsed parameters
            foreach (var key in queryParams.AllKeys)
            {
                if (key == null || key == "hash")
                {
                    continue;
                }

                var value = queryParams[key];
                if (value == null)
                {
                    continue;
                }
                
                pairs.Add($"{key}={value}");
            }

            pairs.Sort();

            if (_signer.SignPayload(string.Join("\n", pairs), token) != parsedData.Hash)
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}