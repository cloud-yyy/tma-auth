namespace TmaAuth.Models;

/// <summary>
/// TmaAuthException is the base exception for all TMA authentication errors
/// </summary>
public class TmaAuthException : Exception
{
    /// <summary>
    /// Creates a new TmaAuthException with the specified message
    /// </summary>
    /// <param name="message">Error message</param>
    public TmaAuthException(string message) : base(message)
    {
    }
}

/// <summary>
/// Exception thrown when auth_date is missing
/// </summary>
public class AuthDateMissingException : TmaAuthException
{
    /// <summary>
    /// Creates a new AuthDateMissingException
    /// </summary>
    public AuthDateMissingException() : base("Auth date is missing")
    {
    }
}

/// <summary>
/// Exception thrown when auth_date is invalid
/// </summary>
public class AuthDateInvalidException : TmaAuthException
{
    /// <summary>
    /// Creates a new AuthDateInvalidException
    /// </summary>
    public AuthDateInvalidException() : base("Auth date is invalid")
    {
    }
}

/// <summary>
/// Exception thrown when signature is missing
/// </summary>
public class SignMissingException : TmaAuthException
{
    /// <summary>
    /// Creates a new SignMissingException
    /// </summary>
    public SignMissingException() : base("Signature is missing")
    {
    }
}

/// <summary>
/// Exception thrown when signature is invalid
/// </summary>
public class SignInvalidException : TmaAuthException
{
    /// <summary>
    /// Creates a new SignInvalidException
    /// </summary>
    public SignInvalidException() : base("Signature is invalid")
    {
    }
}

/// <summary>
/// Exception thrown when init data has unexpected format
/// </summary>
public class UnexpectedFormatException : TmaAuthException
{
    /// <summary>
    /// Creates a new UnexpectedFormatException
    /// </summary>
    public UnexpectedFormatException() : base("Init data has unexpected format")
    {
    }

    /// <summary>
    /// Creates a new UnexpectedFormatException with inner exception
    /// </summary>
    /// <param name="innerException">Inner exception</param>
    public UnexpectedFormatException(Exception innerException) 
        : base($"Init data has unexpected format: {innerException.Message}")
    {
    }
}

/// <summary>
/// Exception thrown when init data is expired
/// </summary>
public class ExpiredException : TmaAuthException
{
    /// <summary>
    /// Creates a new ExpiredException
    /// </summary>
    public ExpiredException() : base("Init data is expired")
    {
    }
}