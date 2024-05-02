using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation.Language;

namespace PsdEdit;

internal class PsdTokenKindValidator : IEnumerable<TokenKind>, IPsdTokenValidatorItem
{
    private List<TokenKind> _tokenKinds;

    public int Count => _tokenKinds?.Count ?? 0;

    public TokenKind this[int index]
    {
        get
        {
            Ensure.IndexIsInRange(index, Count);
            return _tokenKinds[index];
        }
        set
        {
            Ensure.IndexIsInRange(index, Count);
            _tokenKinds[index] = value;
        }
    }

    public PsdTokenKindValidator() { }

    public PsdTokenKindValidator(params TokenKind[] tokenKinds) : base() => Add(tokenKinds);

    public PsdTokenKindValidator Add(params TokenKind[] tokenKinds)
    {
        if (tokenKinds.IsNullOrEmpty())
        {
            return this;
        }

        _tokenKinds ??= new List<TokenKind>();
        _tokenKinds.AddRange(tokenKinds);
        return this;
    }

    public PsdTokenKindValidator Clone() => new PsdTokenKindValidator(_tokenKinds.ToArray());

    public IEnumerator<TokenKind> GetEnumerator() => _tokenKinds.GetEnumerator();

    public bool IsMatch(PsdToken token)
    {
        Ensure.ValueNotNull(token, nameof(token));
        return _tokenKinds.Contains(token.Kind);
    }

    object ICloneable.Clone() => Clone();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}