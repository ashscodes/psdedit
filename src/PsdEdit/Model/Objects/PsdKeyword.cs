namespace PsdEdit;

public sealed class PsdKeyword : IPsdObject
{
    public PsdComment Comment { get; set; }

    public PsdKeywordConditionCollection Conditions { get; set; }

    public bool HasInlineComment => Comment is not null && !Comment.IsNull;

    public bool HasPrecedingEmptyLine { get; set; }

    public bool IsCollection => false;

    public bool IsReadOnly { get; set; } = false;

    public string Keyword { get; set; }

    public PsdScriptBlock ScriptBlock { get; set; }

    public PsdKeyword(string keyword) => Keyword = keyword;

    public override string ToString() => Keyword;
}