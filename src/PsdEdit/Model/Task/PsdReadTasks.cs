using System;

namespace PsdEdit;

public sealed class PsdReadTask : PsdTask<IPsdObject, PsdReadResult, PsdMapBase>, IPsdReadTask
{
    internal PsdReadTask(PsdTokenCollection tokens,
                         string input = null) : base(PsdTokenManager.Create(tokens), input, string.Empty) { }

    internal PsdReadTask(PsdTokenManager tokens,
                         string filePath = null,
                         string input = null) : base(tokens, input, filePath) { }

    public override void AddItemToCurrentCollection(IPsdObject psdObject)
    {
        base.AddItemToCurrentCollection(psdObject);

        if (psdObject.IsNamedMap(out PsdMap map))
        {
            AddCollection(map);
        }
    }

    void IPsdReadTask.Read()
    {
        if (Tokens.IsNullOrEmpty())
        {
            Result.Errors.Add(new PsdReaderException(nameof(Tokens), Tokens.Index));
            return;
        }

        try
        {
            while (!Tokens.HasProcessedAllTokens)
            {
                if (Tokens.Current.IsMapClose)
                {
                    RemoveCollection();
                }
                else if (Tokens.Current.IsMapOpen)
                {
                    AddCollection(Tokens.ReadMap(new PsdMap(), true));
                }
                else
                {
                    AddItemToCurrentCollection(Tokens.ReadNextObject());
                }
            }
        }
        catch (Exception ex)
        {
            var readerException = new PsdReaderException(ex, Tokens.Current.Type.ToString(), Tokens.Index);
            Result.AddError(readerException);
        }
    }
}

public sealed class PsdReadKeywordConditionsTask : PsdTask<IPsdValue, PsdReadKeywordResult, PsdKeywordConditionCollection>, IPsdReadTask
{
    internal PsdReadKeywordConditionsTask(PsdTokenManager tokens) : base(tokens, string.Empty, string.Empty) { }

    public override void AddCollection(PsdKeywordConditionCollection collection)
    {
        if (CollectionTracker.Count > 1)
        {
            AddItemToCurrentCollection(collection);
        }

        base.AddCollection(collection);
    }

    void IPsdReadTask.Read()
    {
        var initialCondition = new PsdKeywordConditionCollection(Tokens.Current);
        Result.Conditions = initialCondition;
        AddCollection(initialCondition);

        try
        {
            while (CollectionTracker.Count != 0)
            {
                if (Tokens.Current.IsArrayClose)
                {
                    RemoveCollection();
                }
                else if (Tokens.Current.IsArrayOpen)
                {
                    AddCollection(new PsdKeywordConditionCollection(Tokens.Current));
                }
                else
                {
                    AddItemToCurrentCollection(Tokens.ReadBaseValue());
                }
            }
        }
        catch (Exception ex)
        {
            var readerException = new PsdReaderException(ex, Tokens.Current.Type.ToString(), Tokens.Index);
            Result.AddError(readerException);
        }
    }
}