using System;
using System.Numerics;

namespace double_pendulum.Services;

public class PendulumPhysics
{
	const float gravity = 9.81f;
	float stepSize = 0.01f;

    PendulumParameters parameters;

    public PendulumPhysics()
	{
    }

	Vector2 Initializtion()
	{
        float startingAngle = parameters.Angle1;
        float startingAngularVelocity = 0.0f;
        Vector2 initialVector = new Vector2(startingAngularVelocity, startingAngle);

        Vector2 euklidPostion = PolarToEuklid(initialVector);

        return euklidPostion;
    }

	Vector2 Step(Vector2 vectorOld)
	{
		Vector2 vectorNew = RK4(vectorOld);

		Vector2 euklidPosition = PolarToEuklid(vectorNew);

		return euklidPosition;
    }

	Vector2 DEQ(Vector2 vector)
	{
		float angle = vector.X;
		float angularVelocity = vector.Y;

		float angularAcceleration = (float)(-gravity / parameters.Length1 * Math.Sin(angle) - parameters.Damp * angularVelocity);

        Vector2 res = new Vector2(angularVelocity, angularAcceleration);

		return res;
	}

	Vector2 RK4(Vector2 vectorOld)
	{
        Vector2 k0 = DEQ(vectorOld);
        Vector2 k1 = DEQ(vectorOld + (stepSize * 0.5f) * k0);
        Vector2 k2 = DEQ(vectorOld + (stepSize * 0.5f) * k1);
        Vector2 k3 = DEQ(vectorOld + stepSize * k2);

		Vector2 vectorNew = vectorOld + stepSize / 6f * (k0 + 2f * k1 + 2f * k2 + k3);

		return vectorNew;
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
