using System.Collections.Generic;

namespace PsdEdit;

public interface IPsdCollection<T> : ICollection<T>, IPsdObject
{
    T this[int index] { get; set; }

    void Move(int objectIndex, int newIndex);
}