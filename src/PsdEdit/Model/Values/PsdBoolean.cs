namespace PsdEdit;

public sealed class PsdBoolean : IPsdValue<bool?>
{
    private bool? _value;

    public PsdComment Comment { get; set; }

    public bool HasInlineComment => Comment is not null && !Comment.IsNull;

    public bool HasPrecedingEmptyLine { get; set; }

    public bool IsNull => !_value.HasValue;

    public bool IsCollection => false;

    public bool IsReadOnly { get; set; }

    public PsdBoolean() { }

    internal PsdBoolean(object value) => TrySetValue(value);

    public bool? GetValue() => _value;

    public void SetValue(bool? value) => _value = value;

    public bool TrySetValue(object value)
    {
        switch (value)
        {
            case null:
                _value = null;
                return true;
            case bool booleanValue:
                _value = booleanValue;
                return true;
            case string strValue:
                if (bool.TryParse(strValue, out bool boolResult))
                {
                    _value = boolResult;
                    return true;
                }

                return false;
            default:
                return false;
        }
    }

    public override string ToString() => GetValue()?.ToString();
}