using System;
using System.Linq;

namespace PsdEdit;

public abstract class PsdArrayBase : PsdCollectionBase
{
    internal bool ShouldWrapValues => Count > 3;

    internal PsdArrayBase() { }

    public override object GetValues()
    {
        var userValues = this.Where(i => i is PsdCollectionBase || i is IPsdValue);
        if (userValues.Any())
        {
            var values = new object[userValues.Count()];
            for (int i = 0; i < Count; i++)
            {
                var currentItem = this[i];
                var value = currentItem switch
                {
                    PsdCollectionBase collection => collection.GetValues(),
                    PsdBoolean boolValue => boolValue.GetValue(),
                    PsdNumber numberValue => numberValue.GetValue(),
                    PsdStringBase strValue => strValue.GetValue(),
                    _ => null,
                };
            }

            return values;
        }

        return Array.Empty<IPsdObject>();
    }
}