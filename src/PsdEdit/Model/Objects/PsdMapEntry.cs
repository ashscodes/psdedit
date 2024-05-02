using System;

namespace PsdEdit;

public class PsdMapEntry : IPsdObject
{
    private IPsdObject _value;

    public PsdComment Comment { get; set; }

    public bool HasInlineComment => Comment is not null && !Comment.IsNull;

    public bool HasPrecedingEmptyLine { get; set; }

    public bool IsCollection => typeof(IPsdCollection<>).IsAssignableFrom(_value.GetType());

    public bool IsReadOnly => false;

    public string Key { get; set; }

    public PsdMapEntry() { }

    public PsdMapEntry(string key, IPsdObject psdObject, bool hasPrecedingEmptyLine = false)
    {
        HasPrecedingEmptyLine = hasPrecedingEmptyLine;
        Key = key;
        if (!TrySetValue(psdObject))
        {
            throw new ArgumentOutOfRangeException(nameof(psdObject));
        }
    }

    public IPsdObject GetValue() => _value;

    public bool TrySetValue(IPsdObject psdObject)
    {
        if (CanSetValue(psdObject))
        {
            _value = psdObject;
            return true;
        }

        return false;
    }

    public override string ToString() => Key;

    private static bool CanSetValue(IPsdObject psdObject) => psdObject is not PsdMapEntry;
}