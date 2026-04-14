using System;

namespace double_pendulum.Services;
public class PendulumParameters
{
    public float Length1 { get; set; }
    public float Length2 { get; set; }
    public float Mass1 { get; set; }
    public float Mass2 { get; set; }
    public float StartingAngle_1 { get; set; }
    public float StartingAngle_2 { get; set; }
    public float Damp { get; set; }


    public PendulumParameters(float Length1, float Length2, float Mass1, float Mass2, float StartingAngle_1, float StartingAngle_2, float Damp)
    {
        this.Length1 = Length1;
        this.Length2 = Length2;
        this.Mass1 = Mass1;
        this.Mass2 = Mass2;
        this.StartingAngle_1 = StartingAngle_1;
        this.StartingAngle_2 = StartingAngle_2;
        this.Damp = Damp;
    }
}