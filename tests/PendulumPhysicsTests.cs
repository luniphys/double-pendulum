using double_pendulum.Services;
using System.Numerics;
using Xunit;

namespace double_pendulum.Tests;

public class PendulumPhysicsTests
{
    [Fact]
    public void Initialization_Test()
    {
        PendulumParameters parameters = new PendulumParameters(1.0f, 1.0f, 1.0f, 1.0f, 45.0f, 90.0f, 5.0f);

        PendulumPhysics physics = new PendulumPhysics(parameters);

        Assert.Equal(45.0f, physics.state.X);
        Assert.Equal(90.0f, physics.state.Y);
    }
}