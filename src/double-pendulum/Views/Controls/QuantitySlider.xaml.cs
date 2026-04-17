using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace double_pendulum.Views.Controls
{
    public partial class QuantitySlider : UserControl
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


        public QuantitySlider()
        {
            InitializeComponent();
        }

        private static object CoerceQuantityValue(DependencyObject d, object baseValue)
        {
            var control = (QuantitySlider)d;
            double value = (double)baseValue;
            double min = control.MinSliderValue;
            double max = control.MaxSliderValue;

            if (min > max)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }
            if (value < min) { return min; }
            if (value > max) { return max; }
            return value;
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var textBox = (TextBox)sender;
                var binding = textBox.GetBindingExpression(TextBox.TextProperty);
                binding?.UpdateSource();
                Sliding.Focus();
            }
        }
    }
}
