using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace double_pendulum.view.UserControls
{
    public partial class Sliders : UserControl
    {
        public static readonly DependencyProperty SliderColorProperty =
            DependencyProperty.Register(
                nameof(SliderColor),
                typeof(Brush),
                typeof(Sliders),
                new PropertyMetadata(Brushes.Yellow));

        public Brush SliderColor
        {
            get { return (Brush)GetValue(SliderColorProperty); }
            set { SetValue(SliderColorProperty, value); }
        }
        public Sliders()
        {
            InitializeComponent();
        }
    }
}
