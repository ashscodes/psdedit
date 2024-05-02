using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PsdEdit;

public abstract class PsdMapBase : PsdCollectionBase
{
    public List<PsdMap> Nested { get; private set; } = [];

    public PsdMapBase() : base() { }

    public PsdMapBase(string name) : base(name) { }

    public bool ContainsKey(string key) => ContainsMapEntryWithKey(key);

    public PsdMapEntry GetMapEntry(string key)
        => TryGetMapEntry(key, out PsdMapEntry mapEntry) ? mapEntry : new PsdMapEntry();

    public bool TryGetMapEntry(string key, out PsdMapEntry mapEntry)
    {
        mapEntry = null;
        if (ContainsKey(key))
        {
            mapEntry = GetObjectsOfType<PsdMapEntry>().FirstOrDefault(o => o.Key.Equals(key));
            return true;
        }

        return false;
    }

    public bool TryGetMapEntryValue(string key, out IPsdObject psdObject, bool recurse = false)
    {
        psdObject = null;
        PsdMapEntry mapEntry;
        if (TryGetMapEntry(key, out mapEntry))
        {
            psdObject = mapEntry.GetValue();
            return true;
        }

        if (recurse)
        {
            foreach (var nestedObject in Nested)
            {
                if (nestedObject.TryGetMapEntry(key, out mapEntry))
                {
                    psdObject = mapEntry.GetValue();
                    return true;
                }
            }
        }

        return false;
    }

    public override void Add(IPsdObject psdObject)
    {
        if (psdObject is not null)
        {
            base.Add(psdObject);
            AddIfNamedMap(psdObject);
        }
    }

    public override void Clear()
    {
        base.Clear();
        Nested.Clear();
    }

    public override object GetValues()
    {
        var values = new Hashtable();
        for (int i = 0; i < Count; i++)
        {
            if (this[i] is PsdMapEntry mapEntry)
            {
                IPsdObject psdObject = mapEntry.GetValue();
                object mapEntryValue = psdObject switch
                {
                    PsdCollectionBase collection => collection.GetValues(),
                    IPsdValue value => value.GetValueInternal(),
                    _ => null
                };

                if (mapEntryValue is not null)
                {
                    values.Add(mapEntry.Key, values);
                }
            }
        }

        return values;
    }

    public override bool Insert(int index, IPsdObject psdObject)
    {
        var result = base.Insert(index, psdObject);
        if (result)
        {
            AddIfNamedMap(psdObject);
        }

        return result;
    }

    public override bool Remove(IPsdObject psdObject)
    {
        var result = base.Remove(psdObject);
        if (result)
        {
            RemoveIfNamedMap(psdObject);
        }

        return result;
    }

    public override string ToString() => IsNamed ? Name : GetType().Name;

    private bool ContainsMapEntryWithKey(string key)
        => GetObjectsOfType<PsdMapEntry>().Any(o => o.Key.Equals(key));

    private void AddIfNamedMap(IPsdObject psdObject)
    {
        if (psdObject.IsNamedMap(out PsdMap map))
        {
            Nested ??= new List<PsdMap>();
            Nested.Add(map);
        }
    }

    private void RemoveIfNamedMap(IPsdObject psdObject)
    {
        if (psdObject.IsNamedMap(out PsdMap map))
        {
            Nested.Remove(map);
        }
    }
}