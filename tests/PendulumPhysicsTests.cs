using double_pendulum.Services;
using System.Numerics;
using Xunit;

namespace double_pendulum.Tests;

public class PendulumPhysicsTests
{
    [Fact]
    public void Initialization_Test()
    {
        float Angle1Degree = 45.0f;
        float Angle2Degree = 90.0f;

        PendulumParameters parameters = new PendulumParameters(1.0f, 1.0f, 1.0f, 1.0f, Angle1Degree, Angle2Degree, 5.0f);

        PendulumPhysics physics = new PendulumPhysics(parameters);

        float Angle1Rad = (float)(Angle1Degree * Math.PI / 180.0f);
        float Angle2Rad = (float)(Angle2Degree * Math.PI / 180.0f);

        Assert.Equal(Angle1Rad, physics.state.X);
        Assert.Equal(Angle2Rad, physics.state.Y);
        Assert.Equal(0.0f, physics.state.Z);
        Assert.Equal(0.0f, physics.state.W);
    }
}