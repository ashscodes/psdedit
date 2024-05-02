using System;
using System.Collections.Generic;

namespace PsdEdit;

public abstract class PsdTaskResult<T> where T : IPsdObject
{
    public int Count => Items?.Count ?? 0;

    public List<Exception> Errors { get; private set; }

    public string Input { get; private set; }

    public List<T> Items { get; private set; }

    public bool Success => (Errors?.Count ?? 0) == 0;

    internal PsdTaskResult() { }

    internal virtual void AddError(Exception error)
    {
        Errors ??= new List<Exception>();

        if (error is not null)
        {
            Errors.Add(error);
        }
    }

    internal virtual void AddItem(T item)
    {
        Items ??= new List<T>();

        if (item is not null)
        {
            Items.Add(item);
        }
    }

    internal void SetInput(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            Input = input;
        }
    }
}