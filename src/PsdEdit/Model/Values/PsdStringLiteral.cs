namespace PsdEdit;

public sealed class PsdStringLiteral : PsdStringBase
{
    public PsdStringLiteral(PsdToken token) : base(token) { }

    public PsdStringLiteral(string value, bool hasPrecedingEmptyLine = false) : base(value, hasPrecedingEmptyLine) { }

    public override string ToString() => (IsHereString ? "@" : "") + "'" + GetValue() + "'";
}