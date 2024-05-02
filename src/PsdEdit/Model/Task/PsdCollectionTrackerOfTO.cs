using System;
using System.Collections.Generic;

namespace PsdEdit;

internal sealed class PsdCollectionTracker<T, O> : IDisposable
    where T : IPsdCollection<O>
    where O : IPsdObject
{
    private Stack<T> _collections;

    private readonly object _lock = new object();

    public int Count => _collections?.Count ?? 0;

    internal PsdCollectionTracker() { }

    public void AddCollection(T collection)
    {
        Ensure.ValueNotNull(collection, nameof(collection));

        lock (_lock)
        {
            _collections ??= new Stack<T>();
            _collections.Push(collection);
        }
    }

    public void AddItemToCurrentCollection(O item, int tokenIndex)
    {
        lock (_lock)
        {
            Ensure.CollectionNotEmpty(_collections, nameof(_collections));
            if (_collections.TryPeek(out T collection))
            {
                collection.Add(item);
            }
            else
            {
                throw new TokenValidationException(Strings.CollectionTrackerEmpty, "addItem", tokenIndex);
            }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _collections?.Clear();
            _collections?.GetEnumerator().Dispose();
            _collections = null;
        }
    }

    public void RemoveCollection(out T collection, int tokenIndex)
    {
        lock (_lock)
        {
            Ensure.ValueNotNull(_collections, nameof(_collections));
            if (!_collections.TryPop(out collection))
            {
                throw new TokenValidationException(Strings.CollectionTrackerEmpty, "remove", tokenIndex);
            }
        }
    }

    public bool TryAddCollection(T collection)
    {
        lock (_lock)
        {
            _collections ??= new Stack<T>();
            if (collection is not null)
            {
                _collections.Push(collection);
                return true;
            }

            return false;
        }
    }

    public bool TryAddCollectionMember(O item)
    {
        lock (_lock)
        {
            if (_collections is null)
            {
                return false;
            }

            var tryPeek = _collections.TryPeek(out T collection);
            if (tryPeek)
            {
                Ensure.ValueNotNull((ICollection<O>)collection, nameof(collection));
                collection?.Add(item);
            }

            return tryPeek;
        }
    }

    public bool TryRemoveCollection(out T collection)
    {
        lock (_lock)
        {
            Ensure.ValueNotNull(_collections, nameof(_collections));
            return _collections.TryPop(out collection);
        }
    }

    void IDisposable.Dispose() => Dispose();
}