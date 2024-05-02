using System;
using System.Collections;
using System.IO;
using System.Text;

namespace PsdEdit;

public static class SystemExtensions
{
    public static StringBuilder AppendIndented(this StringBuilder builder, int indent, string text)
    {
        Ensure.IsNonNegative(indent);
        var prefix = new string(' ', indent);
        return builder.Append(prefix + text);
    }

    public static StringBuilder AppendLineIndented(this StringBuilder builder, int indent, string line)
    {
        Ensure.IsNonNegative(indent);
        var prefix = new string(' ', indent);
        return builder.AppendLine(prefix + line);
    }

    public static StringBuilder AppendLineIndented(this StringBuilder builder, int indent, string[] lines)
    {
        Ensure.IsNonNegative(indent);
        var prefix = new string(' ', indent);
        foreach (var line in lines)
        {
            builder.AppendLine(prefix + line);
        }

        return builder;
    }

    public static bool IsEnumerable(this object obj) => obj switch
    {
        null => false,
        string => false,
        IEnumerable => true,
        _ => false,
    };

    public static bool IsNullOrEmpty(this object obj) => obj switch
    {
        null => true,
        string str => string.IsNullOrEmpty(str),
        Array array => array.Length == 0,
        ICollection collection => collection.Count == 0,
        IEnumerable enumerable => enumerable.IsEnumerableEmpty(),
        _ => false
    };

    public static PsdReadResult ReadPsdFile(this FileInfo psdFile, IPsdTokenParser parser = null)
    {
        Ensure.FileHasExtension(psdFile, ".psd1");

        var tokenManager = PsdTokenManager.Create(psdFile.FullName, out string fileContent, parser);
        Ensure.StringNotNullOrEmpty(fileContent, nameof(fileContent));

        using var task = new PsdReadTask(tokenManager, psdFile.FullName, fileContent);
        task.Process();
        return task.Result;
    }

    public static PsdReadResult[] ReadPsdFiles(this DirectoryInfo directoryInfo,
                                               IPsdTokenParser parser = null,
                                               SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var files = directoryInfo.GetFiles(searchOption);
        var result = new PsdReadResult[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            result[i] = files[i].ReadPsdFile(parser);
        }

        return result;
    }

    public static string RemoveQuotationCharacter(this string value, char symbol)
    {
        if (value[0] == symbol)
        {
            value = value[1..];
        }

        if (value[^1] == symbol)
        {
            value = value[..^1];
        }

        return value;
    }

    public static string RemovePSVariableCharacter(this string value)
    {
        if (value[0] == '@')
        {
            value = value.TrimStart('@');
        }

        if (value[0] == '$')
        {
            value = value.TrimStart('$');
        }

        return value;
    }

    public static PsdArray ToPsdArray(this object[] values,
                                      bool isArrayLiteral = false) => ((IList)values).ToPsdArray(isArrayLiteral);

    public static PsdMap ToPsdMap(this Hashtable hashtable,
                                  string name = null) => ((IDictionary)hashtable).ToPsdMap(name);

    public static IPsdObject ToPsdObject(this object value) => value switch
    {
        bool boolean => new PsdBoolean(value),
        decimal or double or float or int or long => new PsdNumber(value),
        string strValue when strValue.IsPowerShellVariable() => new PsdVariable(strValue),
        IDictionary dictionary => dictionary.ToPsdMap(),
        IList list => list.ToPsdArray(),
        _ => new PsdStringLiteral((string)value)
    };

    private static FileInfo[] GetFiles(this DirectoryInfo directoryInfo, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var foundFiles = directoryInfo.GetFiles("*.psd1", searchOption);
        return foundFiles.Length > 0 ? foundFiles : throw new PsdReaderException(Strings.DataFileNotFound, 0);
    }

    private static bool IsPowerShellVariable(this string strValue)
        => !string.IsNullOrEmpty(strValue) && (strValue.StartsWith('$') || strValue.StartsWith('@'));
}