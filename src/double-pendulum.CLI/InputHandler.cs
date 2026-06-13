using double_pendulum.Core;

namespace double_pendulum.CLI;

public class InputHandler
{
    /// <summary>
    /// Prompts user to input parameter values via console.
    /// </summary>
    /// <returns>Initial parameter values as PendulumParameters object.</returns>
    public static PendulumParameters ReadParameters()
    {
        float length1 = ReadValue("Enter length of pendulum 1 in [m]:", "[1 nm, 1 km]", 0.000000001f, 1000.0f);
        float length2 = ReadValue("Enter length of pendulum 2 in [m]:", "[1 nm, 1 km]", 0.000000001f, 1000.0f);
        float mass1 = ReadValue("Enter mass of pendulum 1 in [kg]:", "[1 μg, 1000 kg]", 0.000000001f, 1000.0f);
        float mass2 = ReadValue("Enter mass of pendulum 2 in [kg]:", " [1 μg, 1000 kg]", 0.000000001f, 1000.0f);
        float angle1 = ReadValue("Enter starting angle of pendulum 1 in [°]:", "[-180°, 180°]", -180.0f, 180.0f);
        float angle2 = ReadValue("Enter starting angle of pendulum 2 in [°]:", "[-180°, 180°]", -180.0f, 180.0f);
        float damp = ReadValue("Enter damping coefficient:", "[0, 1000]", 0, 1000);

        return new PendulumParameters(length1, length2, mass1, mass2, angle1, angle2, damp);
    }


    /// <summary>
    /// Casts user entered strings to usable float values and checks range limits, as well as invalid inputs.
    /// </summary>
    /// <param name="prompt">Console prompt text.</param>
    /// <param name="range">Allowed range of values for user info.</param>
    /// <param name="minValue">Minimum allowed value.</param>
    /// <param name="maxValue">Maximum allowed value.</param>
    /// <returns></returns>
    private static float ReadValue(string prompt, string range, float minValue, float maxValue)
    {
        while (true)
        {
            Console.WriteLine(prompt);

            if (float.TryParse(Console.ReadLine(), out float value))
            {
                if (value >= minValue && value <= maxValue)
                {
                    Console.WriteLine();
                    return value;
                }
                else
                {
                    Console.WriteLine($"Stay within range! {range}\n");
                }
            }
            else
            {
                Console.WriteLine("Invalid input! Only (decimal) numbers allowed.\n");
            }
        }
    }
}
