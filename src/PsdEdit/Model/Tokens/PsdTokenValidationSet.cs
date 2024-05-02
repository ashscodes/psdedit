using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation.Language;

namespace PsdEdit;

public class PsdTokenValidationSet : IPsdTokenValidationSet
{
    private List<IPsdTokenValidatorItem> _validators;

    public int Count => _validators?.Count ?? 0;

    public IPsdTokenValidatorItem this[int index]
    {
        get
        {
            Ensure.IndexIsInRange(index, Count);
            return _validators[index];
        }
        set
        {
            Ensure.IndexIsInRange(index, Count);
            _validators[index] = value;
        }
    }

    public PsdTokenValidationSet() => _validators = new List<IPsdTokenValidatorItem>();

    public PsdTokenValidationSet(TokenFlags tokenFlags) : this() => Add(tokenFlags);

    public PsdTokenValidationSet(params TokenKind[] tokenKinds) : this() => Add(tokenKinds);

    public PsdTokenValidationSet(IPsdTokenValidatorItem validatorItem) : this() => Add(validatorItem);

    public PsdTokenValidationSet Add(TokenFlags tokenFlags)
    {
        _validators ??= new List<IPsdTokenValidatorItem>();
        _validators.Add(new PsdTokenFlagsValidator(tokenFlags));
        return this;
    }

    public PsdTokenValidationSet Add(params TokenKind[] tokenKinds)
    {
        if (tokenKinds.IsNullOrEmpty())
        {
            return this;
        }

        _validators ??= new List<IPsdTokenValidatorItem>();
        _validators.Add(new PsdTokenKindValidator(tokenKinds));
        return this;
    }

    public PsdTokenValidationSet Clone()
    {
        var tokenValidatorSet = new PsdTokenValidationSet();
        foreach (var validationItem in _validators)
        {
            tokenValidatorSet.Add((IPsdTokenValidatorItem)validationItem.Clone());
        }

        return tokenValidatorSet;
    }

    public IEnumerator<IPsdTokenValidatorItem> GetEnumerator() => _validators.GetEnumerator();

    public bool IsMatch(PsdToken[] tokenArray, PsdTokenLookupDirection lookupDirection = PsdTokenLookupDirection.LookForward)
    {
        Ensure.ArrayNotEmpty(tokenArray, nameof(tokenArray));
        Ensure.CountIsGreaterThanOrEqualTo(tokenArray.Length, nameof(tokenArray), Count);

        for (int i = 0; i < Count; i++)
        {
            int index = lookupDirection switch
            {
                PsdTokenLookupDirection.LookBehind => (Count - 1) - i,
                _ => i
            };

            if (!_validators[index].IsMatch(tokenArray[index]))
            {
                return false;
            }
        }

        return true;
    }

    internal PsdTokenValidationSet Add(IPsdTokenValidatorItem tokenValidator)
    {
        Ensure.ValueNotNull(tokenValidator, nameof(tokenValidator));

        _validators.Add((IPsdTokenValidatorItem)tokenValidator.Clone());
        return this;
    }

    object ICloneable.Clone() => Clone();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}