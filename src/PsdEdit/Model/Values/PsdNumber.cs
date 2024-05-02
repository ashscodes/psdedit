namespace PsdEdit;

public sealed class PsdNumber : IPsdValue<decimal?>
{
    private decimal? _value;

    public PsdComment Comment { get; set; }

    public bool HasInlineComment => Comment is not null && !Comment.IsNull;

    public bool HasPrecedingEmptyLine { get; set; }

    public bool IsCollection => false;

    public bool IsNull => !_value.HasValue;

    public bool IsReadOnly { get; set; }

    public PsdNumber() { }

    internal PsdNumber(object value) => TrySetValue(value);

    public decimal? GetValue() => _value;

    public void SetValue(decimal? value) => _value = value;

    public void SetValue(double? value) => _value = (decimal?)value;

    public void SetValue(float? value) => _value = (decimal?)value;

    public void SetValue(int? value) => _value = value;

    public void SetValue(long? value) => _value = value;

    public bool TrySetValue(object value)
    {
        switch (value)
        {
            case null:
                _value = null;
                return true;
            case decimal:
            case double:
            case float:
            case int:
            case long:
                _value = (decimal?)value;
                return true;
            case string strValue:
                if (decimal.TryParse(strValue, out decimal decimalValue))
                {
                    _value = decimalValue;
                    return true;
                }

                return false;
            default:
                return false;
        }
    }

    public override string ToString() => GetValue()?.ToString();
}