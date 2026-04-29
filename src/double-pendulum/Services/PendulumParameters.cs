namespace double_pendulum.Services;

/// <summary>
/// Represents the set of physical and initial state parameters for the double pendulum, set by the user.
/// Values are read-only after construction.
/// </summary>

// Improvement: Use "record" instead of class for read-only classes.
public class PendulumParameters
{
    public float Length1 { get; }
    public float Length2 { get; }
    public float Mass1 { get; }
    public float Mass2 { get; }
    public float Angle1 { get; }
    public float Angle2 { get; }
    public float Damp { get; }

    private const float DegreesToRadians = (float)(MathF.PI / 180.0f);



    public PendulumParameters(float length1, float length2, float mass1, float mass2, float angle1, float angle2, float damp)
    {
        this.Length1 = length1;
        this.Length2 = length2;
        this.Mass1 = mass1;
        this.Mass2 = mass2;
        this.Angle1 = angle1 * DegreesToRadians; // Conversion degrees -> radiants
        this.Angle2 = angle2 * DegreesToRadians;
        this.Damp = damp;
    }
}