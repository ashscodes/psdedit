using System;

namespace PsdEdit;

public abstract class PsdStringBase : IPsdValue<string>
{
    private string _value;

    public PsdComment Comment { get; set; }

    public bool HasInlineComment => Comment is not null && !Comment.IsNull;

    public bool HasPrecedingEmptyLine { get; set; }

    public bool IsHereString { get; set; } = false;

    public bool IsNull => string.IsNullOrEmpty(_value);

    public bool IsCollection => false;

    public bool IsReadOnly { get; set; } = false;

    public PsdStringBase(string value, bool hasPrecedingEmptyLine = false)
    {
        HasPrecedingEmptyLine = hasPrecedingEmptyLine;
        SetValue(value);
    }

    protected PsdStringBase(PsdToken token) : this(token.Text) => IsHereString = token.IsHereString;

    public string GetValue() => _value;

    public override string ToString() => GetValue();

    public virtual void SetValue(string value)
    {
        if (!TrySetValue(value))
        {
            throw new ArgumentException(string.Format(Strings.CouldNotSetValue, GetType().Name));
        }
    }

    public virtual bool TrySetValue(object value)
    {
        _value = value switch
        {
            null => null,
            string strValue => strValue,
            _ => value.ToString(),
        };

        if (this is PsdStringExpandable || this is PsdStringLiteral)
        {
            if (_value.StartsWith('\''))
            {
                _value = _value.RemoveQuotationCharacter('\'');
            }

            if (_value.StartsWith('"'))
            {
                _value = _value.RemoveQuotationCharacter('"');
            }
        }

        return true;
    }
}