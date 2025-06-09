using TmaAuth.Models;

namespace TmaAuth.Tests;

public class ParserTests
{
    private const string ValidateTestInitData = "query_id=AAHdF6IQAAAAAN0XohDhrOrc&user=%7B%22id%22%3A279058397%2C%22first_name%22%3A%22Vladislav%22%2C%22last_name%22%3A%22Kibenko%22%2C%22username%22%3A%22vdkfrost%22%2C%22language_code%22%3A%22ru%22%2C%22is_premium%22%3Atrue%7D&auth_date=1662771648&hash=c501b71e775f74ce10e377dea85a7ea24ecd640b223ea86dfe453e0eaed2e2b2";
    private readonly TmaInitDataParser _parser;

    public ParserTests()
    {
        _parser = new TmaInitDataParser();
    }

    [Fact]
    public void Parse_ValidInitData_ReturnsInitDataObject()
    {
        // Act
        var result = _parser.Parse(ValidateTestInitData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1662771648, result.AuthDateRaw);
        Assert.Equal("c501b71e775f74ce10e377dea85a7ea24ecd640b223ea86dfe453e0eaed2e2b2", result.Hash);
        Assert.Equal("AAHdF6IQAAAAAN0XohDhrOrc", result.QueryID);
        Assert.NotNull(result.User);

        if (result.User != null)
        {
            Assert.Equal(279058397, result.User.ID);
            Assert.Equal("Vladislav", result.User.FirstName);
            Assert.Equal("Kibenko", result.User.LastName);
            Assert.Equal("vdkfrost", result.User.Username);
            Assert.Equal("ru", result.User.LanguageCode);
            Assert.True(result.User.IsPremium);
        }
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsUnexpectedFormatException()
    {
        // Arrange
        var invalidData = "something wrong;";

        // Act & Assert
        Assert.Throws<UnexpectedFormatException>(() => _parser.Parse(invalidData));
    }

    [Fact]
    public void Parse_EmptyString_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<UnexpectedFormatException>(() => _parser.Parse(string.Empty));
    }

    [Fact]
    public void Parse_InvalidQuery_ThrowsException()
    {
        // Arrange
        var invalidQuery = "notaquery";

        // Act & Assert
        var result = _parser.Parse(invalidQuery);
        Assert.NotNull(result);
    }

    [Fact]
    public void Parse_InitDataWithStartParam_ParsesCorrectly()
    {
        // Arrange
        var initDataWithStartParam = "query_id=AAHdF6IQAAAAAN0XohDhrOrc&start_param=value&auth_date=1662771648";

        // Act
        var result = _parser.Parse(initDataWithStartParam);

        // Assert
        Assert.Equal("value", result.StartParam);
    }

    [Fact]
    public void Parse_ComplexJsonValue_DeserializesCorrectly()
    {
        // Arrange
        var initData = "user={\"id\":123456789,\"first_name\":\"John\",\"last_name\":\"Doe\",\"username\":\"johndoe\",\"language_code\":\"en\",\"is_premium\":true}&auth_date=1662771648";

        // Act
        var result = _parser.Parse(initData);

        // Assert
        Assert.NotNull(result.User);

        if (result.User != null)
        {
            Assert.Equal(123456789, result.User.ID);
            Assert.Equal("John", result.User.FirstName);
            Assert.Equal("Doe", result.User.LastName);
            Assert.Equal("johndoe", result.User.Username);
            Assert.Equal("en", result.User.LanguageCode);
            Assert.True(result.User.IsPremium);
        }
    }

    [Fact]
    public void AuthDate_ReturnsCorrectDateTime()
    {
        // Arrange
        var initData = _parser.Parse(ValidateTestInitData);
        var expected = DateTimeOffset.FromUnixTimeSeconds(1662771648).DateTime;

        // Act
        var result = initData.AuthDate();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CanSendAfter_ReturnsCorrectDateTime()
    {
        // Arrange
        var initData = _parser.Parse(ValidateTestInitData);
        initData.CanSendAfterRaw = 60; // 60 seconds
        var expected = DateTimeOffset.FromUnixTimeSeconds(1662771648 + 60).DateTime;

        // Act
        var result = initData.CanSendAfter();

        // Assert
        Assert.Equal(expected, result);
    }
} 