using double_pendulum.Model;

namespace double_pendulum.CLI;

public class Program
{
    public static void Main()
    {
        string again;

        do
        {
            PendulumParameters parameters = InputHandler.ReadParameters();
            SimulationPrinter.Run(parameters);

            do
            {
                Console.WriteLine("\n-------------------------------------------------------------------------------");
                Console.WriteLine("Run another simulation? (y/n)");
                again = (Console.ReadLine() ?? string.Empty).ToLower();

                if (again != "y" && again != "yes" && again != "n" && again != "no")
                {
                    Console.WriteLine("Invalid input!");
                }
            }
            while (again != "y" && again != "yes" && again != "n" && again != "no");
        }
        while (again == "y" || again == "yes");
    }
}
