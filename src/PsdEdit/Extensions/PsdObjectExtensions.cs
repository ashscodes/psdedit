using System;
using System.IO;

namespace PsdEdit;

internal static class PsdObjectExtensions
{
    public static object GetValueInternal(this IPsdValue psdValue) => psdValue switch
    {
        PsdBoolean booleanValue => booleanValue.GetValue(),
        PsdNumber numberValue => numberValue.GetValue(),
        PsdStringBase stringValue => stringValue.GetValue(),
        _ => null
    };

    public static bool TryConvertToMapEntry(this PsdComment comment,
                                            out PsdMapEntry mapEntry,
                                            out IPsdObject psdObject)
    {
        psdObject = mapEntry = null;

        try
        {
            var commentStr = comment?.ToUncommentedString();
            if (string.IsNullOrEmpty(commentStr) || !commentStr.Contains('='))
            {
                return false;
            }

            var result = ParseCommentString(commentStr);
            if (result is null || !result.Success || result.Count == 0)
            {
                return false;
            }

            psdObject = result.Items[0];
            mapEntry = GetMapEntry(psdObject);
            return mapEntry is not null;
        }
        catch (PsdReaderException)
        {
            return false;
        }
    }

    public static string Write(this IPsdObject psdObject) => PsdWriter.Create(psdObject).ToString();

    public static void Write(this IPsdObject psdObject, FileInfo target) => PsdWriter.Create(psdObject, target).Save();

    private static PsdMapEntry GetMapEntry(IPsdObject psdObject) => psdObject switch
    {
        PsdMapEntry me => me,
        PsdCollectionBase c when c.Count > 0 && c[0] is PsdMapEntry me => me,
        _ => null
    };

    private static PsdReadResult ParseCommentString(string commentStr)
    {
        try
        {
            var mapStr = "@{" + Environment.NewLine + commentStr + Environment.NewLine + "}";
            var tokenCollection = PsdTokenParser.Parse(mapStr);
            using var task = new PsdReadTask(tokenCollection, mapStr);
            task.Process();
            return task.Result;
        }
        catch (PsdReaderException)
        {
            return null;
        }
    }

    internal static bool IsNamedMap(this IPsdObject psdObject, out PsdMap map)
    {
        if (psdObject is PsdMapEntry mapEntry)
        {
            return mapEntry.GetValue().IsNamedMap(out map);
        }

        if (psdObject is PsdMap psdMap)
        {
            map = psdMap;
            return psdMap.IsNamed;
        }

        map = null;
        return false;
    }
}