namespace PsdEdit;

public sealed class TokenValidationException : PsdExceptionBase
{
    public TokenValidationException() : base() { }

    public TokenValidationException(string message, params object[] items) : base(FormatErrorMessage(message, items)) { }
}