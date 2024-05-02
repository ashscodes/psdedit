using System.Management.Automation.Language;

namespace PsdEdit;

public enum PsdTokenValueType
{
    Boolean = 1,
    Comment = 3,
    ComparisonOperator = 5,
    LogicalOperator = 6,
    None = 0,
    Number = 2,
    ScriptBlock = 7,
    StringExpandable = 8,
    StringLiteral = 9,
    Variable = 4,
}

public record PsdToken
{
    private readonly Token _token;

    internal static readonly PsdToken Null = new PsdToken();

    public bool IsArrayClose { get; }

    public bool IsArrayOpen { get; }

    public bool IsComma { get; }

    public bool IsComment { get; }

    public bool IsComparisonOperator { get; }

    public bool IsExpandableString { get; }

    public bool IsHereString { get; }

    public bool IsKeyword { get; }

    public bool IsLineTerminator { get; }

    public bool IsLogicalOperator { get; }

    public bool IsMapClose { get; }

    public bool IsMapOpen { get; }

    public bool IsNestedMap { get; }

    public bool IsNewLine { get; }

    public TokenKind Kind { get; }

    public string Text { get; }

    public PsdTokenValueType Type { get; }

    public PsdToken(Token token)
    {
        Ensure.ValueNotNull(token, nameof(token));

        Kind = token.Kind;
        Text = token.Text;
        Type = ToValueType(token);

        _token = token;
        IsArrayClose = token.Kind == TokenKind.RParen;
        IsArrayOpen = token.Kind == TokenKind.AtParen || token.Kind == TokenKind.LParen;
        IsComma = token.Kind == TokenKind.Comma;
        IsComment = token.Kind == TokenKind.Comment;
        IsComparisonOperator = token.TokenFlags.HasFlag(TokenFlags.BinaryPrecedenceComparison);
        IsExpandableString = Kind == TokenKind.HereStringExpandable || Kind == TokenKind.StringExpandable;
        IsHereString = token.Kind == TokenKind.HereStringExpandable || token.Kind == TokenKind.HereStringLiteral;
        IsKeyword = token.TokenFlags.HasFlag(TokenFlags.Keyword);
        IsLogicalOperator = token.TokenFlags.HasFlag(TokenFlags.BinaryPrecedenceLogical);
        IsMapClose = token.Kind == TokenKind.RCurly;
        IsMapOpen = token.Kind == TokenKind.AtCurly;
        IsNestedMap = token.Kind == TokenKind.LCurly;
        IsNewLine = token.Kind == TokenKind.NewLine;

        // Relies on conditions above.
        IsLineTerminator = IsNewLine || token.Kind == TokenKind.Semi;
    }

    private PsdToken()
    {
        _token = null;
        Kind = TokenKind.Unknown;
        Text = string.Empty;
        Type = PsdTokenValueType.None;
    }

    public bool HasFlag(TokenFlags tokenFlags) => _token?.TokenFlags.HasFlag(tokenFlags) ?? false;

    public IPsdValue ToPsdValue()
    {
        string value = ToString();

        return Type switch
        {
            PsdTokenValueType.Boolean => new PsdBoolean(value),
            PsdTokenValueType.Comment => new PsdComment(value),
            PsdTokenValueType.ComparisonOperator => new PsdComparisonOperator(value),
            PsdTokenValueType.LogicalOperator => new PsdLogicalOperator(value),
            PsdTokenValueType.Number => new PsdNumber(value),
            PsdTokenValueType.StringExpandable => new PsdStringExpandable(this),
            PsdTokenValueType.StringLiteral => new PsdStringLiteral(this),
            PsdTokenValueType.Variable => new PsdVariable(value, _token.Kind == TokenKind.SplattedVariable),
            _ => new PsdScriptBlockValue(this)
        };
    }

    public override string ToString()
    {
        return _token switch
        {
            NumberToken numberToken => numberToken.Value.ToString(),
            StringToken stringToken => stringToken.Value,
            VariableToken variableToken => variableToken.Name,
            Token => _token.Text,
            _ => string.Empty
        };
    }

    public static explicit operator PsdToken(Token token) => new(token);

    public static implicit operator Token(PsdToken psdToken) => psdToken._token;

    private PsdTokenValueType ToValueType(Token token) => token switch
    {
        NumberToken => PsdTokenValueType.Number,
        StringToken when token.Kind == TokenKind.Comment => PsdTokenValueType.Comment,
        StringExpandableToken => PsdTokenValueType.StringExpandable,
        StringLiteralToken => PsdTokenValueType.StringLiteral,
        VariableToken varToken when varToken.Name.Equals("true") || varToken.Name.Equals("false") => PsdTokenValueType.Boolean,
        VariableToken => PsdTokenValueType.Variable,
        Token when token.TokenFlags.HasFlag(TokenFlags.BinaryPrecedenceComparison) => PsdTokenValueType.ComparisonOperator,
        Token when token.TokenFlags.HasFlag(TokenFlags.BinaryPrecedenceLogical) => PsdTokenValueType.LogicalOperator,
        _ => PsdTokenValueType.ScriptBlock
    };
}