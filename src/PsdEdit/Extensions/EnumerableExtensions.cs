using System;
using System.Collections;

namespace PsdEdit;

internal static class EnumerableExtensions
{
    public static T[] CopyInReverse<T>(this T[] sourceArray, int startIndex, int endIndex)
    {
        Ensure.ValueNotNull(sourceArray, nameof(sourceArray));
        Ensure.IndexIsInRange(startIndex, sourceArray.Length);
        Ensure.IndexIsInRange(endIndex, sourceArray.Length);

        int length = endIndex - startIndex;
        var reversedArray = new T[length];
        for (int i = startIndex, j = length - 1; i < endIndex; i++, j--)
        {
            reversedArray[j] = sourceArray[i];
        }

        return reversedArray;
    }

    public static bool IsEnumerableEmpty(this IEnumerable enumerable)
    {
        foreach (var item in enumerable)
        {
            return false;
        }

        return true;
    }

    public static bool Move(this IList list, int objectIndex, int newIndex)
    {
        Ensure.IndexIsInRange(objectIndex, list.Count);
        Ensure.IndexIsInRange(newIndex, list.Count);

        var item = list[objectIndex];
        list.RemoveAt(objectIndex);
        if (newIndex > objectIndex)
        {
            newIndex--;
        }

        try
        {
            list.Insert(newIndex, item);
            return true;
        }
        catch (NotSupportedException)
        {
            throw;
        }
        catch
        {
            list.Insert(objectIndex, item);
            return false;
        }
    }

    public static PsdArray ToPsdArray(this IList list, bool isArrayLiteral = false)
    {
        var array = new PsdArray(isArrayLiteral);

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] is not null)
            {
                var psdObject = list[i].ToPsdObject();
                array.Add(psdObject);
            }
        }

        return array;
    }

    public static PsdMap ToPsdMap(this IDictionary dictionary, string name = null)
    {
        var map = new PsdMap(name);

        foreach (DictionaryEntry entry in dictionary)
        {
            IPsdObject value;
            if (entry.Value is null)
            {
                value = new PsdComment(string.Format(Strings.CommentedMapEntry, entry.Key));
            }
            else
            {
                var psdObject = entry.Value.ToPsdObject();
                value = new PsdMapEntry((string)entry.Key, psdObject);
            }

            map.Add(value);
        }

        return map;
    }
}