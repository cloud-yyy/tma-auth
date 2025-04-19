using TmaAuth.Models;

namespace TmaAuth.Abstractions
{
    /// <summary>
    /// Parses Telegram Mini App init data
    /// </summary>
    public interface ITmaInitDataParser
    {
        /// <summary>
        /// Parse converts passed init data presented as query string to InitData object.
        /// </summary>
        /// <param name="initData">Init data string to parse</param>
        /// <returns>Parsed InitData object</returns>
        /// <exception cref="UnexpectedFormatException">Thrown when init data has unexpected format</exception>
        InitData Parse(string initData);
    }
}