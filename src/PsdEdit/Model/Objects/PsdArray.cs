namespace PsdEdit;

public sealed class PsdArray : PsdArrayBase
{
    public bool HasAtSymbol { get; set; }

    public bool IsArrayLiteral { get; set; }

    public PsdArray(bool isArrayLiteral = false) => IsArrayLiteral = isArrayLiteral;
}