using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;

namespace double_pendulum.Views.Controls
{
    public partial class QuantitySlider : UserControl // partial makes class available for and xaml this cs file. UserControl provides a lot of UI dependent code functionality
    {
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
                new PropertyMetadata(Brushes.Yellow)); // default value: yellow


        public string Quantity
        {
            get { return (string)GetValue(QuantityProperty); }
            set { SetValue(QuantityProperty, value); }
        }
        public static readonly DependencyProperty QuantityProperty =
            DependencyProperty.Register(
                nameof(Quantity),
                typeof(string),
                typeof(QuantitySlider));
       

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


        public string QuantityUnit
        {
            get { return (string)GetValue(QuantityUnitProperty); }
            set { SetValue(QuantityUnitProperty, value); }
        }
        public static readonly DependencyProperty QuantityUnitProperty =
            DependencyProperty.Register(nameof(QuantityUnit),
                typeof(string),
                typeof(QuantitySlider));


        public double MinSliderValue
        {
            get { return (double)GetValue(MinSliderValueProperty); }
            set { SetValue(MinSliderValueProperty, value); }
        }
        public static readonly DependencyProperty MinSliderValueProperty =
            DependencyProperty.Register(
                nameof(MinSliderValue),
                typeof(double),
                typeof(QuantitySlider));


        public double MaxSliderValue
        {
            get { return (double)GetValue(MaxSliderValueProperty); }
            set { SetValue(MaxSliderValueProperty, value); }
        }
        public static readonly DependencyProperty MaxSliderValueProperty =
            DependencyProperty.Register(
                nameof(MaxSliderValue),
                typeof(double),
                typeof(QuantitySlider));


        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register(
                nameof(TickFrequency),
                typeof(double),
                typeof(QuantitySlider));



        public QuantitySlider()
        {
            InitializeComponent();
        }

        private static object CoerceQuantityValue(DependencyObject d, object baseValue)
        {
            QuantitySlider control = (QuantitySlider)d;
            double value = (double)baseValue;
            double min = control.MinSliderValue;
            double max = control.MaxSliderValue;

            if (min > max)
            {
                double tmp = min;
                min = max;
                max = tmp;
            }
            if (value < min) { return min; }
            if (value > max) { return max; }
            return value;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) // object is generic type for all UI elements "sender" (here: TextBox)
        {
            TextBox textBox = (TextBox)sender; // Casting object->TextBox
            BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);

            if (e.Key == Key.Enter)
            {
                binding?.UpdateSource(); // pushes the typed value to the binding source
                Sliding.Focus(); // Moves focus to slider

            }
            if (e.Key == Key.Escape)
            {
                binding?.UpdateTarget(); // pulls the original source value back, discarding edits
                Sliding.Focus(); // Moves focus to slider
            }
        }
    }
}
