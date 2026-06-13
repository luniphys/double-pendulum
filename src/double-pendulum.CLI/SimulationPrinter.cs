using double_pendulum.Core;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;
using System.Text.Json;

namespace double_pendulum.CLI;

public class SimulationPrinter
{
    /// <summary>
    /// Runs a full physics simulation on all inital parameters in real time.
    /// <remarks>Possibility to set a fixed sim timespan or start & end by key press.</remarks>
    /// </summary>
    /// <param name="parameters"></param>
    public static void Run(PendulumParameters parameters)
    {
        PendulumPhysics physics = new PendulumPhysics(parameters);

        Dictionary<string, List<float>> positionData = new Dictionary<string, List<float>>();
        List<float> X1 = new List<float>();
        List<float> Y1 = new List<float>();
        List<float> X2 = new List<float>();
        List<float> Y2 = new List<float>();

        bool fixedTime = false;
        bool timePrompt = true;
        double timeSpan = 0.0;
        Console.WriteLine("Do you want to simulate for a fixed time, or start & end yourself by key press?\n");
        while (timePrompt)
        {
            Console.WriteLine("Enter time span: (Enter 0 if you want key press based start & stop.)");
            if (double.TryParse(Console.ReadLine(), out double value))
            {
                if (value != 0)
                {
                    fixedTime = true;
                    timeSpan = value;
                }
                timePrompt = false;
            }
        }

        if (fixedTime)
        {
            Console.WriteLine("\nPress any key to Start the simulation.\n");
        }
        else
        {
            Console.WriteLine("\nPress any key to Start & Stop the simulation.\n");
        }
        Console.WriteLine($"{"t",12}{"X1",12}{"Y1",12}{"X2",12}{"Y2", 12}");
        Console.ReadKey(intercept: true);

        Vector4 positions;
        float t = 0.0f;

        Stopwatch stopwatch = Stopwatch.StartNew();
        TimeSpan previousTime = stopwatch.Elapsed;
        double accumulator = 0.0;

        bool running = true;
        while (running)
        {
            TimeSpan currentTime = stopwatch.Elapsed;
            double deltaTime = (currentTime - previousTime).TotalSeconds;
            previousTime = currentTime;
            accumulator += deltaTime;

            while (accumulator >= physics.StepSize)
            {
                physics.Step();
                t += physics.StepSize;
                accumulator -= physics.StepSize;
            }

            positions = physics.GetPosition();
            Console.WriteLine($"{t, 12:F8}{positions.X, 12:F8}{positions.Y, 12:F8}{positions.Z, 12:F8}{positions.W, 12:F8}");

            X1.Add(positions.X);
            Y1.Add(positions.Y);
            X2.Add(positions.Z);
            Y2.Add(positions.W);

            Thread.Sleep(1);

            if (fixedTime)
            {
                if (t >= timeSpan)
                {
                    running = false;
                }
            }
            else
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(intercept: true);
                    running = false;
                }
            }
        }

        positionData.Add("X1", X1);
        positionData.Add("Y1", Y1);
        positionData.Add("X2", X2);
        positionData.Add("Y2", Y2);

        while (Console.KeyAvailable)
            Console.ReadKey(intercept: true);

        ExportData(positionData);
    }


    /// <summary>
    /// Asks user if the data should be exported as JSON to double-pendulum/output/simulation_***.json
    /// </summary>
    /// <param name="positionData">Dictionary with positions as separate lists</param>
    private static void ExportData(Dictionary<string, List<float>> positionData)
    {
        string answer;

        do
        {
            Console.WriteLine("\nWant to export data to JSON? (y/n)");

            answer = Console.ReadLine()!;
            answer = answer.ToLower();
        }
        while (answer != "y" && answer != "yes" && answer != "n" && answer != "no");

        if (answer != "y" && answer != "yes") { return; }


        string ROOT = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        string OUTPUT = Path.Combine(ROOT, "output");
        Directory.CreateDirectory(OUTPUT);

        string timestamp = DateTime.Now.ToString("dd-MM-yyyy__HH-mm-ss");
        string filePath = Path.Combine(OUTPUT, $"simulation_{timestamp}.json");

        try
        {
            string json = JsonSerializer.Serialize(positionData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Exported to: {filePath}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Export failed: {e.Message}");
        }
    }
}
