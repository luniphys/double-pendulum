namespace double_pendulum.Services;

/// <summary>
/// Represents the set of physical and initial state parameters for the double pendulum, set by the user.
/// Values are read-only after construction.
/// </summary>

public class PendulumParameters
{
    public float Length1 { get; }
    public float Length2 { get; }
    public float Mass1 { get; }
    public float Mass2 { get; }
    public float Angle1 { get; }
    public float Angle2 { get; }
    public float Damp { get; }

    public PendulumParameters(float Length1, float Length2, float Mass1, float Mass2, float Angle1, float Angle2, float Damp)
    {
        this.Length1 = Length1;
        this.Length2 = Length2;
        this.Mass1 = Mass1;
        this.Mass2 = Mass2;
        this.Angle1 = Angle1 * (float)Math.PI / 180.0f; // Conversion degrees -> radiants
        this.Angle2 = Angle2 * (float)Math.PI / 180.0f;
        this.Damp = Damp;
    }
}