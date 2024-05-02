using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PsdEdit;

public abstract class PsdCollectionBase : IPsdCollection<IPsdObject>
{
    private string _name = string.Empty;

    private List<IPsdObject> _objects = [];

    public PsdComment Comment { get; set; }

    public int Count => _objects is not null ? _objects.Count : 0;

    public bool HasInlineComment => Comment is not null && !Comment.IsNull;

    public bool HasPrecedingEmptyLine { get; set; }

    public bool IsCollection => true;

    public bool IsNamed => !string.IsNullOrEmpty(_name);

    public bool IsReadOnly { get; set; } = false;

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public IPsdObject this[int index]
    {
        get
        {
            Ensure.IndexIsInRange(index, Count);
            return _objects[index];
        }
        set
        {
            Ensure.IndexIsInRange(index, Count);
            _objects[index] = value;
        }
    }

    internal PsdCollectionBase() { }

    internal PsdCollectionBase(string name) : this() => Name = name;

    public bool Contains(IPsdObject psdObject) => Count > 0 && _objects.Contains(psdObject);

    public void CopyTo(IPsdObject[] array, int offset) => _objects?.CopyTo(array, offset);

    public IEnumerator<IPsdObject> GetEnumerator() => _objects.GetEnumerator();

    public virtual void Add(IPsdObject psdObject)
    {
        if (psdObject is null)
        {
            return;
        }

        _objects ??= new List<IPsdObject>();
        _objects.Add(psdObject);
    }

    public virtual void Clear()
    {
        if (Count > 0)
        {
            _objects.Clear();
            return;
        }

        _objects = new List<IPsdObject>();
    }

    public virtual bool Insert(int index, IPsdObject psdObject)
    {
        Ensure.IndexIsInRange(index, Count);
        if (!Contains(psdObject))
        {
            _objects.Insert(index, psdObject);
            return true;
        }

        return false;
    }

    public virtual void Move(int objectIndex, int newIndex) => _objects.Move(objectIndex, newIndex);

    public virtual bool Remove(IPsdObject item) => Count > 0 && _objects.Remove(item);

    public abstract object GetValues();

    internal IEnumerable<T> GetObjectsOfType<T>() where T : IPsdObject
    {
        if (Count == 0)
        {
            return Enumerable.Empty<T>();
        }

        return _objects.Where(o => o is T) as IEnumerable<T>;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}