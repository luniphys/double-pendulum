using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace double_pendulum.Services;

/// <summary>
/// Provides physics simulation for a double pendulum using numerical integration method Runge-Kutta of 4th order.
/// </summary>

public class PendulumPhysics
{
	const float gravity = 9.81f;
	float stepSize = 0.01f;

	public Vector4 state;
	
    PendulumParameters parameters;

    public PendulumPhysics(PendulumParameters parameters)
	{
		this.parameters = parameters;
		state = Initialization();
    }

	Vector4 Initialization()
	{
        float startingAngle1 = parameters.Angle1;
        float startingAngle2 = parameters.Angle2;
        float startingAngularVelocity1 = 0.0f;
        float startingAngularVelocity2 = 0.0f;
        Vector4 initialState = new Vector4(startingAngle1, startingAngle2, startingAngularVelocity1, startingAngularVelocity2);

        return initialState;
    }

	public Vector4 DEQ(Vector4 vector)
	{
		float angle1 = vector.X;
		float angle2 = vector.Y;
		float angularVelocity1 = vector.Z;
		float angularVelocity2 = vector.W;

		Matrix<double> Mat = DenseMatrix.OfArray(new double[,] { { (parameters.Mass1 + parameters.Mass2) * parameters.Length1, parameters.Mass2 * parameters.Length2 * Math.Cos(angle1 - angle2) },
																 { parameters.Mass2 * parameters.Length1 * Math.Cos(angle1 - angle2), parameters.Mass2 * parameters.Length2 } });

		MathNet.Numerics.LinearAlgebra.Vector<double> Vec = DenseVector.OfArray(new double[] {  -parameters.Mass2 * parameters.Length2 * Math.Pow(angularVelocity2,2) * Math.Sin(angle1 - angle2) - (parameters.Mass1 + parameters.Mass2) * gravity * Math.Sin(angle1) - parameters.Damp * angularVelocity1,
																								parameters.Mass2 * parameters.Length1 * Math.Pow(angularVelocity1,2) * Math.Sin(angle1 - angle2) - parameters.Mass2 * gravity * Math.Sin(angle2) - parameters.Damp * angularVelocity2});

		Matrix<double> MatInverse = Mat.Inverse();
		MathNet.Numerics.LinearAlgebra.Vector<double> ResultVec = MatInverse * Vec;

		float angularAcceleration1 = (float)ResultVec[0];
        float angularAcceleration2 = (float)ResultVec[1];

        Vector4 vectorDerivative = new Vector4(angularVelocity1, angularVelocity2, angularAcceleration1, angularAcceleration2);

		return vectorDerivative;
	}

	public Vector4 RK4(Vector4 stateOld)
	{
        Vector4 k0 = DEQ(stateOld);
        Vector4 k1 = DEQ(stateOld + (stepSize * 0.5f) * k0);
        Vector4 k2 = DEQ(stateOld + (stepSize * 0.5f) * k1);
        Vector4 k3 = DEQ(stateOld + stepSize * k2);

		Vector4 stateNew = stateOld + stepSize / 6f * (k0 + 2f * k1 + 2f * k2 + k3);

		return stateNew;
    }

    public void Step()
    {
        state = RK4(state);
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
        return (float)(parameters.Length1 * Math.Sin(angle1) + parameters.Length2 * Math.Sin(angle2));
    }
    float Y2(float angle1, float angle2)
    {
        return (float)(-parameters.Length1 * Math.Cos(angle1) - parameters.Length2 * Math.Cos(angle2));
    }

	public Vector4 PolarToEuklid(Vector4 vector)
	{
		float angle1 = vector.X;
        float angle2 = vector.Y;

        float x1Position = X1(angle1);
		float y1Position = Y1(angle1);

        float x2Position = X2(angle1, angle2);
        float y2Position = Y2(angle1, angle2);

        Vector4 euklidPosition = new Vector4(x1Position, y1Position, x2Position, y2Position);

		return euklidPosition;
	}

    public Vector4 GetPosition()
    {
        Vector4 euklidPosition = PolarToEuklid(state);

        return euklidPosition;
    }
}
