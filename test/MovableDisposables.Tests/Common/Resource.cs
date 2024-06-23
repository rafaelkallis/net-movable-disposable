namespace RafaelKallis.MovableDisposables.Tests.Common;

public sealed class Resource : IDisposable
{
    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        IsDisposed = true;
    }
}