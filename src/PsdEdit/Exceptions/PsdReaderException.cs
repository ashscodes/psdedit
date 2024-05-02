using System;

namespace PsdEdit;

public sealed class PsdReaderException : PsdExceptionBase
{
    public PsdReaderException() : base() { }

    public PsdReaderException(string message, params object[] items) : base(message, items) { }

    public PsdReaderException(Exception innerException,
                              string message,
                              params object[] items) : base(innerException, message, items) { }

    public PsdReaderException(string item, int index) : base(Strings.DataFileReadError, item, index) { }

    public PsdReaderException(Exception innerException, string item, int index) : base(innerException, Strings.DataFileReadError, item, index) { }
}