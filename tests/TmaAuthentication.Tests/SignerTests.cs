namespace TmaAuth.Tests;

public class SignerTests
{
    private const string SignTestInitData = "query_id=AAHdF6IQAAAAAN0XohDhrOrc&user=%7B%22id%22%3A279058397%2C%22first_name%22%3A%22Vladislav%22%2C%22last_name%22%3A%22Kibenko%22%2C%22username%22%3A%22vdkfrost%22%2C%22language_code%22%3A%22ru%22%2C%22is_premium%22%3Atrue%7D&auth_date=1662771648";
    private const string SignTestToken = "5768337691:AAH5YkoiEuPk8-FZa32hStHTqXiLPtAEhx8";
    private const string SignTestHash = "226d6676794dfcb867f9495f96d1641c5d2b72587c94f28bcaab04bf6c409c56";
    private static readonly DateTime SignTestAuthDate = DateTimeOffset.FromUnixTimeSeconds(1662771648).DateTime;
    private readonly TmaInitDataSigner _signer;

    public SignerTests()
    {
        _signer = new TmaInitDataSigner();
    }

    [Fact]
    public void SignQueryString_StandardCase_ReturnsCorrectHash()
    {
        // Arrange
        var initData = "query_id=AAHdF6IQAAAAAN0XohDhrOrc&user=%7B%22id%22%3A279058397%2C%22first_name%22%3A%22Vladislav%22%2C%22last_name%22%3A%22Kibenko%22%2C%22username%22%3A%22vdkfrost%22%2C%22language_code%22%3A%22ru%22%2C%22is_premium%22%3Atrue%7D";

        // Act
        var hash = _signer.SignQueryString(initData, SignTestToken, SignTestAuthDate);

        // Assert
        Assert.Equal(SignTestHash, hash);
    }

    [Fact]
    public void SignQueryString_WithAuthDateAndHash_IgnoresThem()
    {
        // Arrange
        var initData = SignTestInitData + "&hash=" + SignTestHash;

        // Act
        var hash = _signer.SignQueryString(initData, SignTestToken, SignTestAuthDate);

        // Assert
        Assert.Equal(SignTestHash, hash);
    }

    [Fact]
    public void Sign_WithDictionary_ReturnsCorrectHash()
    {
        // Arrange
        var dict = new Dictionary<string, string>
        {
            ["query_id"] = "AAHdF6IQAAAAAN0XohDhrOrc",
            ["user"] = "{\"id\":279058397,\"first_name\":\"Vladislav\",\"last_name\":\"Kibenko\",\"username\":\"vdkfrost\",\"language_code\":\"ru\",\"is_premium\":true}"
        };

        // Act
        string hash = _signer.Sign(dict, SignTestToken, SignTestAuthDate);

        // Assert
        Assert.Equal(SignTestHash, hash);
    }

    [Fact]
    public void Sign_IgnoresAuthDateAndHash()
    {
        // Arrange
        var dict = new Dictionary<string, string>
        {
            ["query_id"] = "AAHdF6IQAAAAAN0XohDhrOrc",
            ["user"] = "{\"id\":279058397,\"first_name\":\"Vladislav\",\"last_name\":\"Kibenko\",\"username\":\"vdkfrost\",\"language_code\":\"ru\",\"is_premium\":true}",
            ["auth_date"] = "1234567890", // Different from SignTestAuthDate
            ["hash"] = "somewronghash"
        };

        // Act
        var hash = _signer.Sign(dict, SignTestToken, SignTestAuthDate);

        // Assert
        Assert.Equal(SignTestHash, hash);
    }

    [Fact]
    public void SignPayload_ReturnsCorrectHash()
    {
        // Arrange
        var expectedHash = "c501b71e775f74ce10e377dea85a7ea24ecd640b223ea86dfe453e0eaed2e2b2";
        var payload = "auth_date=1662771648\nquery_id=AAHdF6IQAAAAAN0XohDhrOrc\nuser={\"id\":279058397,\"first_name\":\"Vladislav\",\"last_name\":\"Kibenko\",\"username\":\"vdkfrost\",\"language_code\":\"ru\",\"is_premium\":true}";

        // Act
        var hash = _signer.SignPayload(payload, SignTestToken);

        // Assert
        Assert.Equal(expectedHash, hash);
    }
} 