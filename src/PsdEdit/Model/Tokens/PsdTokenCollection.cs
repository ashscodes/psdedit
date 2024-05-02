using System;
using System.Collections;

namespace PsdEdit;

public class PsdTokenCollection : IEnumerable
{
    private PsdToken[] _tokens;

    public int Count => _tokens?.Length ?? 0;

    public PsdToken Current => (Count <= 0 || (Index < 0 || Index > Count)) ? null : this[Index];

    public int Index { get; private set; }

    public PsdToken Next => Index + 1 >= Count ? PsdToken.Null : this[Index + 1];

    public PsdToken Previous => Index - 1 < 0 ? PsdToken.Null : this[Index - 1];

    public PsdToken this[int index]
    {
        get
        {
            Ensure.IndexIsInRange(index, Count);
            return _tokens[index];
        }
    }

    public PsdTokenCollection() => Index = 0;

    public void Add(params PsdToken[] tokens) => _tokens = tokens;

    public void Consume(bool skipFollowingNewLines = false)
    {
        Ensure.IndexIsInRange(Index + 1, Count);

        Index++;
        if (skipFollowingNewLines)
        {
            if (Current.IsNewLine && (Next.IsNewLine || Next.IsKeyword))
            {
                Consume(skipFollowingNewLines);
            }
        }
    }

    public void Consume(int count, bool skipFollowingNewLines = false)
    {
        for (int i = 0; i < count; i++)
        {
            Consume(skipFollowingNewLines);
        }
    }

    public void Copy(PsdToken[] destinationArray,
                     int length,
                     int destinationIndex = 0,
                     int sourceIndex = 0) => Array.Copy(_tokens, sourceIndex, destinationArray, destinationIndex, length);

    public IEnumerator GetEnumerator() => _tokens.GetEnumerator();

    // LookAhead takes 'x' tokens including the current token.
    internal PsdToken[] LookAhead(int tokenCount)
    {
        Ensure.IndexIsInRange((Index + tokenCount) - 1, Count);

        var result = new PsdToken[tokenCount];
        Copy(result, tokenCount, 0, Index);
        return result;
    }

    // LookBehind takes 'x' tokens starting at the previous token.
    internal PsdToken[] LookBehind(int tokenCount)
    {
        int startIndex = Index - tokenCount;
        Ensure.IndexIsInRange(startIndex, Count);

        return _tokens.CopyInReverse(startIndex, Index - 1);
    }

    internal bool TryLookAhead(int tokenCount, out PsdToken[] tokens)
    {
        tokens = [];

        try
        {
            tokens = LookAhead(tokenCount);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    internal bool TryLookBehind(int tokenCount, out PsdToken[] tokens)
    {
        tokens = [];

        try
        {
            tokens = LookBehind(tokenCount);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}