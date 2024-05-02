using System;
using System.IO;

namespace PsdEdit;

public abstract class PsdTask<T, R, C> : IDisposable
    where T : IPsdObject
    where R : PsdTaskResult<T>, new()
    where C : IPsdCollection<T>
{
    public R Result { get; protected set; }

    public FileInfo Target { get; set; }

    public PsdTokenManager Tokens { get; }

    internal PsdCollectionTracker<C, T> CollectionTracker { get; private protected set; }

    public PsdTask(string input, string target = null) : this()
    {
        Result.SetInput(input);

        if (!string.IsNullOrEmpty(target))
        {
            Target = new FileInfo(target);
        }
    }

    public PsdTask(PsdTokenManager tokens,
                   string input,
                   string target = null) : this(input, target) => Tokens = tokens;

    internal PsdTask()
    {
        CollectionTracker = new PsdCollectionTracker<C, T>();
        Result = new R();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Process()
    {
        if (this is IPsdReadTask readTask)
        {
            readTask.Read();
        }
    }

    public override string ToString() => GetType().Name;

    public virtual void AddCollection(C collection)
    {
        Tokens.Consume();
        CollectionTracker.AddCollection(collection);
    }

    public virtual void AddItemToCurrentCollection(T item)
    {
        // item can be null, collection will ultimately not add it.
        Ensure.ValueNotNull(CollectionTracker, nameof(CollectionTracker));

        if (CollectionTracker.Count == 0)
        {
            Result.AddItem(item);
            return;
        }

        CollectionTracker.AddItemToCurrentCollection(item, Tokens.Index);
    }

    public virtual void RemoveCollection()
    {
        Ensure.ValueNotNull(CollectionTracker, nameof(CollectionTracker));

        CollectionTracker.RemoveCollection(out C collection, Tokens.Index);
        Tokens.Consume();
        Tokens.TestForInlineComment(collection);

        if (CollectionTracker.Count == 0)
        {
            Result.AddItem((T)(collection as IPsdObject));
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (CollectionTracker is not null)
            {
                CollectionTracker.Dispose();
                CollectionTracker = null;
            }
        }
    }
}