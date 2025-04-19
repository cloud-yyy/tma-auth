namespace TmaAuth.Abstractions
{
    /// <summary>
    /// Validates Telegram Mini App init data
    /// </summary>
    public interface ITmaInitDataValidator
    {
        /// <summary>
        /// Validates passed init data. This method expects initData to be
        /// passed in the exact raw format as it could be found in window.Telegram.WebApp.initData.
        /// </summary>
        /// <param name="initData">Init data passed from application</param>
        /// <param name="token">Bot token used to validate the signature</param>
        /// <param name="expIn">Maximum init data lifetime. In case expIn is TimeSpan.Zero,
        /// the method does not check if data is expired.</param>
        /// <returns>True if validation succeeds, false otherwise</returns>
        bool Validate(string initData, string token, TimeSpan expIn);
    }
}