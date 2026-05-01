using System.Numerics;

namespace double_pendulum.Services;

/// <summary>
/// Provides physics simulation for a double pendulum using numerical integration method Runge-Kutta of 4th order.
/// </summary>
public class PendulumPhysics
{
	private const float Gravity = 9.81f;

    private const float DefaultStepSize = 0.001f;
    public float StepSize { get; set; } = DefaultStepSize;

    private readonly PendulumParameters _parameters;

    

    public PendulumPhysics(PendulumParameters parameters)
	{
		_parameters = parameters;

        float startingAngle1 = parameters.Angle1;
        float startingAngle2 = parameters.Angle2;
        float startingAngularVelocity1 = 0.0f;
        float startingAngularVelocity2 = 0.0f;

        State = new Vector4(startingAngle1, startingAngle2, startingAngularVelocity1, startingAngularVelocity2);
    }



    /// <summary>
    /// Current simulation state: (θ₁, θ₂, ω₁, ω₂).
    /// </summary>
    public Vector4 State { get; set; }



    #region Public methods

    /// <summary>
    /// Advancing the state vector for one time step.
    /// </summary>
    /// <remarks>Call RungeKutta4 method to update the current state by one integration step.</remarks>
    public void Step()
    {
        State = RungeKutta4(State);
    }
    // Shorter version: public void Step() => State = RungeKutta4(State);


    /// <summary>
    /// Gets the current position for both pendulums in Cartesian coordinates.
    /// </summary>
    /// <returns>A <see cref="Vector4"/> structure containing the position in Cartesian coordinates.</returns>
    public Vector4 GetPosition()
    {
        Vector4 cartesianPosition = PolarToCartesian(State);

        return cartesianPosition;
    }
    // Shorter version: public Vector4 GetPosition() => PolarToCartesian(State);

    #endregion



    #region Private (helper) methods
    // Only public due to PendulumPhysicsTests.cs!

    /// <summary>
    /// Calculates the time derivative of a state vector.
    /// </summary>
    /// <param name="vector">A state vector of the form (angle1, angle2, angularVelocity1, angularVelocity2).</param>
    /// <returns>The derivate of the vector in the form (angularVelocity1, angularVelocity2, angularAcceleration1, angularAcceleration2)</returns>
    public Vector4 Derivative(Vector4 vector)
	{
        float length1 = _parameters.Length1;
        float length2 = _parameters.Length2;
        float mass1 = _parameters.Mass1;
        float mass2 = _parameters.Mass2;
        float damp = _parameters.Damp;

        float angle1 = vector.X;
		float angle2 = vector.Y;
		float angularVelocity1 = vector.Z;
		float angularVelocity2 = vector.W;

        float cosDelta = MathF.Cos(angle1 - angle2);
        float sinDelta = MathF.Sin(angle1 - angle2);

        // 2x2 matrix for equation of motion
        float mX1 = (mass1 + mass2) * length1;
        float mX2 = mass2 * length2 * cosDelta;
        float mY1 = mass2 * length1 * cosDelta;
        float mY2 = mass2 * length2;

        // vector for equation of motion
        float vX = -mass2 * length2 * MathF.Pow(angularVelocity2, 2) * sinDelta - (mass1 + mass2) * Gravity * MathF.Sin(angle1) - damp * angularVelocity1;
        float vY = mass2 * length1 * MathF.Pow(angularVelocity1, 2) * sinDelta - mass2 * Gravity * MathF.Sin(angle2) - damp * angularVelocity2;

        // Inverse of matrix: (1/det) * [[mY2, -mX2], [-mY1, MX1]]
        float det = mX1 * mY2 - mY1 * mX2;
        float angularAcceleration1 = (float)((mY2 * vX - mX2 * vY) / det);
        float angularAcceleration2 = (float)((-mY1 * vX + mX1 * vY) / det);

        Vector4 vectorDerivative = new Vector4(angularVelocity1, angularVelocity2, angularAcceleration1, angularAcceleration2);

		return vectorDerivative;
	}

    /// <summary>
    /// Using fourth-order Runge-Kutta (RK4) scheme to advance the state for one time step.
    /// </summary>
    /// <param name="stateOld">The current state vector to be advanced.</param>
    /// <returns>The advanced state vector.</returns>
	public Vector4 RungeKutta4(Vector4 stateOld)
	{
        Vector4 k0 = Derivative(stateOld);
        Vector4 k1 = Derivative(stateOld + (StepSize * 0.5f) * k0);
        Vector4 k2 = Derivative(stateOld + (StepSize * 0.5f) * k1);
        Vector4 k3 = Derivative(stateOld + StepSize * k2);

		Vector4 stateNew = stateOld + StepSize / 6f * (k0 + 2f * k1 + 2f * k2 + k3);

		return stateNew;
    }


    /// <summary>
    /// Converts polar vector to Cartesian vector
    /// system.
    /// </summary>
    /// <param name="vector">The polar vector comes in the form of (angle1, angle2, angularVelocity1,angularVelocity2) in radians (per second).</param>
    /// <returns>A Vector4 with Cartesian coordinates of the endpoints: (x1, y1, x2, y2), where (x1, y1) is the
    /// mass point of the first pendulum and (x2, y2) the one of the second.</returns>
    public Vector4 PolarToCartesian(Vector4 vector)
    {
        float angle1 = vector.X;
        float angle2 = vector.Y;

        float x1Position = _parameters.Length1 * MathF.Sin(angle1);
        float y1Position = -_parameters.Length1 * MathF.Cos(angle1);

        float x2Position = x1Position + _parameters.Length2 * MathF.Sin(angle2);
        float y2Position = y1Position - _parameters.Length2 * MathF.Cos(angle2);

        Vector4 cartesianPosition = new Vector4(x1Position, y1Position, x2Position, y2Position);

        return cartesianPosition;
    }

    #endregion
}
