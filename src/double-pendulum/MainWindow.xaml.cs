using double_pendulum.Services;
using double_pendulum.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Numerics;
using System.Windows.Threading;

namespace double_pendulum
{
    public partial class MainWindow : Window

    {
        PendulumPhysics pendulum;
        PendulumRenderer renderer;
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Check if clicked element is not a textbox, update values and clear focus
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source && !IsChildOfType<TextBox>(source)) // e.OriginalSource is topmost visual element clicked
            {
                if (Keyboard.FocusedElement is TextBox focusedTextBox)
                {
                    var binding = focusedTextBox.GetBindingExpression(TextBox.TextProperty);
                    Console.WriteLine(TextBox.TextProperty);
                    binding?.UpdateSource();
                }
                Keyboard.ClearFocus();
                FocusManager.SetFocusedElement(this, this);
            }
        }

        // Walk up visual tree of clicked element to check if it (or any parent) is of type T
        private static bool IsChildOfType<T>(DependencyObject element) where T : DependencyObject
        {
            while (element != null)
            {
                if (element is T)
                    return true;
                element = VisualTreeHelper.GetParent(element);
            }
            return false;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            PendulumParameters parameters = new PendulumParameters(
                (float)SliderL1.QuantityValue,
                (float)SliderL2.QuantityValue,
                (float)SliderM1.QuantityValue,
                (float)SliderM2.QuantityValue,
                (float)SliderA1.QuantityValue,
                (float)SliderA2.QuantityValue,
                (float)SliderD.QuantityValue
                );

            pendulum = new PendulumPhysics(parameters);
            renderer = new PendulumRenderer(PendulumCanvas);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            pendulum.Step();
            renderer.Draw(pendulum.GetPosition(), new Vector2(0, 0));
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            timer?.Stop();
            renderer.ClearCanvas();

            PendulumParameters parameters = new PendulumParameters(
                (float)SliderL1.QuantityValue,
                (float)SliderL2.QuantityValue,
                (float)SliderM1.QuantityValue,
                (float)SliderM2.QuantityValue,
                (float)SliderA1.QuantityValue,
                (float)SliderA2.QuantityValue,
                (float)SliderD.QuantityValue
                );

            pendulum = new PendulumPhysics(parameters);
            renderer = new PendulumRenderer(PendulumCanvas);

            renderer.Draw(pendulum.GetPosition(), new Vector2(0, 0));
        }
    }
}