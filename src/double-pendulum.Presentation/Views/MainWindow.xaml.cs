using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using double_pendulum.Model;
using double_pendulum.Presentation.Views.Rendering;
using double_pendulum.Presentation.Views.Controls;
using double_pendulum.Presentation.ViewModels;

namespace double_pendulum.Presentation
{
    public partial class MainWindow : Window

    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;



        public MainWindow()
        {
            InitializeComponent();
        }



        #region Event handlers

        /// <summary>
        /// Initializing the pendulum renderer and setting up the inital pendulum. Adds functionality that continiously
        /// ensures redrawing of pendulum once slider values are changed.
        /// </summary>
        private void PendulumCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _renderer = new PendulumRenderer(PendulumCanvas);

            _renderer.SetTrailLength((int)(TrailLength.QuantityValue * 100));

            DrawPreview();



            List<QuantitySlider> sliders = new List<QuantitySlider> { SliderL1, SliderL2, SliderM1, SliderM2, SliderA1, SliderA2, SliderD };
            DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(
                Views.Controls.QuantitySlider.QuantityValueProperty,
                typeof(Views.Controls.QuantitySlider)); // WPF helper that listens for changes on QuantityValueProperty of a QuantitySlider

            foreach (QuantitySlider slider in sliders)
            {
                descriptor.AddValueChanged(slider, (s, args) => { if (!_isRunning) DrawPreview(); });
            }

            descriptor.AddValueChanged(TrailLength, (s, args) => { _renderer.SetTrailLength((int)(TrailLength.QuantityValue * 100)); });
        }


        /// <summary>
        /// Handles the SizeChanged event of the canvas to update the pendulum when window size is changed.
        /// </summary>
        private void PendulumCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isRunning) { DrawPreview(); }
        }


        /// <summary>
        /// Handles clicking the Start button to initialize and start the pendulum simulation via Rendering.
        /// </summary>
        //private void StartButton_Click(object sender, RoutedEventArgs e)
        //{
        //    _isRunning = true;
            
        //    PendulumParameters parameters = BuildParameters();
        //    _pendulum = new PendulumPhysics(parameters);

        //    _stepAccumulator = 0.0;
        //    _lastRenderTime = TimeSpan.MinValue;

        //    CompositionTarget.Rendering += OnRendering;
        //}

        
        /// <summary>
        /// Fires at every frame (synced to GPU) to advance pendulum simulation and redraw its state.
        /// Also updates pendulums color depenging on ColorCheckBox.
        /// </summary>
        //private void OnRendering(object? sender, EventArgs e)
        //{
        //    var renderArgs = (RenderingEventArgs)e;

        //    if (_lastRenderTime == TimeSpan.MinValue) // .MinValue is special first frame value. Checking if its first frame -> Save time & skip.
        //    {
        //        _lastRenderTime = renderArgs.RenderingTime;
        //        return;
        //    }

        //    if (renderArgs.RenderingTime == _lastRenderTime) { return; } // Avoid double event firing per frame

        //    double elapsedMs = (renderArgs.RenderingTime - _lastRenderTime).TotalMilliseconds;
        //    _lastRenderTime = renderArgs.RenderingTime;

        //    _stepAccumulator += elapsedMs * SpeedSlider.QuantityValue * 0.65;

        //    while (_stepAccumulator >= 1.0)
        //    {
        //        _pendulum.Step();
        //        _stepAccumulator -= 1.0;
        //    }

        //    _renderer.Draw(_pendulum.GetPosition());

        //    UpdatePendulumColor();
        //}


        /// <summary>
        /// Handles Click event of Reset button by stopping the timer, resetting the UI to preview, and clearing the trail.
        /// </summary>
        //private void ResetButton_Click(object sender, RoutedEventArgs e)
        //{
        //    _isRunning = false;

        //    _renderer.EraseTrail();

        //    CompositionTarget.Rendering -= OnRendering;
        //    DrawPreview();
        //}


        /// <summary>
        /// Handles Checked and Unchecked events for color switch checkbox, updating the preview when the box is (un-)ticked
        /// </summary>
        private void ColorCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isRunning) { DrawPreview(); }
        }


        /// <summary>
        /// Normalizes Simulation Speed for SpeedSlider to 1.0x
        /// </summary>
        //private void SpeedReset_Click(object sender, RoutedEventArgs e)
        //{
        //    SpeedSlider.QuantityValue = 1.0;
        //}


        /// <summary>
        /// PreviewMouseDown updates any active TextBox bindings and clears focus when not clicking a TextBox
        /// </summary>
        /// <remarks>Ensures that pending changes are committed before focus is lost.</remarks>
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var topVisualElement = e.OriginalSource;

            if (topVisualElement is DependencyObject)
            {
                DependencyObject topVisualDO = (DependencyObject)topVisualElement;

                if (!IsChildOfType<TextBox>(topVisualDO))
                {
                    if (Keyboard.FocusedElement is TextBox)
                    {
                        TextBox focusedTextBox = (TextBox)Keyboard.FocusedElement;

                        BindingExpression binding = focusedTextBox.GetBindingExpression(TextBox.TextProperty);
                        binding?.UpdateSource();
                    }
                    Keyboard.ClearFocus();
                    FocusManager.SetFocusedElement(this, this);
                }
            }
        }

        #endregion



        #region Private (helper) methods

        /// <summary>
        /// Determines whether an element is a child of a specific type.
        /// </summary>
        /// <remarks>Travelling up the visual tree from the initial element and checking each parents' type.</remarks>
        private static bool IsChildOfType<T>(DependencyObject element) where T : DependencyObject
        {
            while (element != null)
            {
                if (element is T) { return true; }
                element = VisualTreeHelper.GetParent(element);
            }
            return false;
        }

        #endregion
    }
}
