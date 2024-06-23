namespace RafaelKallis.MovableDisposables;

/// <summary>
/// <para>
/// Wraps an unmanaged (disposable) resource and allows using the resource across contexts.
/// Ownership can be transferred, that is, the responsibility of disposing the resource can be <b>moved</b> across contexts.
/// </para>
/// <para>
/// The resource can be accessed using the <see cref="Value"/> property.
/// </para>
/// </summary>
/// <example>
/// The following example demonstrates how to create a resource and move it across contexts:
/// <code>
/// public async Task&lt;Resource&gt; CreateResource()
/// {
///   await using var resource = new Resource().ToAsyncMovable();
///   await ConfigureResource(resource);
///   return resource.Move();
/// }
/// </code>
/// </example>
[PublicAPI]
public sealed class MovableAsyncDisposable<T> : IAsyncDisposable where T : class, IAsyncDisposable
{
    private T? _value;

    /// <summary>
    /// Creates a new <see cref="MovableAsyncDisposable{T}"/> instance.
    /// </summary>
    public MovableAsyncDisposable([HandlesResourceDisposal] T value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The underlying resource.
    /// </summary>
    /// <returns>The underlying resource.</returns>
    /// <exception cref="InvalidOperationException">The resource has been moved or disposed.</exception>
    public T Value =>
        _value ?? throw new InvalidOperationException("The resource has been moved or disposed.");

    /// <summary>
    /// Returns <see langword="true"/> if the underlying resource has not been moved.
    /// </summary>
    public bool HasValue =>
        _value is not null;

    /// <summary>
    /// Releases ownership of the resource and delegates the responsibility of disposing the resource.
    /// The underlying resource will not be disposed when the <see cref="MovableAsyncDisposable{T}"/> is disposed.
    /// Any subsequent calls to <see cref="Value"/> will throw an <see cref="InvalidOperationException"/>.
    /// </summary>
    [MustDisposeResource]
    public T Move()
    {
        T value = Value;
        _value = null;
        return value;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_value is not null)
        {
            await _value.DisposeAsync().ConfigureAwait(false);
            _value = null;
        }
    }

    /// <summary>
    /// Implicitly converts the <see cref="MovableAsyncDisposable{T}"/> to the underlying resource.
    /// </summary>
#pragma warning disable CA2225
    public static implicit operator T(MovableAsyncDisposable<T> owned)
#pragma warning restore CA2225
    {
        if (owned is null)
        {
#pragma warning disable CA1065
            throw new ArgumentNullException(nameof(owned));
#pragma warning restore CA1065
        }
        return owned.Value;
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"{nameof(MovableAsyncDisposable<T>)}[{_value}]";
}