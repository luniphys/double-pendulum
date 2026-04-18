using System.Numerics;

namespace double_pendulum.Services;

/// <summary>
/// Provides physics simulation for a double pendulum using numerical integration method Runge-Kutta of 4th order.
/// </summary>

public class PendulumPhysics
{
	const float gravity = 9.81f;
	float stepSize = 0.01f;

	Vector2 state;
	
    PendulumParameters parameters;

    public PendulumPhysics(PendulumParameters parameters)
	{
		this.parameters = parameters;
		state = Initialization();
    }

	Vector2 Initialization()
	{
        float startingAngle = parameters.Angle1;
        float startingAngularVelocity = 0.0f;
        Vector2 initialState = new Vector2(startingAngle, startingAngularVelocity);

        return initialState;
    }

	public void Step()
	{
		state = RK4(state);
    }

    public Vector2 GetPosition()
	{
		Vector2 euklidPosition = PolarToEuklid(state);

		return euklidPosition;
	}

	Vector2 DEQ(Vector2 vector)
	{
		float angle = vector.X;
		float angularVelocity = vector.Y;

		float angularAcceleration = (float)(-gravity / parameters.Length1 * Math.Sin(angle) - parameters.Damp * angularVelocity);

        Vector2 vectorDerivative = new Vector2(angularVelocity, angularAcceleration);

		return vectorDerivative;
	}

	Vector2 RK4(Vector2 stateOld)
	{
        Vector2 k0 = DEQ(stateOld);
        Vector2 k1 = DEQ(stateOld + (stepSize * 0.5f) * k0);
        Vector2 k2 = DEQ(stateOld + (stepSize * 0.5f) * k1);
        Vector2 k3 = DEQ(stateOld + stepSize * k2);

		Vector2 stateNew = stateOld + stepSize / 6f * (k0 + 2f * k1 + 2f * k2 + k3);

		return stateNew;
    }

	float X1(float angle1)
	{
		return (float)(parameters.Length1 * Math.Sin(angle1));
	}
    float Y1(float angle1)
    {
        return (float)(-parameters.Length1 * Math.Cos(angle1));
    }

	float X2(float angle1, float angle2)
	{
        return (float)(parameters.Length1 * Math.Sin(angle1) + parameters.Length2 + Math.Sin(angle2));
    }
    float Y2(float angle1, float angle2)
    {
        return (float)(-parameters.Length1 * Math.Cos(angle1) - parameters.Length2 + Math.Cos(angle2));
    }

	Vector2 PolarToEuklid(Vector2 vector)
	{
		float angle = vector.X;

		float xPosition = X1(angle);
		float yPosition = Y1(angle);

		Vector2 euklidPosition = new Vector2(xPosition, yPosition);

		return euklidPosition;
	}
}
