namespace PsdEdit;

public sealed class PsdTokenManager
{
    private readonly PsdTokenCollection _tokens;

    public int Count => _tokens?.Count ?? 0;

    public PsdToken Current => _tokens.Current ?? null;

    public int Index => _tokens?.Index ?? 0;

    public PsdTokenLookup Lookup { get; }

    public PsdToken Next => _tokens.Next ?? null;

    public PsdToken Previous => _tokens.Previous ?? null;

    public PsdTokenCollection Tokens => _tokens;

    internal bool HasProcessedAllTokens => Index == Count - 1;

    private PsdTokenManager() => Lookup = PsdTokenLookup.Instance;

    private PsdTokenManager(PsdTokenCollection tokens) : this()
    {
        _tokens = tokens;
        Lookup?.SetTokens(tokens);
    }

    public void Consume(bool skipFollowingNewLines = false) => Tokens?.Consume(skipFollowingNewLines);

    public void Consume(int count, bool skipFollowingNewLines = false) => Tokens?.Consume(count, skipFollowingNewLines);

    public static PsdTokenManager Create(string filePath, out string content, IPsdTokenParser tokenParser = null)
    {
        tokenParser ??= new PsdTokenParser();
        var tokenCollection = tokenParser.Parse(filePath, out content);
        return new PsdTokenManager(tokenCollection);
    }

    internal static PsdTokenManager Create(PsdTokenCollection tokens) => new PsdTokenManager(tokens);
}