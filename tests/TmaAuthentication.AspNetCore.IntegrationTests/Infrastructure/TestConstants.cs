namespace TmaAuthentication.AspNetCore.IntegrationTests.Infrastructure;

public static class TestConstants
{
    public const string ValidInitData = "query_id=AAHdF6IQAAAAAN0XohDhrOrc&user=%7B%22id%22%3A279058397%2C%22first_name%22%3A%22Vladislav%22%2C%22last_name%22%3A%22Kibenko%22%2C%22username%22%3A%22vdkfrost%22%2C%22language_code%22%3A%22ru%22%2C%22is_premium%22%3Atrue%7D&auth_date=1662771648&hash=c501b71e775f74ce10e377dea85a7ea24ecd640b223ea86dfe453e0eaed2e2b2";
    public const string ValidBotToken = "5768337691:AAH5YkoiEuPk8-FZa32hStHTqXiLPtAEhx8";
    public const string JwtSecretKey = "test-secret-key-for-jwt-integration-tests-minimum-32-characters";

    // Invalid test data
    public const string InvalidInitData = "here comes something wrong;";
    public const string InvalidBotToken = "invalid:bot-token";
    public const string TamperedInitData = ValidInitData + "abc";
    public const string NoHashInitData = "no_hash=true";
    public const string ExpiredInitData = "hash=abc&auth_date=1512371876";
    
    // User data from valid init data
    public const long ExpectedUserId = 279058397;
    public const string ExpectedFirstName = "Vladislav";
    public const string ExpectedLastName = "Kibenko";
    public const string ExpectedUsername = "vdkfrost";
    public const string ExpectedLanguageCode = "ru";
    public const bool ExpectedIsPremium = true;
}