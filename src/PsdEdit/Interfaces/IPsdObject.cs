namespace PsdEdit;

public interface IPsdObject
{
    PsdComment Comment { get; set; }

    bool HasInlineComment { get; }

    bool HasPrecedingEmptyLine { get; set; }

    bool IsCollection { get; }

    bool IsReadOnly { get; }
}