# .NET Movable Disposable

[![nuget-stable](https://img.shields.io/nuget/v/RafaelKallis.MovableDisposables.svg?label=stable)](https://www.nuget.org/packages/RafaelKallis.MovableDisposables/)

In .NET, objects that include [unmanaged resources](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/unmanaged)
must implement `IDisposable`, or `IAsyncDisposable`, and they must be explicitly released when they are no longer needed.

This can sometimes lead to complex and error-prone code, for example when [stacked usings](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#stacked-usings)
are used, or factory methods that transfer ownership of unmanaged resources to the caller.

This library introduces the `MovableDisposable<T>` and `MovableAsyncDisposable<T>` types that allow for the transfer of ownership of disposable objects to the caller.
This results in cleaner and more readable code, and reduces the risk of resource leaks.

```cs
public static void Main()
{
    using File file = OpenFile("example.txt");
    MayThrow(file); // file is disposed if an exception is thrown
    // file is disposed
}

public File OpenFile(string path)
{
    using MovableDisposable<File> file = File.OpenWrite(path).ToMovable();
    MayThrow(file); // file is disposed if an exception is thrown
    return file.Move(); // file is not disposed, ownership is transferred to the caller
}
```

## Related

- [Learn .NET](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/using-objects) Using objects that implement IDisposable
- [dotnet/runtime #1163](https://github.com/dotnet/runtime/issues/1163) Proposal: New API of "move semantics" for IDisposable

## Getting Started

### Step 1: Install Package

Add a reference to the [RafaelKallis.MovableDisposables](https://www.nuget.org/packages/RafaelKallis.MovableDisposables) package.

```sh
dotnet add package RafaelKallis.MovableDisposables
```

## Example

The following example shows a factory method `CreateResourceAsync()` with stacked usings that transfers ownership of three hypothetical unmanaged resources `Lock`, `Connection`, and `Transaction` to the caller.
Since ownership of the resources is intended to be transferred to the caller, the [`using`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/using) statement cannot be used as it would dispose the resources when the method returns.

```cs
public Resource CreateResource()
{
    Lock @lock = _lockManager.CreateLock();
    try
    {
        Connection connection = _connectionManager.CreateConnection();
        try 
        {
            Transaction transaction = await connection.BeginTransaction();
            try 
            {
                // ownership of the lock, connection, and transaction is transferred to the Resource
                return new Resource(@lock, connection, transaction);
            }
            catch
            {
                transaction.Dispose();
                throw;
            }
        } 
        catch 
        {
            connection.Dispose();
            throw;
        }
    }
    catch
    {
        @lock.Dispose();
        throw;
    }
}
```

Using `MovableDisposable<T>`, or `MovableAsyncDisposable<T>`, the code can be simplified as follows:

```cs
public Resource CreateResource()
{
    using MovableDisposable<Lock> @lock = _lockManager.CreateLock().ToMovable();
    using MovableDisposable<Connection> connection = _connectionManager.CreateConnection().ToMovable();
    using MovableDisposable<Transaction> transaction = connection.Value.BeginTransaction().ToMovable();
    return new Resource(@lock.Move(), connection.Move(), transaction.Move());
}

```
