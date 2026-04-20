using double_pendulum.Services;
using double_pendulum.Views;
using double_pendulum.Views.Controls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace double_pendulum
{
    public partial class MainWindow : Window

    {
        PendulumPhysics pendulum;
        PendulumRenderer renderer;
        DispatcherTimer timer;

        bool isRunnning = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private PendulumParameters BuildParameters()
        {
            return new PendulumParameters(
                (float)SliderL1.QuantityValue,
                (float)SliderL2.QuantityValue,
                (float)SliderM1.QuantityValue,
                (float)SliderM2.QuantityValue,
                (float)SliderA1.QuantityValue,
                (float)SliderA2.QuantityValue,
                (float)SliderD.QuantityValue);
        }
        
        private void PendulumCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            renderer = new PendulumRenderer(PendulumCanvas);
            renderer.SetTrailLength((int)(TrailDuration.QuantityValue * 100));

            List<QuantitySlider> sliders = new List<QuantitySlider> { SliderL1, SliderL2, SliderM1, SliderM2, SliderA1, SliderA2, SliderD };
            var descriptor = DependencyPropertyDescriptor.FromProperty(
                Views.Controls.QuantitySlider.QuantityValueProperty,
                typeof(Views.Controls.QuantitySlider));

            foreach (QuantitySlider slider in sliders)
            {
                descriptor.AddValueChanged(slider, (s, args) => { if (!isRunnning) DrawPreview(); }); // use lambda function for inline method
            }

            descriptor.AddValueChanged(TrailDuration, (s, args) => { renderer.SetTrailLength((int)(TrailDuration.QuantityValue * 100)); });

            UpdateButtonStates();
            DrawPreview();
        }

        // Draw previewPendulum if window size is changed
        private void PendulumCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!isRunnning) { DrawPreview(); }
        }
        
        // Draws a preview of the double pendulum with initial slider values
        private void DrawPreview()
        {
            if (renderer == null) { return; }

            if (ColorCheckBox.IsChecked == true) { renderer.ChangeColor(0, 0, 255, 0, 0, 255); }
            else { renderer.ChangeColor(255, 255, 255, 255, 255, 255); }

            renderer.UpdateRadii(SliderM1.QuantityValue, SliderM2.QuantityValue);

            PendulumParameters parameters = BuildParameters();
            PendulumPhysics previewPendulum = new PendulumPhysics(parameters);
            renderer.Draw(previewPendulum.GetPosition());
        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            isRunnning = true;
            UpdateButtonStates();
            pendulum = new PendulumPhysics(BuildParameters());

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            pendulum.Step();
            renderer.Draw(pendulum.GetPosition());
            
            if (ColorCheckBox.IsChecked ==  true)
            {
                const double maxAngularVelocity = 10.0;

                double angularVelocity1 = Math.Min(Math.Abs(pendulum.state.Z), maxAngularVelocity);
                byte red1 = (byte)(angularVelocity1 / maxAngularVelocity * 255);

                double angularVelocity2 = Math.Min(Math.Abs(pendulum.state.W), maxAngularVelocity);
                byte red2 = (byte)(angularVelocity2 / maxAngularVelocity * 255);

                renderer.ChangeColor(red1, 0, (byte)(255 - red1), red2, 0, (byte)(255 - red2));
            }
            else
            {
                renderer.ChangeColor(255, 255, 255, 255, 255, 255);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            isRunnning = false;
            UpdateButtonStates();

            timer?.Stop();
            DrawPreview();

            renderer.EraseTrail();
        }


        // Switches button states of exclusive button group: Start, Reset
        private void UpdateButtonStates()
        {
            StartButton.IsEnabled = !isRunnning;
            ResetButton.IsEnabled = isRunnning;
        }

        // Makes color changes while pendulum at rest possible
        private void ColorCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (!isRunnning) { DrawPreview(); }
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
    }
}