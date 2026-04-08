using System.Windows;

namespace double_pendulum
{
    public partial class MainWindow : Window

    {
        bool running = false;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (running)
            {
                TestButton.Content = "Run";
                TestTextBlock.Text = "Stopped";
            }
            else
            {
                TestButton.Content = "Stop";
                TestTextBlock.Text = "Running";
            }
            running = !running;
        }
    }
}