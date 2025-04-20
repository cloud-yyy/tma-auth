namespace TmaAuth.Abstractions
{
    /// <summary>
    /// Signs Telegram Mini App init data
    /// </summary>
    public interface ITmaInitDataSigner
    {
        /// <summary>
        /// Signs passed payload using specified key. Method removes such
        /// technical parameters as "hash" and "auth_date".
        /// </summary>
        /// <param name="payload">Key-value pairs to sign</param>
        /// <param name="token">Bot token used for signing</param>
        /// <param name="authDate">Authentication date</param>
        /// <returns>HMAC-SHA256 signature</returns>
        string Sign(IDictionary<string, string> payload, string token, DateTime authDate);

        /// <summary>
        /// Signs passed query string
        /// </summary>
        /// <param name="queryString">Query string to sign</param>
        /// <param name="token">Bot token used for signing</param>
        /// <param name="authDate">Authentication date</param>
        /// <returns>HMAC-SHA256 signature</returns>
        /// <exception cref="ArgumentException">Thrown when query string parsing fails</exception>
        string SignQueryString(string queryString, string token, DateTime authDate);

        /// <summary>
        /// Performs payload signing. Payload is a string of key-value pairs joined with "\n".
        /// </summary>
        /// <param name="payload">Data to sign</param>
        /// <param name="token">Bot token</param>
        /// <returns>HMAC-SHA256 signature as a hex string</returns>
        string SignPayload(string payload, string token);
    }
} 