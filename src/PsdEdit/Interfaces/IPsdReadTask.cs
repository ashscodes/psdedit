namespace PsdEdit;

internal interface IPsdReadTask
{
    PsdTokenManager Tokens { get; }

    void Read();
}