using System;
using System.Linq;

namespace PsdEdit;

public abstract class PsdOperatorBase : PsdStringBase
{
    public PsdOperatorBase(string value) : base(value, false) { }

    public override bool TrySetValue(object value)
    {
        if (value is string stringValue)
        {
            string[] operators = this switch
            {
                PsdComparisonOperator => Strings.ComparisonOperators,
                PsdLogicalOperator => Strings.LogicalOperators,
                _ => []
            };

            if (operators.Contains(stringValue, StringComparer.OrdinalIgnoreCase))
            {
                return base.TrySetValue(stringValue);
            }
        }

        return false;
    }
}