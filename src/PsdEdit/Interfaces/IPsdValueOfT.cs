namespace PsdEdit;

public interface IPsdValue<T> : IPsdValue
{
    bool IsNull { get; }

    T GetValue();

    void SetValue(T value);

    bool TrySetValue(object value);
}
