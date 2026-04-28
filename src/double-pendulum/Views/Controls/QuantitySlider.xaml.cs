using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace double_pendulum.Views.Controls
{
    public partial class QuantitySlider : UserControl
    {
        /// <summary>
        /// Color property of a QuantitiySlider's rail.
        /// </summary>
        public Brush SliderColor
        {
            get { return (Brush)GetValue(SliderColorProperty); }
            set { SetValue(SliderColorProperty, value); }
        }
        public static readonly DependencyProperty SliderColorProperty =
            DependencyProperty.Register(
                                        nameof(SliderColor),
                                        typeof(Brush),
                                        typeof(QuantitySlider),
                                        new PropertyMetadata(Brushes.Yellow));


        /// <summary>
        /// Physical quantity letter property
        /// </summary>
        public string Quantity
        {
            get { return (string)GetValue(QuantityProperty); }
            set { SetValue(QuantityProperty, value); }
        }
        public static readonly DependencyProperty QuantityProperty =
            DependencyProperty.Register(
                                        nameof(Quantity),
                                        typeof(string),
                                        typeof(QuantitySlider),
                                        new PropertyMetadata("Q"));


        /// <summary>
        /// Physical quantity unit letter property
        /// </summary>
        public string QuantityUnit
        {
            get { return (string)GetValue(QuantityUnitProperty); }
            set { SetValue(QuantityUnitProperty, value); }
        }
        public static readonly DependencyProperty QuantityUnitProperty =
            DependencyProperty.Register(
                                        nameof(QuantityUnit),
                                        typeof(string),
                                        typeof(QuantitySlider),
                                        new PropertyMetadata(""));


        /// <summary>
        /// Property for minimal value of a QuantitySlider
        /// </summary>
        public double MinSliderValue
        {
            get { return (double)GetValue(MinSliderValueProperty); }
            set { SetValue(MinSliderValueProperty, value); }
        }
        public static readonly DependencyProperty MinSliderValueProperty =
            DependencyProperty.Register(
                                        nameof(MinSliderValue),
                                        typeof(double),
                                        typeof(QuantitySlider),
                                        new PropertyMetadata(0.0));


        /// <summary>
        /// Property for maximal value of a QuantitySlider
        /// </summary>
        public double MaxSliderValue
        {
            get { return (double)GetValue(MaxSliderValueProperty); }
            set { SetValue(MaxSliderValueProperty, value); }
        }
        public static readonly DependencyProperty MaxSliderValueProperty =
            DependencyProperty.Register(
                                        nameof(MaxSliderValue),
                                        typeof(double),
                                        typeof(QuantitySlider),
                                        new PropertyMetadata(100.0));


        /// <summary>
        /// Property of the tick frequency the thumb snaps to.
        /// </summary>
        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register(
                                        nameof(TickFrequency),
                                        typeof(double),
                                        typeof(QuantitySlider),
                                        new PropertyMetadata(1.0));


        /// <summary>
        /// Actual quantity value of a QuantitySlider
        /// </summary>
        public double QuantityValue
        {
            get { return (double)GetValue(QuantityValueProperty); }
            set { SetValue(QuantityValueProperty, value); }
        }
        public static readonly DependencyProperty QuantityValueProperty =
            DependencyProperty.Register(
                                        nameof(QuantityValue),
                                        typeof(double),
                                        typeof(QuantitySlider),
                                        new FrameworkPropertyMetadata(
                                                                      0.0,
                                                                      FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                      null,
                                                                      CoerceQuantityValue));


        /// <summary>
        /// StringFormat of Quantity values' display
        /// </summary>
        /// <remarks>Controls how man decimal places are shown for a value.</remarks>
        public string ValueStringFormat
        {
            get { return (string)GetValue(ValueStringFormatProperty); }
            set { SetValue(ValueStringFormatProperty, value); }
        }
        public static readonly DependencyProperty ValueStringFormatProperty =
            DependencyProperty.Register(
                                        nameof(ValueStringFormat),
                                        typeof(string),
                                        typeof(QuantitySlider),
                                        new PropertyMetadata(
                                                             "F0",
                                                             OnValueStringFormatChanged));



        public QuantitySlider()
        {
            InitializeComponent();
        }



        /// <summary>
        /// Eventhandler ensures, that the value falls within the minimum and maximum slider limits.
        /// </summary>
        /// <remarks>Called whenever quantity value is changed, but before it is stored.</remarks>
        /// <returns>
        /// Returns minimum if value is less than the minimum and maximum if greater than maximum. Else it returns the original value if it's within range.
        /// </returns>
        private static object CoerceQuantityValue(DependencyObject d, object baseValue)
        {
            QuantitySlider slider = (QuantitySlider)d;
            double value = (double)baseValue;

            double min = slider.MinSliderValue;
            double max = slider.MaxSliderValue;

            if (double.IsNaN(value)) { return min; }

            if (value < min) { return min; }
            if (value > max) { return max; }

            return value;
        }


        /// <summary>
        /// Handles changes regarding the ValueStringFormat property and updates the binding for the value display accordingly.
        /// </summary>
        /// <remarks>Targets the StringFormat of the QuantitySlider as source in a new binding and sets this new binding for the QuantityTextBox</remarks>
        private static void OnValueStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuantitySlider slider = (QuantitySlider)d;
            Binding binding = new Binding(nameof(QuantityValue))
            {
                Source = slider,
                StringFormat = (string)e.NewValue,
                Mode = BindingMode.TwoWay
            };
            slider.ValueTextBox.SetBinding(TextBox.TextProperty, binding);
        }


        /// <summary>
        /// Handles key press events for the QuantityValue TextBox to commit or revert changes based on the pressed key.
        /// </summary>
        /// <remarks>Pressing Enter commits, while pressing Escape reverts the TextBox text to its original value.
        /// After that, focus is moved to the associated slider control. If TextBox is empty, focus stays after pressing Enter.</remarks>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Escape) { return; }

            TextBox textBox = (TextBox)sender;
            BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);

            if (e.Key == Key.Enter && string.IsNullOrWhiteSpace(textBox.Text)) { return; }

            if (e.Key == Key.Enter)
            {
                binding?.UpdateSource();
            }
            else
            {
                binding?.UpdateTarget();
            }
            QSlider.Focus();
        }


        /// <summary>
        /// Makes sure that when TextBox is not focused, clicking it ensures all of the existing text to be selected. 
        /// </summary>
        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!textBox.IsKeyboardFocused)
            {
                textBox.SelectAll();
                textBox.Focus();
                e.Handled = true;
            }
        }


        private static readonly Regex NumericRegex = new(@"^-?\d*\.?\d*$", RegexOptions.Compiled); 
        /// <summary>
        /// Handler prevents non numerical input in ValueTextBox.
        /// </summary>
        private void ValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string resultingText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            e.Handled = !NumericRegex.IsMatch(resultingText);
        }


        /// <summary>
        /// Handler prevents whitespaces as input in ValueTextBox.
        /// </summary>
        private void ValueTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}

//  Property Guide:

//  public int Name
//  {
//      get { return (int)GetValue(NameProperty); }                         Getters/Setters are only thin wrappers
//      set { SetValue(NameProperty, value); }
//  }
//  public static readonly DependencyProperty NameProperty =                DependencyProperty is entire WPF internal storage/system table with key/token access,
//  DependencyProperty.Register(                                            that provides extra integrated functionality.
//                              nameof(Name),                               Name of property
//                              typeof(int),                                Value type
//                              typeof(QuantitySlider),                     Owner type
//                              new PropertyMetadata(
//                                                   Default value,
//                                                   Change callback,       Can execute commands after value is changed by: (d, e) => { code; } or Method(),
//                                                   Coerce callback)       Can execute commands after value is changed by: - "" -
//                              );