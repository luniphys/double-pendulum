using double_pendulum.Services;
using System.Numerics;
using Xunit;

namespace double_pendulum.Tests;

public class PendulumPhysicsTests
{
    [Fact]
    public void Initialization_Test()
    {
        var parameters = new PendulumParameters { Angle1 = 1.0f, Angle2 = 0.5f };

        var physics = new PendulumPhysics(parameters);

        Assert.Equal(1.0f, physics.state.X);
        Assert.Equal(0.5f, physics.state.Y);
    }
}