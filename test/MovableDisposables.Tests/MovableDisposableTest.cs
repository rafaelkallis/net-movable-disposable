using RafaelKallis.MovableDisposables.Tests.Common;

namespace RafaelKallis.MovableDisposables.Tests;

public class MovableDisposableTest
{
    [Fact]
    public void WhenMovableIsDisposed_ThenShouldDisposeResource()
    {
        using Resource resource = new();

        resource.IsDisposed.Should().BeFalse();

        MovableDisposable<Resource> movable = resource.ToMovable();

        movable.Value.IsDisposed.Should().BeFalse();

        movable.Dispose();

        resource.IsDisposed.Should().BeTrue();

        movable.HasValue.Should().BeFalse();
        Func<Resource> act = () => movable.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void WhenMovableIsMoved_ThenShouldNotDisposeResource()
    {
        using Resource resource = new();

        resource.IsDisposed.Should().BeFalse();

        MovableDisposable<Resource> movable = resource.ToMovable();

        movable.Value.IsDisposed.Should().BeFalse();

        using Resource resource2 = movable.Move();

        resource.Should().Be(resource2);
        resource.IsDisposed.Should().BeFalse();
        movable.HasValue.Should().BeFalse();
        Func<Resource> act = () => movable.Value;
        act.Should().Throw<InvalidOperationException>();

        movable.Dispose();
        resource.IsDisposed.Should().BeFalse("disposing a movable that was moved should not dispose the resource");
    }

    [Fact]
    public async Task WhenMovableIsCreatedFromExtensions()
    {
        using Resource resource = new();

        using MovableDisposable<Resource> movable1 = resource.ToMovable();
        movable1.Value.Should().Be(resource);

        using MovableDisposable<Resource> movable2 = await Task.FromResult(resource).ToMovable();
        movable2.Value.Should().Be(resource);

        using MovableDisposable<Resource> movable3 = await new ValueTask<Resource>(resource).ToMovable();
        movable3.Value.Should().Be(resource);
    }
}