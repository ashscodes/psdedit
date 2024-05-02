using System;
using System.Collections.Generic;

namespace PsdEdit;

public interface IPsdTokenValidator
{
    int Count { get; }
}

public interface IPsdTokenValidatorItem : ICloneable, IPsdTokenValidator
{
    bool IsMatch(PsdToken token);
}

public interface IPsdTokenValidationSet : ICloneable, IEnumerable<IPsdTokenValidatorItem>, IPsdTokenValidator
{
    bool IsMatch(PsdToken[] tokenArray, PsdTokenLookupDirection lookupDirection = PsdTokenLookupDirection.LookForward);
}