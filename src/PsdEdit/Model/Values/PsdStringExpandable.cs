namespace PsdEdit;

public sealed class PsdStringExpandable : PsdStringBase
{
    public PsdStringExpandable(PsdToken token) : base(token) { }

    public PsdStringExpandable(string value, bool hasPrecedingEmptyLine = false) : base(value, hasPrecedingEmptyLine) { }

    public override string ToString() => (IsHereString ? "@" : "") + "\"" + GetValue() + "\"";
}