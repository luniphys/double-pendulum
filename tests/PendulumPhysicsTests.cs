using double_pendulum.Services;
using System.Numerics;

namespace double_pendulum.Tests;

/// <summary>
/// Contains xUnit tests for the PendulumPhysics, to ensure correctness of pendulum state initialization,
/// differential equations, Runge-Kutta integration, simulation step updates and correct coordinate transformation via example with randomly choosen values.
/// </summary>

public class PendulumPhysicsTests
{
    float angle1;
    float angle2;

    float angularVelocity1;
    float angularVelocity2;

    PendulumParameters parameters;
    PendulumPhysics physics;

    Vector4 initialState;
    Vector4 state;


    public PendulumPhysicsTests()
    {
        angle1 = 95.4f;
        angle2 = -37.3f;

        angularVelocity1 = -6.2f;
        angularVelocity2 = 0.3f;

        parameters = new PendulumParameters(1.3f, 2.0f, 9.4f, 10.6f, angle1, angle2, 5.2f);
        physics = new PendulumPhysics(parameters);

        initialState = physics.state;

        state = physics.state;
        state.Z = angularVelocity1;
        state.W = angularVelocity2;
        physics.state = state; // Needed to push the modified Z,W values into physics
    }


    [Fact] // xUnit-Testframework Attribut. Markiert Methode als Unit Test ohne Parameter.
    public void Initialization_Test()
    {
        float expectedAngle1 = (float)(angle1 * (float)Math.PI / 180.0f);
        float expectedAngle2 = (float)(angle2 * (float)Math.PI / 180.0f);

        Assert.Equal(expectedAngle1, initialState.X, precision: 5);
        Assert.Equal(expectedAngle2, initialState.Y, precision: 5);
        Assert.Equal(0.0f, initialState.Z);
        Assert.Equal(0.0f, initialState.W);
    }

    [Fact]
    public void DEQ_Test()
    {
        Vector4 stateDEQ = physics.DEQ(state);

        float expectedAngularAcceleration1 = 7.1802826f;
        float expectedAngularAcceleration2 = 24.426456f;

        Assert.Equal(angularVelocity1, stateDEQ.X);
        Assert.Equal(angularVelocity2, stateDEQ.Y);
        Assert.Equal(expectedAngularAcceleration1, stateDEQ.Z, precision: 5);
        Assert.Equal(expectedAngularAcceleration2, stateDEQ.W, precision: 5);
    }

    [Fact]
    public void RK4_Test()
    {
        float expectedAngle1 = 1.6033831f;
        float expectedAngle2 = -0.6467889f;
        float expectedAngularVelocity1 = -6.134317f;
        float expectedAngularVelocity2 = 0.54352266f;

        Vector4 stateRK4 = physics.RK4(state);

        Assert.Equal(expectedAngle1, stateRK4.X, precision: 5);
        Assert.Equal(expectedAngle2, stateRK4.Y, precision: 5);
        Assert.Equal(expectedAngularVelocity1, stateRK4.Z, precision: 5);
        Assert.Equal(expectedAngularVelocity2, stateRK4.W, precision: 5);
    }

    [Fact]
    public void Step_Test()
    {
        float expectedAngle1 = 1.6033831f;
        float expectedAngle2 = -0.6467889f;
        float expectedAngularVelocity1 = -6.134317f;
        float expectedAngularVelocity2 = 0.54352266f;

        physics.Step();

        Assert.Equal(expectedAngle1, physics.state.X, precision: 5); // Without physics. the state with 0 value for Z,W would be used.
        Assert.Equal(expectedAngle2, physics.state.Y, precision: 5);
        Assert.Equal(expectedAngularVelocity1, physics.state.Z, precision: 5);
        Assert.Equal(expectedAngularVelocity2, physics.state.W, precision: 5);
    }

    [Fact]
    public void PolarToEuklid_Test()
    {
        Vector4 euklidCoordinates = physics.PolarToEuklid(state);

        float expectedX1 = 1.2942305f;
        float expectedY1 = 0.12234091f;
        float expectedX2 = 0.08225376f;
        float expectedY2 = -1.4686061f;

        Vector4 euklidState = physics.PolarToEuklid(state);

        Assert.Equal(expectedX1, euklidState[0], precision: 5);
        Assert.Equal(expectedY1, euklidState[1], precision: 5);
        Assert.Equal(expectedX2, euklidState[2], precision: 5);
        Assert.Equal(expectedY2, euklidState[3], precision: 5);
    }

    [Fact]
    public void GetPosition_Test()
    {
        float expectedX1 = 1.2942305f;
        float expectedY1 = 0.12234091f;
        float expectedX2 = 0.08225376f;
        float expectedY2 = -1.4686061f;

        Vector4 positionState = physics.GetPosition();

        Assert.Equal(expectedX1, positionState[0], precision: 5);
        Assert.Equal(expectedY1, positionState[1], precision: 5);
        Assert.Equal(expectedX2, positionState[2], precision: 5);
        Assert.Equal(expectedY2, positionState[3], precision: 5);
    }
}
