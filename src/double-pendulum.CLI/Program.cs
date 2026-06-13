using double_pendulum.Core;

namespace double_pendulum.CLI;

public class Program
{
    public static void Main()
    {
        //PendulumParameters parameters = InputHandler.ReadParameters();
        PendulumParameters parameters = new PendulumParameters(1, 2, 25, 15, 170, 90, 1);
        SimulationPrinter.Run(parameters);
    }
}
