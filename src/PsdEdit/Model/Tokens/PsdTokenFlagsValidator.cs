using System;
using System.Management.Automation.Language;

namespace PsdEdit;

internal class PsdTokenFlagsValidator : IPsdTokenValidatorItem
{
    private TokenFlags _tokenFlags = TokenFlags.None;

    public int Count => _tokenFlags == TokenFlags.None ? 0 : 1;

    public TokenFlags Flags
    {
        get => _tokenFlags;
        set => _tokenFlags = value;
    }

    public PsdTokenFlagsValidator() { }

    public PsdTokenFlagsValidator(TokenFlags tokenFlags) : base() => _tokenFlags = tokenFlags;

    public PsdTokenFlagsValidator Clone() => new PsdTokenFlagsValidator(_tokenFlags);

    public bool IsMatch(PsdToken token)
    {
        Ensure.ValueNotNull(token, nameof(token));
        return token.HasFlag(_tokenFlags);
    }

    object ICloneable.Clone() => Clone();
}