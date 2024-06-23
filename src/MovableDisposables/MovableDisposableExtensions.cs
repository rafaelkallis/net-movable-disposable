using JetBrains.Annotations;

namespace RafaelKallis.MovableDisposables;

/// <summary>
/// Extension methods for <see cref="MovableAsyncDisposable{T}"/>.
/// </summary>
public static class MovableDisposableExtensions
{
    /// <summary>
    /// Creates a new <see cref="MovableDisposable{T}"/> instance from <paramref name="resource"/>.
    /// </summary>
    [MustDisposeResource]
    public static MovableDisposable<T> ToMovable<T>([HandlesResourceDisposal] this T resource) where T : class, IDisposable =>
        new(resource);

    /// <summary>
    /// Creates a new <see cref="MovableDisposable{T}"/> instance from the result of <paramref name="resourceTask"/>.
    /// </summary>
    [MustDisposeResource]
    public static async Task<MovableDisposable<T>> ToMovable<T>([HandlesResourceDisposal] this Task<T> resourceTask) where T : class, IDisposable
    {
        if (resourceTask is null)
        {
            throw new ArgumentNullException(nameof(resourceTask));
        }
        T resource = await resourceTask.ConfigureAwait(false);
        return resource.ToMovable();
    }

    /// <summary>
    /// Creates a new <see cref="MovableDisposable{T}"/> instance from the result of <paramref name="resourceValueTask"/>.
    /// </summary>
    [MustDisposeResource]
    public static async ValueTask<MovableDisposable<T>> ToMovable<T>([HandlesResourceDisposal] this ValueTask<T> resourceValueTask) where T : class, IDisposable
    {
        T resource = await resourceValueTask.ConfigureAwait(false);
        return resource.ToMovable();
    }

    /// <summary>
    /// Creates a new <see cref="MovableAsyncDisposable{T}"/> instance from <paramref name="resource"/>.
    /// </summary>
    [MustDisposeResource]
    public static MovableAsyncDisposable<T> ToAsyncMovable<T>([HandlesResourceDisposal] this T resource) where T : class, IAsyncDisposable =>
        new(resource);

    /// <summary>
    /// Creates a new <see cref="MovableAsyncDisposable{T}"/> instance from the result of <paramref name="resourceTask"/>.
    /// </summary>
    [MustDisposeResource]
    public static async Task<MovableAsyncDisposable<T>> ToAsyncMovable<T>([HandlesResourceDisposal] this Task<T> resourceTask) where T : class, IAsyncDisposable
    {
        if (resourceTask is null)
        {
            throw new ArgumentNullException(nameof(resourceTask));
        }
        T resource = await resourceTask.ConfigureAwait(false);
        return resource.ToAsyncMovable();
    }

    /// <summary>
    /// Creates a new <see cref="MovableAsyncDisposable{T}"/> instance from the result of <paramref name="resourceValueTask"/>.
    /// </summary>
    [MustDisposeResource]
    public static async ValueTask<MovableAsyncDisposable<T>> ToAsyncMovable<T>([HandlesResourceDisposal] this ValueTask<T> resourceValueTask) where T : class, IAsyncDisposable
    {
        T resource = await resourceValueTask.ConfigureAwait(false);
        return resource.ToAsyncMovable();
    }
}