using System;
using System.Management.Automation.Language;
using System.Threading;

namespace PsdEdit;

public enum PsdTokenLookupDirection
{
    LookBehind = 0,
    LookForward = 1,
}

public class PsdTokenLookup
{
    private PsdTokenCollection _tokens;

    private static readonly Lazy<PsdTokenLookup> _instance =
        new Lazy<PsdTokenLookup>(() => new PsdTokenLookup(), LazyThreadSafetyMode.ExecutionAndPublication);

    public IPsdTokenValidationSet ArrayLiteral { get; }

    public IPsdTokenValidatorItem BaseValues { get; }

    public IPsdTokenValidatorItem BasicStrings { get; }

    public IPsdTokenValidatorItem Identifier { get; }

    public IPsdTokenValidationSet InlineComment { get; }

    public IPsdTokenValidationSet InlineCommentWithComma { get; }

    public IPsdTokenValidatorItem LineTerminator { get; }

    public IPsdTokenValidationSet MapEntry { get; }

    public IPsdTokenValidatorItem StringExpandable { get; }

    public IPsdTokenValidatorItem StringLiteral { get; }

    public IPsdTokenValidatorItem Variable { get; }

    public static PsdTokenLookup Instance => _instance.Value;

    internal PsdTokenCollection Tokens
    {
        get => _tokens;
        private set => _tokens = value;
    }

    private PsdTokenLookup()
    {
        // Single check TokenValidatorItems
        BasicStrings = new PsdTokenKindValidator(TokenKind.StringExpandable, TokenKind.StringLiteral);
        LineTerminator = new PsdTokenKindValidator(TokenKind.NewLine, TokenKind.Semi);
        StringExpandable = new PsdTokenKindValidator(TokenKind.HereStringExpandable, TokenKind.StringExpandable);
        StringLiteral = new PsdTokenKindValidator(TokenKind.HereStringLiteral, TokenKind.StringLiteral);
        Variable = new PsdTokenKindValidator(TokenKind.SplattedVariable, TokenKind.Variable);

        // Merged TokenValidatorItems.
        Identifier = ((PsdTokenKindValidator)BasicStrings).Clone().Union(TokenKind.Identifier);
        BaseValues = new PsdTokenKindValidator(TokenKind.Number).Union(StringExpandable).Union(StringLiteral).Union(Variable);

        // TokenValidatorSets
        ArrayLiteral = new PsdTokenValidationSet(BaseValues).Add(TokenKind.Comma).Add(BaseValues);
        InlineComment = new PsdTokenValidationSet(TokenKind.Comment).Add(LineTerminator);
        InlineCommentWithComma = new PsdTokenValidationSet(TokenKind.Comma).Add(TokenKind.Comment).Add(LineTerminator);
        MapEntry = new PsdTokenValidationSet(Identifier).Add(TokenKind.Equals);
    }

    public bool HasPrecedingEmptyLines()
    {
        if ((Tokens.Index - 1) < 0)
        {
            return false;
        }

        if ((Tokens.Index - 2) < 0)
        {
            return Tokens.Previous.IsNewLine;
        }

        return Tokens.Previous.IsNewLine && Tokens[Tokens.Index - 2].IsNewLine;
    }

    public bool IsArrayLiteral()
    {
        if (Tokens.TryLookAhead(3, out PsdToken[] tokens))
        {
            return ArrayLiteral.IsMatch(tokens);
        }

        return false;
    }

    public bool IsBaseValue() => BaseValues.IsMatch(_tokens.Current);

    public bool IsInlineComment()
    {
        if (Tokens.TryLookAhead(3, out PsdToken[] tokens))
        {
            return InlineComment.IsMatch(tokens) || InlineCommentWithComma.IsMatch(tokens);
        }

        return false;
    }

    public bool IsMapEntry()
    {
        if (Tokens.TryLookAhead(2, out PsdToken[] tokens))
        {
            return MapEntry.IsMatch(tokens);
        }

        return false;
    }

    internal void SetTokens(PsdTokenCollection tokens) => Tokens = tokens;
}