using System;

namespace PsdEdit;

public sealed class PsdScriptBlockValue : PsdStringBase
{
    public PsdScriptBlockValue(string value, bool hasPrecedingEmptyLine = false) : base(value, hasPrecedingEmptyLine) { }

    internal PsdScriptBlockValue(PsdToken token) : base(token.Text)
    {
        if (token.IsNewLine)
        {
            SetValue(Environment.NewLine);
        }
    }
}