using System;
using System.Linq;

namespace PsdEdit;

public sealed class PsdComment : PsdStringBase
{
    public bool IsEmpty => GetValue() == "#";

    public bool IsInMap { get; set; }

    public bool IsMultiLine => GetValue().StartsWith("<#");

    internal bool IsWithinArray { get; set; }

    public PsdComment(string value, bool hasPrecedingEmptyLine = false) : base(value, hasPrecedingEmptyLine) { }

    public string ToUncommentedString()
    {
        string[] lines = ToString()?.Split(Environment.NewLine);

        if (lines is null || lines.Length == 0)
        {
            throw new ArgumentNullException(nameof(PsdComment));
        }

        for (int i = 0; i < lines.Length; i++)
        {
            var tempLine = lines[i].TrimStart() switch
            {
                var x when x.StartsWith("<# ") => x[3..],
                var x when x.EndsWith(" #>") => x[0..^3],
                var x when x.StartsWith("<#")
                           || x.StartsWith("# ")
                           || x.StartsWith("#>") => x[2..],
                var x when x.StartsWith('#') => x[1..],
                var x => x
            };

            lines[i] = tempLine.Trim();
        }

        return string.Join(Environment.NewLine, lines.Where(l => !string.IsNullOrEmpty(l)));
    }

    public bool TryGetMapEntry(out PsdMapEntry psdMapEntry, out IPsdObject otherObject)
        => this.TryConvertToMapEntry(out psdMapEntry, out otherObject);
}