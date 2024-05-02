namespace PsdEdit;

public sealed class PsdReadResult : PsdTaskResult<IPsdObject>
{
    public PsdReadResult() : base() { }
}

public sealed class PsdReadKeywordResult : PsdTaskResult<IPsdValue>
{
    internal PsdKeywordConditionCollection Conditions { get; set; }

    public PsdReadKeywordResult() : base() { }
}