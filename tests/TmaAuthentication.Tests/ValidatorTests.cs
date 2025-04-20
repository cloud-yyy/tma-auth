namespace TmaAuth.Tests;

public class ValidatorTests
{
    private const string ValidateTestInitData = "query_id=AAHdF6IQAAAAAN0XohDhrOrc&user=%7B%22id%22%3A279058397%2C%22first_name%22%3A%22Vladislav%22%2C%22last_name%22%3A%22Kibenko%22%2C%22username%22%3A%22vdkfrost%22%2C%22language_code%22%3A%22ru%22%2C%22is_premium%22%3Atrue%7D&auth_date=1662771648&hash=c501b71e775f74ce10e377dea85a7ea24ecd640b223ea86dfe453e0eaed2e2b2";
    private const string ValidateTestToken = "5768337691:AAH5YkoiEuPk8-FZa32hStHTqXiLPtAEhx8";
    private readonly TmaInitDataValidator _validator;

    public ValidatorTests()
    {
        _validator = new TmaInitDataValidator(new TmaInitDataSigner(), new TmaInitDataParser());
    }

    [Fact]
    public void Validate_ValidInitData_ReturnsTrue()
    {
        // Act
        var result = _validator.Validate(ValidateTestInitData, ValidateTestToken, TimeSpan.Zero);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Validate_WithExpiration_ReturnsFalse()
    {
        // Act
        var result = _validator.Validate(ValidateTestInitData, ValidateTestToken, TimeSpan.FromSeconds(1));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Validate_InvalidFormat_ReturnsFalse()
    {
        // Arrange
        var invalidData = "here comes something wrong;";

        // Act
        var result = _validator.Validate(invalidData, ValidateTestToken, TimeSpan.Zero);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Validate_NoHash_ReturnsFalse()
    {
        // Arrange
        var noHashData = "no_hash=true";

        // Act
        var result = _validator.Validate(noHashData, ValidateTestToken, TimeSpan.Zero);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Validate_NoAuthDateWithExpiration_ReturnsFalse()
    {
        // Arrange
        var noAuthDateData = "hash=abc";

        // Act
        var result = _validator.Validate(noAuthDateData, ValidateTestToken, TimeSpan.FromSeconds(1));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Validate_ExpiredAuthDate_ReturnsFalse()
    {
        // Arrange
        var expiredData = "hash=abc&auth_date=1512371876";

        // Act
        var result = _validator.Validate(expiredData, ValidateTestToken, TimeSpan.FromSeconds(1));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Validate_InvalidHash_ReturnsFalse()
    {
        // Arrange
        var tamperedData = ValidateTestInitData + "abc";

        // Act
        var result = _validator.Validate(tamperedData, ValidateTestToken, TimeSpan.Zero);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Validate_InvalidAuthDate_ReturnsFalse()
    {
        // Arrange
        var invalidAuthDateData = "hash=abc&auth_date=test";

        // Act
        var result = _validator.Validate(invalidAuthDateData, ValidateTestToken, TimeSpan.FromSeconds(1));

        // Assert
        Assert.False(result);
    }
}