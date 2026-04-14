using System;
using System.Numerics;

namespace double_pendulum.Services;

public class PendulumPhysics
{
	const float g = 9.81f;
	float h = 0.01f;

    PendulumParameters parameters;

    public PendulumPhysics()
	{
    }

	void Step()
	{

    }

	Vector2 f(Vector2 y)
	{
		float theta = y.X;
		float w = y.Y;

		float w_dot = (float)(-g / parameters.Length1 * Math.Sin(theta) - parameters.Damp * w);

        Vector2 res = new Vector2(w, w_dot);

		return res;
	}

	Vector2 RK4(Vector2 y)
	{
        Vector2 k0 = f(y);
        Vector2 k1 = f(y + (h * 0.5f) * k0);
        Vector2 k2 = f(y + (h * 0.5f) * k1);
        Vector2 k3 = f(y + h * k2);

		return y + h / 6f * (k0 + 2f * k1 + 2f * k2 + k3);
    }

	float X1(float theta1)
	{
		return (float)(parameters.Length1 * Math.Sin(theta1));
	}
    float Y1(float theta1)
    {
        return (float)(-parameters.Length1 * Math.Cos(theta1));
    }
	float X2(float theta1, float theta2)
	{
        return (float)(parameters.Length1 * Math.Sin(theta1) + parameters.Length2 + Math.Sin(theta2));
    }
    float Y2(float theta1, float theta2)
    {
        return (float)(-parameters.Length1 * Math.Cos(theta1) - parameters.Length2 + Math.Cos(theta2));
    }
}
