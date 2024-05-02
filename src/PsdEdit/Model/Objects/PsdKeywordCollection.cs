using System.Collections;
using System.Collections.Generic;

namespace PsdEdit;

public sealed class PsdKeywordCollection : IPsdCollection<PsdKeyword>
{
    private List<PsdKeyword> _keywords = [];

    public PsdComment Comment { get; set; }

    public int Count => _keywords is not null ? _keywords.Count : 0;

    public bool HasInlineComment => Comment is not null && !Comment.IsNull;

    public bool HasPrecedingEmptyLine { get; set; }

    public bool IsCollection => true;

    public bool IsReadOnly => false;

    public PsdKeyword this[int index]
    {
        get
        {
            Ensure.IndexIsInRange(index, Count);
            return _keywords[index];
        }
        set
        {
            Ensure.IndexIsInRange(index, Count);
            _keywords[index] = value;
        }
    }

    public PsdKeywordCollection() { }

    public PsdKeywordCollection(params PsdKeyword[] keywords) => Add(keywords);

    public void Add(PsdKeyword keyword)
    {
        _keywords ??= new List<PsdKeyword>();
        _keywords.Add(keyword);
    }

    public void Add(params PsdKeyword[] keywords)
    {
        _keywords ??= new List<PsdKeyword>();
        _keywords.AddRange(keywords);
    }

    public void Clear()
    {
        if (Count > 0)
        {
            _keywords.Clear();
            return;
        }

        _keywords = new List<PsdKeyword>();
    }

    public bool Contains(PsdKeyword keyword) => Count > 0 && _keywords.Contains(keyword);

    public void CopyTo(PsdKeyword[] array, int offset) => _keywords?.CopyTo(array, offset);

    public IEnumerator<PsdKeyword> GetEnumerator() => _keywords?.GetEnumerator();

    public void Move(int itemIndex, int targetIndex) => _keywords.Move(itemIndex, targetIndex);

    public bool Remove(PsdKeyword keyword) => Count > 0 && _keywords.Remove(keyword);

    public override string ToString()
        => Count > 0 ? string.Join("; ", _keywords) : typeof(PsdKeywordCollection).FullName;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}