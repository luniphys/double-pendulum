using double_pendulum.Services;
using double_pendulum.Views;
using double_pendulum.Views.Controls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace double_pendulum
{
    public partial class MainWindow : Window

    {
        PendulumPhysics pendulum = null!;
        PendulumRenderer renderer = null!;
        DispatcherTimer timer = null!;

        bool isRunnning = false;


        public MainWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Create new instance of the PendulumParameters class using the values from the sliders.
        /// </summary>
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
        

        /// <summary>
        /// Initializing the pendulum renderer and setting up the inital pendulum. Adds functionality that continiously
        /// ensures redrawing of pendulum once slider values are changed.
        /// </summary>
        private void PendulumCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            renderer = new PendulumRenderer(PendulumCanvas);

            renderer.SetTrailLength((int)(TrailLength.QuantityValue * 100));

            DrawPreview();


            List<QuantitySlider> sliders = new List<QuantitySlider> { SliderL1, SliderL2, SliderM1, SliderM2, SliderA1, SliderA2, SliderD };
            DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(
                Views.Controls.QuantitySlider.QuantityValueProperty,
                typeof(Views.Controls.QuantitySlider)); // WPF helper that listens for changes on QuantityValueProperty of a QuantitySlider

            foreach (QuantitySlider slider in sliders)
            {
                descriptor.AddValueChanged(slider, (s, args) => { if (!isRunnning) DrawPreview(); });
            }

            descriptor.AddValueChanged(TrailLength, (s, args) => { renderer.SetTrailLength((int)(TrailLength.QuantityValue * 100)); });
        }


        /// <summary>
        /// Handles the SizeChanged event of the canvas to update the pendulum when window size is changed.
        /// </summary>
        private void PendulumCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!isRunnning) { DrawPreview(); }
        }


        /// <summary>
        /// Draws a preview of the double pendulum with the initial slider values
        /// </summary>
        /// <remarks>Ensures blue color (zero velocity) or white depending on ColorCheckBox. Sets radii
        /// corresponding to pendulums masses.</remarks>
        private void DrawPreview()
        {
            if (renderer == null) { return; }

            renderer.EraseTrail();

            if (ColorCheckBox.IsChecked == true) { renderer.ChangeColor(0, 0, 255, 0, 0, 255); }
            else { renderer.ChangeColor(255, 255, 255, 255, 255, 255); }

            renderer.UpdateRadii(SliderM1.QuantityValue, SliderM2.QuantityValue);

            PendulumParameters parameters = BuildParameters();
            PendulumPhysics previewPendulum = new PendulumPhysics(parameters);
            renderer.Draw(previewPendulum.GetPosition());
        }


        /// <summary>
        /// Handles clicking the Start button to initialize and start the pendulum simulation via DispatchTimer
        /// that calls Timer_Tick every 1 ms.
        /// </summary>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            isRunnning = true;
            UpdateButtonStates();

            PendulumParameters parameters = BuildParameters();
            pendulum = new PendulumPhysics(parameters);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        /// <summary>
        /// Timer tick event to advance pendulum simulation and redraw its state.
        /// </summary>
        /// <remarks>If the ColorCheckBox is enabled, the pendulum's colors will be based on
        /// their angular velocities. Otherwise, the pendulums are rendered in white.</remarks>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            pendulum.Step();
            renderer.Draw(pendulum.GetPosition());
            
            if (ColorCheckBox.IsChecked == true)
            {
                const double maxAngularVelocity = 10.0;

                double angularVelocity1 = Math.Min(Math.Abs(pendulum.State.Z), maxAngularVelocity);
                byte red1 = (byte)(angularVelocity1 / maxAngularVelocity * 255);

                double angularVelocity2 = Math.Min(Math.Abs(pendulum.State.W), maxAngularVelocity);
                byte red2 = (byte)(angularVelocity2 / maxAngularVelocity * 255);

                renderer.ChangeColor(red1, 0, (byte)(255 - red1), red2, 0, (byte)(255 - red2));
            }
            else
            {
                renderer.ChangeColor(255, 255, 255, 255, 255, 255);
            }
        }


        /// <summary>
        /// Handles Click event of Reset button by stopping the timer, resetting the UI to preview, and clearing the trail.
        /// </summary>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            isRunnning = false;
            UpdateButtonStates();

            timer.Stop();
            DrawPreview();

            renderer.EraseTrail();
        }


        /// <summary>
        /// Switches button states of exclusive button group: Start, Reset
        /// </summary>
        private void UpdateButtonStates()
        {
            StartButton.IsEnabled = !isRunnning;
            ResetButton.IsEnabled = isRunnning;
        }


        /// <summary>
        /// Handles Checked and Unchecked events for color switch checkbox, updating the preview when the box is (un-)ticked
        /// </summary>
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
                    BindingExpression binding = focusedTextBox.GetBindingExpression(TextBox.TextProperty);
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

// TODO: = null! on top?
// TODO: Install speed slider?
// TODO: professional code?