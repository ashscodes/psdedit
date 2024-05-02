using System.Linq;
using System.Management.Automation.Language;

namespace PsdEdit;

internal static class PsdTokenValidationExtensions
{
    public static PsdTokenValidationSet Add(this PsdTokenValidationSet validatorSet, TokenFlags flags)
    {
        Ensure.ValueNotNull(validatorSet, nameof(validatorSet));

        validatorSet.Add(new PsdTokenFlagsValidator(flags));
        return validatorSet;
    }

    public static PsdTokenValidationSet Add(this PsdTokenValidationSet validatorSet, params TokenKind[] tokenKinds)
    {
        Ensure.ValueNotNull(validatorSet, nameof(validatorSet));

        validatorSet.Add(new PsdTokenKindValidator(tokenKinds));
        return validatorSet;
    }

    public static PsdTokenValidationSet Add(this PsdTokenValidationSet validatorSet, IPsdTokenValidatorItem tokenValidator)
    {
        Ensure.ValueNotNull(validatorSet, nameof(validatorSet));

        validatorSet.Add(tokenValidator);
        return validatorSet;
    }

    public static PsdTokenFlagsValidator Union(this PsdTokenFlagsValidator validator, TokenFlags flags)
    {
        validator.Flags = validator.Flags | flags;
        return validator;
    }

    public static PsdTokenKindValidator Union(this PsdTokenKindValidator validator, params TokenKind[] tokenKinds)
    {
        Ensure.ValueNotNull(validator, nameof(validator));
        if (tokenKinds.IsNullOrEmpty())
        {
            return validator;
        }

        validator.Add(tokenKinds.Where(k => !validator.Contains(k)).ToArray());
        return validator;
    }

    public static IPsdTokenValidatorItem Union(this IPsdTokenValidatorItem first, IPsdTokenValidatorItem second) => second switch
    {
        PsdTokenFlagsValidator when first is PsdTokenFlagsValidator tfv => tfv.Union(((PsdTokenFlagsValidator)second).Flags),
        PsdTokenKindValidator when first is PsdTokenKindValidator tkv => tkv.Union((PsdTokenKindValidator)second),
        _ => throw new TokenValidationException(Strings.InvalidValidationItem, first.GetType(), second.GetType())
    };

    public static PsdTokenKindValidator Union(this PsdTokenKindValidator first, PsdTokenKindValidator second)
    {
        Ensure.ValueNotNull(first, nameof(first));
        Ensure.ValueNotNull(second, nameof(second));

        return first.Union(second.ToArray());
    }
}