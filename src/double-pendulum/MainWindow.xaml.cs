using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace double_pendulum
{
    public partial class MainWindow : Window

    {
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
    }
}