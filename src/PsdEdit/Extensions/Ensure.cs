using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PsdEdit;

[DebuggerStepThrough]
internal static class Ensure
{
    public static void ArrayNotEmpty(object[] value, string name)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name, string.Format(Strings.ObjectIsNull, name));
        }

        if (value.Length < 1)
        {
            throw new ArgumentNullException(name, string.Format(Strings.ArrayIsEmpty, name));
        }
    }

    public static void CollectionNotEmpty(ICollection value, string name)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name, string.Format(Strings.ObjectIsNull, name));
        }

        if (value.Count < 1)
        {
            throw new ArgumentNullException(name, string.Format(Strings.CollectionIsEmpty, name));
        }
    }

    public static void CountIsEqual(int firstObjectCount, int secondObjectCount)
    {
        if (firstObjectCount != secondObjectCount)
        {
            throw new ArgumentException(string.Format(Strings.RangesNotEqual, firstObjectCount, secondObjectCount));
        }
    }

    public static void CountIsGreaterThanOrEqualTo(int firstObjectCount, string firstObjectName, int secondObjectCount)
    {
        if (firstObjectCount < secondObjectCount)
        {
            throw new ArgumentException(string.Format(Strings.InsufficientValuesInArray, firstObjectName, firstObjectCount, secondObjectCount));
        }
    }

    public static void FileExists(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(null, filePath);
        }
    }

    public static void FileHasExtension(FileInfo fileInfo, string extension)
    {
        FileExists(fileInfo.FullName);

        if (!fileInfo.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
        {
            throw new PsdReaderException(Strings.InvalidFileType, fileInfo, extension);
        }
    }

    public static void IndexIsInRange(int index, int count)
    {
        if (index < 0 || index >= count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), string.Format(Strings.ArgumentOutsideOfIndex, index, count));
        }
    }

    public static void IsNonNegative(int number)
    {
        if (number < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(number), string.Format(Strings.NegativeNumberInvalid, number));
        }
    }

    public static void ParseTaskSucceeded(IEnumerable<string> errors, string messageContent, bool isScriptBlock = false)
    {
        if (errors.Any())
        {
            throw isScriptBlock
                ? new PsdReaderException(Strings.ScriptBlockInvalid,
                                         string.Join(Environment.NewLine, errors),
                                         messageContent)
                : new PsdReaderException(Strings.DataFileInvalid, messageContent);
        }
    }

    public static void StringNotNullOrEmpty(string value, string propertyName)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(string.Format(Strings.StringIsNullOrEmpty, propertyName));
        }
    }

    public static void ValueNotNull(object value, string name)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name, string.Format(Strings.ObjectIsNull, name));
        }
    }
}