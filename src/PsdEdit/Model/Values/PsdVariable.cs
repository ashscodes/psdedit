namespace PsdEdit;

public sealed class PsdVariable : PsdStringBase
{
    public bool IsSplatted { get; set; } = false;

    public string VariablePath { get; set; }

    public PsdVariable(string value, bool isSplatted = false) : base(value) => IsSplatted = isSplatted;

    public override void SetValue(string value)
    {
        IsSplatted = value.StartsWith('@');
        var startsWithDollar = value.StartsWith('$');

        if (IsSplatted || startsWithDollar)
        {
            value = value[1..];
        }

        base.SetValue(value);
    }

    public override string ToString()
    {
        if (IsSplatted)
        {
            return '@' + GetValue();
        }

        return '$' + GetValue();
    }
}