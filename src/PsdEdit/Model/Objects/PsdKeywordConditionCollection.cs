using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PsdEdit;

public sealed class PsdKeywordConditionCollection : IPsdCollection<IPsdValue>, IPsdValue
{
    private List<IPsdValue> _conditions = [];

    // Single whitespace character.
    private char _space = (char)32;

    private PsdToken _token;

    public PsdComment Comment { get; set; }

    public int Count => _conditions?.Count ?? 0;

    public bool IsReadOnly => false;

    public bool HasInlineComment => Comment is not null && !Comment.IsNull;

    public bool HasPrecedingEmptyLine { get; set; }

    public bool IsCollection => true;

    public IPsdValue this[int index]
    {
        get
        {
            Ensure.IndexIsInRange(index, Count);
            return _conditions[index];
        }
        set
        {
            Ensure.IndexIsInRange(index, Count);
            _conditions[index] = value;
        }
    }

    internal PsdKeywordConditionCollection(PsdToken token) => _token = token;

    public void Add(IPsdValue condition)
    {
        _conditions ??= new List<IPsdValue>();
        _conditions.Add(condition);
    }

    public void Clear()
    {
        if (Count > 0)
        {
            _conditions.Clear();
            return;
        }

        _conditions = new List<IPsdValue>();
    }

    public bool Contains(IPsdValue condition) => Count > 0 && _conditions.Contains(condition);

    public void CopyTo(IPsdValue[] array, int offset) => _conditions?.CopyTo(array, offset);

    public IEnumerator<IPsdValue> GetEnumerator() => _conditions.GetEnumerator();

    public override string ToString()
    {
        if (Count == 0)
        {
            return string.Empty;
        }

        var strBuilder = new StringBuilder(_token.Text ?? string.Empty);

        for (int i = 0; i < Count; i++)
        {
            strBuilder.Append(_conditions[i].ToString());

            if (i < (Count - 1))
            {
                strBuilder.Append(_space);
            }
        }

        strBuilder.Append(_token.Text is not null ? ")" : string.Empty);
        return strBuilder.ToString();
    }

    public void Move(int objectIndex, int newIndex) => _conditions.Move(objectIndex, newIndex);

    public bool Remove(IPsdValue condition) => Count > 0 && _conditions.Remove(condition);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}