namespace Ping.Client.Counter.Services;

internal sealed class SimpleCounter
{
    public int Value { get; private set; }

    public event Action? Changed;

    public void Decrement()
    {
        Value--;

        Changed?.Invoke();
    }

    public void Increment()
    {
        Value++;

        Changed?.Invoke();
    }

    public void Reset()
    {
        Value = 0;

        Changed?.Invoke();
    }
}
