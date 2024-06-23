using RafaelKallis.MovableDisposables.Tests.Common;

namespace RafaelKallis.MovableDisposables.Tests;

public class MovableAsyncDisposableTest
{

    [Fact]
    public async Task WhenAsyncMovableIsDisposed_ThenShouldDisposeResource()
    {
        await using AsyncResource resource = new();

        resource.IsDisposedAsync.Should().BeFalse();

        MovableAsyncDisposable<AsyncResource> movable = resource.ToAsyncMovable();

        movable.Value.IsDisposedAsync.Should().BeFalse();

        await movable.DisposeAsync();

        resource.IsDisposedAsync.Should().BeTrue();

        movable.HasValue.Should().BeFalse();
        Func<AsyncResource> act = () => movable.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public async Task WhenAsyncMovableIsMoved_ThenShouldNotDisposeResource()
    {
        await using AsyncResource resource = new();

        resource.IsDisposedAsync.Should().BeFalse();

        MovableAsyncDisposable<AsyncResource> movable = resource.ToAsyncMovable();

        movable.Value.IsDisposedAsync.Should().BeFalse();

        await using AsyncResource resource2 = movable.Move();

        resource.Should().Be(resource2);
        resource.IsDisposedAsync.Should().BeFalse();
        movable.HasValue.Should().BeFalse();
        Func<AsyncResource> act = () => movable.Value;
        act.Should().Throw<InvalidOperationException>();

        await movable.DisposeAsync();
        resource.IsDisposedAsync.Should().BeFalse("disposing a movable that was moved should not dispose the resource");
    }

    [Fact]
    public async Task WhenAsyncMovableIsCreatedFromExtensions()
    {
        await using AsyncResource resource = new();

        await using MovableAsyncDisposable<AsyncResource> movable1 = resource.ToAsyncMovable();
        movable1.Value.Should().Be(resource);

        await using MovableAsyncDisposable<AsyncResource> movable2 = await Task.FromResult(resource).ToAsyncMovable();
        movable2.Value.Should().Be(resource);

        await using MovableAsyncDisposable<AsyncResource> movable3 = await new ValueTask<AsyncResource>(resource).ToAsyncMovable();
        movable3.Value.Should().Be(resource);
    }
}