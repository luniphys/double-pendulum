using double_pendulum.Services;
using System.Numerics;

namespace double_pendulum.Tests;

/// <summary>
/// Contains xUnit tests for the PendulumPhysics, to ensure correctness of pendulum state initialization,
/// differential equations, Runge-Kutta integration, simulation step updates and correct coordinate transformation via example with randomly choosen values.
/// Also trivial physical properties are tested.
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

    #region Testing one explicit case

    /// <summary>
    /// Verifies that the initial state is correctly set with the transferred angles and zeroed angular velocities.
    /// </summary>
    [Fact]
    public void Initialization_Test()
    {
        float expectedAngle1 = (float)(angle1 * (float)Math.PI / 180.0f);
        float expectedAngle2 = (float)(angle2 * (float)Math.PI / 180.0f);

        Assert.Equal(expectedAngle1, initialState.X, precision: 5);
        Assert.Equal(expectedAngle2, initialState.Y, precision: 5);
        Assert.Equal(0.0f, initialState.Z);
        Assert.Equal(0.0f, initialState.W);
    }

    /// <summary>
    /// Verifies that the differential equation (DEQ) method returns the expected angular accelerations and transferred velocities.
    /// </summary>
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

    /// <summary>
    /// Verifies that the Runge-Kutta method produces the expected followed state for the given initial state.
    /// </summary>
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

    /// <summary>
    /// Verifies that the Step method updates the physics state directly to the expected values.
    /// </summary>
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

    /// <summary>
    /// Verifies that the PolarToEuklid method correctly converts a polar coordinate state to its Euclidean/Cartesian representation.
    /// </summary>
    [Fact]
    public void PolarToEuklid_Test()
    {
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

    /// <summary>
    /// Verifies that the GetPosition method returns the correct position values.
    /// </summary>
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

    #endregion


    #region Specific physical cases

    /// <summary>
    /// Verifies that the pendulum initialized at rest stays at rest after multiple simulation steps.
    /// </summary>
    [Fact]
    public void RestingAtZero_Test()
    {
        PendulumParameters zeroParams = new PendulumParameters(1.0f, 1.0f, 10.0f, 10.0f, 0.0f, 0.0f, 0.0f);
        PendulumPhysics zeroPhysics = new PendulumPhysics(zeroParams);

        Vector4 initialState = zeroPhysics.RK4(zeroPhysics.state);

        for (int i = 0; i < 100; i++)
        {
            zeroPhysics.Step();
        }

        Vector4 finalState = zeroPhysics.RK4(zeroPhysics.state);

        Assert.Equal(initialState[0], finalState[0], precision: 5);
        Assert.Equal(initialState[1], finalState[1], precision: 5);
        Assert.Equal(initialState[2], finalState[2], precision: 5);
        Assert.Equal(initialState[3], finalState[3], precision: 5);
    }

    /// <summary>
    /// Verifies that the calculated Cartesian positions match the expected values for various inital states.
    /// </summary>
    [Theory]
    [InlineData(1.0f, 1.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, -2.0f)] // Resting at Zero
    [InlineData(1.0f, 1.0f, 90.0f, 0.0f, 1.0f, 0.0f, 1.0f, -1.0f)] // 90 degrees
    [InlineData(1.0f, 1.0f, 180.0f, 180.0f, 0.0f, 1.0f, 0.0f, 2.0f)] // Straight up
    public void VariousPositions_Test(
        float length1, float length2, float angle1, float angle2,
        float expectedX1, float expectedY1, float expectedX2, float expectedY2)
    {
        PendulumParameters posParams = new PendulumParameters(length1, length2, 10.0f, 10.0f, angle1, angle2, 0.0f);
        PendulumPhysics posPhysics = new PendulumPhysics(posParams);

        Vector4 position = posPhysics.PolarToEuklid(posPhysics.state);

        Assert.Equal(expectedX1, position.X, precision: 5);
        Assert.Equal(expectedY1, position.Y, precision: 5);
        Assert.Equal(expectedX2, position.Z, precision: 5);
        Assert.Equal(expectedY2, position.W, precision: 5);
    }

    /// <summary>
    /// Verifies that total energy is conserved without damping and not conserved with damping.
    /// </summary>
    [Fact]
    public void EnergyConservation_Test()
    {
        PendulumParameters noDampParams = new PendulumParameters(1.0f, 1.0f, 10.0f, 10.0f, 60.0f, 45.0f, 0.0f);
        PendulumPhysics noDampPhysics = new PendulumPhysics(noDampParams);

        PendulumParameters dampParams = new PendulumParameters(1.0f, 1.0f, 10.0f, 10.0f, 60.0f, 45.0f, 10.0f);
        PendulumPhysics dampPhysics = new PendulumPhysics(dampParams);

        float TotalEnergy(Vector4 vec, float L1, float L2, float M1, float M2)
        {
            const float g = 9.81f;

            float T = (float)(0.5f * (M1 + M2) * Math.Pow(L1, 2) * Math.Pow(vec.Z, 2) +
                      0.5f * M2 * Math.Pow(L2, 2) * Math.Pow(vec.W, 2) +
                      M2 * L1 * L2 * vec.Z * vec.W * Math.Cos(vec.X - vec.Y));
            float V = (float)(-(M1 + M2) * g * L1 * Math.Cos(vec.X)
                      - M2 * g * L2 * Math.Cos(vec.Y));

            return T + V;

        }

        float initialNoDamp = TotalEnergy(noDampPhysics.state, noDampParams.Length1 , noDampParams.Length2 , noDampParams.Mass1 , noDampParams.Mass2);
        float initialDamp = TotalEnergy(dampPhysics.state, dampParams.Length1, dampParams.Length2, dampParams.Mass1, dampParams.Mass2);

        for (int i = 0; i < 100; i++)
        {
            noDampPhysics.Step();
            dampPhysics.Step();
        }

        float finalNoDamp = TotalEnergy(noDampPhysics.state, noDampParams.Length1, noDampParams.Length2, noDampParams.Mass1, noDampParams.Mass2);
        float finalDamp = TotalEnergy(dampPhysics.state, dampParams.Length1, dampParams.Length2, dampParams.Mass1, dampParams.Mass2);

        Assert.Equal(initialNoDamp, finalNoDamp, precision: 2); // (RK4 not the best at keeping energy conservation -> Low precision)
        Assert.NotEqual(initialDamp, finalDamp, precision: 2);
    }

    #endregion
}
