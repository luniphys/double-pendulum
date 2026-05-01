using double_pendulum.Services;
using System.Numerics;

namespace double_pendulum.Tests;

/// <summary>
/// Contains xUnit tests for the PendulumPhysics, to ensure correctness of pendulum state initialization,
/// differential equations, Runge-Kutta integration, simulation step updates and correct coordinate transformation via example with randomly chosen values.
/// Also trivial physical properties are tested.
/// </summary>

public class PendulumPhysicsTests
{
    private const float DegreesToRadians = MathF.PI / 180.0f;

    private readonly float _angle1;
    private readonly float _angle2;

    private readonly float _angularVelocity1;
    private readonly float _angularVelocity2;

    private readonly PendulumParameters _parameters;
    private readonly PendulumPhysics _physics;

    private readonly Vector4 _initialState;
    private readonly Vector4 _state;



    public PendulumPhysicsTests()
    {
        _angle1 = 95.4f;
        _angle2 = -37.3f;

        _angularVelocity1 = -6.2f;
        _angularVelocity2 = 0.3f;

        _parameters = new PendulumParameters(1.3f, 2.0f, 9.4f, 10.6f, _angle1, _angle2, 5.2f);
        _physics = new PendulumPhysics(_parameters);

        _initialState = _physics.State;

        _state = _physics.State;
        _state.Z = _angularVelocity1;
        _state.W = _angularVelocity2;
    }



    #region Testing one explicit case

    /// <summary>
    /// Verifies that the initial state is correctly set with the transferred angles and zeroed angular velocities.
    /// </summary>
    [Fact]
    public void Initialization_Test()
    {
        float expectedAngle1 = _angle1 * DegreesToRadians;
        float expectedAngle2 = _angle2 * DegreesToRadians;

        Assert.Equal(expectedAngle1, _initialState.X, precision: 5);
        Assert.Equal(expectedAngle2, _initialState.Y, precision: 5);
        Assert.Equal(0.0f, _initialState.Z);
        Assert.Equal(0.0f, _initialState.W);
    }

    /// <summary>
    /// Verifies that the Derivative method returns the expected angular accelerations and transferred velocities.
    /// </summary>
    [Fact]
    public void Derivative_Test()
    {
        Vector4 stateDerivative = _physics.Derivative(_state);

        float expectedAngularAcceleration1 = 7.1802826f;
        float expectedAngularAcceleration2 = 24.426456f;

        Assert.Equal(_angularVelocity1, stateDerivative.X);
        Assert.Equal(_angularVelocity2, stateDerivative.Y);
        Assert.Equal(expectedAngularAcceleration1, stateDerivative.Z, precision: 5);
        Assert.Equal(expectedAngularAcceleration2, stateDerivative.W, precision: 5);
    }

    /// <summary>
    /// Verifies that the Runge-Kutta method produces the expected followed state for the given initial state.
    /// </summary>
    [Fact]
    public void RungeKutta4_Test()
    {
        float expectedAngle1 = 1.6588478f;
        float expectedAngle2 = -0.65069556f;
        float expectedAngularVelocity1 = -6.1928787f;
        float expectedAngularVelocity2 = 0.32441953f;

        Vector4 stateRungeKutta4 = _physics.RungeKutta4(_state);

        Assert.Equal(expectedAngle1, stateRungeKutta4.X, precision: 5);
        Assert.Equal(expectedAngle2, stateRungeKutta4.Y, precision: 5);
        Assert.Equal(expectedAngularVelocity1, stateRungeKutta4.Z, precision: 5);
        Assert.Equal(expectedAngularVelocity2, stateRungeKutta4.W, precision: 5);
    }

    /// <summary>
    /// Verifies that the Step method updates the physics state directly to the expected values.
    /// </summary>
    [Fact]
    public void Step_Test()
    {
        float expectedAngle1 = 1.6588478f;
        float expectedAngle2 = -0.65069556f;
        float expectedAngularVelocity1 = -6.1928787f;
        float expectedAngularVelocity2 = 0.32441953f;

        _physics.State = _state;

        _physics.Step();

        Assert.Equal(expectedAngle1, _physics.State.X, precision: 5); // Without physics. the state with 0 value for Z,W would be used.
        Assert.Equal(expectedAngle2, _physics.State.Y, precision: 5);
        Assert.Equal(expectedAngularVelocity1, _physics.State.Z, precision: 5);
        Assert.Equal(expectedAngularVelocity2, _physics.State.W, precision: 5);
    }

    /// <summary>
    /// Verifies that the PolarToCartesian method correctly converts a polar coordinate state to its Cartesian representation.
    /// </summary>
    [Fact]
    public void PolarToCartesian_Test()
    {
        float expectedX1 = 1.2942305f;
        float expectedY1 = 0.12234091f;
        float expectedX2 = 0.08225376f;
        float expectedY2 = -1.4686061f;

        Vector4 cartesianState = _physics.PolarToCartesian(_state);

        Assert.Equal(expectedX1, cartesianState.X, precision: 5);
        Assert.Equal(expectedY1, cartesianState.Y, precision: 5);
        Assert.Equal(expectedX2, cartesianState.Z, precision: 5);
        Assert.Equal(expectedY2, cartesianState.W, precision: 5);
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

        Vector4 positionState = _physics.GetPosition();

        Assert.Equal(expectedX1, positionState.X, precision: 5);
        Assert.Equal(expectedY1, positionState.Y, precision: 5);
        Assert.Equal(expectedX2, positionState.Z, precision: 5);
        Assert.Equal(expectedY2, positionState.W, precision: 5);
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

        Vector4 stateBefore = zeroPhysics.RungeKutta4(zeroPhysics.State);

        for (int i = 0; i < 100; i++)
        {
            zeroPhysics.Step();
        }

        Vector4 stateAfter = zeroPhysics.RungeKutta4(zeroPhysics.State);

        Assert.Equal(stateBefore.X, stateAfter.X, precision: 5);
        Assert.Equal(stateBefore.Y, stateAfter.Y, precision: 5);
        Assert.Equal(stateBefore.Z, stateAfter.Z, precision: 5);
        Assert.Equal(stateBefore.W, stateAfter.W, precision: 5);
    }

    /// <summary>
    /// Verifies that the calculated Cartesian positions match the expected values for various initial states.
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

        Vector4 position = posPhysics.PolarToCartesian(posPhysics.State);

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

        float initialNoDamp = TotalEnergy(noDampPhysics.State, noDampParams.Length1, noDampParams.Length2, noDampParams.Mass1, noDampParams.Mass2);
        float initialDamp = TotalEnergy(dampPhysics.State, dampParams.Length1, dampParams.Length2, dampParams.Mass1, dampParams.Mass2);

        for (int i = 0; i < 100; i++)
        {
            noDampPhysics.Step();
            dampPhysics.Step();
        }

        float finalNoDamp = TotalEnergy(noDampPhysics.State, noDampParams.Length1, noDampParams.Length2, noDampParams.Mass1, noDampParams.Mass2);
        float finalDamp = TotalEnergy(dampPhysics.State, dampParams.Length1, dampParams.Length2, dampParams.Mass1, dampParams.Mass2);

        Assert.Equal(initialNoDamp, finalNoDamp, precision: 2); // (RK4 not the best at keeping energy conservation -> Low precision)
        Assert.NotEqual(initialDamp, finalDamp, precision: 2);
    }

    #endregion
}
