namespace RafaelKallis.MovableDisposables.Tests.Common;

public sealed class AsyncResource : IAsyncDisposable
{
    public bool IsDisposedAsync { get; private set; }

    public async ValueTask DisposeAsync()
    {
        await ValueTask.CompletedTask;
        IsDisposedAsync = true;
    }
}