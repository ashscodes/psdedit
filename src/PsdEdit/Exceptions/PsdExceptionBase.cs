using System;

namespace PsdEdit;

public abstract class PsdExceptionBase : Exception
{
    internal PsdExceptionBase() : base() { }

    internal PsdExceptionBase(string message, params object[] items) : base(FormatErrorMessage(message, items)) { }

    internal PsdExceptionBase(Exception innerException,
                              string message,
                              params object[] items) : base(FormatErrorMessage(message, items), innerException) { }

    internal static string FormatErrorMessage(string message, params object[] items)
    {
        if (items is null || items.Length == 0)
        {
            return message;
        }

        return string.Format(message, items);
    }
}