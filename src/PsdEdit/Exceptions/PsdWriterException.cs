using System;

namespace PsdEdit;

public sealed class PsdWriterException : PsdExceptionBase
{
    public PsdWriterException() : base() { }

    public PsdWriterException(string message, params object[] items) : base(FormatErrorMessage(message, items)) { }

    public PsdWriterException(Exception innerException,
                              string message,
                              params object[] items) : base(innerException, message, items) { }
}