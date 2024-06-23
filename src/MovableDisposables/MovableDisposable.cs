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
/// public Resource CreateResource()
/// {
///   using var resource = new Resource().ToMovable();
///   ConfigureResource(resource);
///   return resource.Move();
/// }
/// </code>
/// </example>
[PublicAPI]
public struct MovableDisposable<T> : IDisposable, IEquatable<MovableDisposable<T>> where T : class, IDisposable
{
    private T? _value;

    /// <summary>
    /// Creates a new <see cref="MovableDisposable{T}"/> instance.
    /// </summary>
    public MovableDisposable([HandlesResourceDisposal] T value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The underlying resource.
    /// </summary>
    public readonly T Value =>
        _value ?? throw new InvalidOperationException("The resource has been moved or disposed.");

    /// <summary>
    /// Returns <see langword="true"/> if the underlying resource has not been moved.
    /// </summary>
    public readonly bool HasValue =>
        _value is not null;

    /// <summary>
    /// Releases ownership of the resource and delegates the responsibility of disposing the resource.
    /// The underlying resource will not be disposed when the <see cref="MovableDisposable{T}"/> is disposed.
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
    public void Dispose()
    {
        if (_value is not null)
        {
            _value.Dispose();
            _value = null;
        }
    }

    /// <summary>
    /// Implicitly converts the <see cref="MovableDisposable{T}"/> to the underlying resource.
    /// </summary>
#pragma warning disable CA2225
    public static implicit operator T(MovableDisposable<T> owned) => owned.Value;
#pragma warning restore CA2225

    /// <inheritdoc />
    public override readonly bool Equals(object? obj) =>
        obj is MovableDisposable<T> other && Equals(other);

    /// <summary>
    /// Indicates whether this instance and a specified <see cref="MovableDisposable{T}"/> are equal.
    /// </summary>
    public readonly bool Equals(MovableDisposable<T> other) =>
        EqualityComparer<MovableDisposable<T>>.Default.Equals(this, other);

    /// <inheritdoc />
    public override readonly int GetHashCode() =>
        EqualityComparer<MovableDisposable<T>>.Default.GetHashCode(this);

    /// <inheritdoc />
    public override readonly string ToString() =>
        $"{nameof(MovableDisposable<T>)}[{_value}]";

    /// <inheritdoc cref="Equals(MovableDisposable{T})" />
    public static bool operator ==(MovableDisposable<T> left, MovableDisposable<T> right) =>
        left.Equals(right);

    /// <summary>
    /// Indicates whether this instance and a specified <see cref="MovableDisposable{T}"/> are not equal.
    /// </summary>
    public static bool operator !=(MovableDisposable<T> left, MovableDisposable<T> right) =>
        !(left.Equals(right));
}