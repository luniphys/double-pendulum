using double_pendulum.Presentation.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace double_pendulum.Presentation;

public partial class MainWindow : Window
{
    private MainViewModel ViewModel => (MainViewModel)DataContext;


    public MainWindow()
    {
        InitializeComponent();
    }


    /// <summary>
    /// Passes the canvas to the ViewModel once it's ready.
    /// </summary>
    private void PendulumCanvas_Loaded(object sender, RoutedEventArgs e)
        => ViewModel.Initialize(PendulumCanvas);


    /// <summary>
    /// Notifies the ViewModel when the canvas' size has changed.
    /// </summary>
    private void PendulumCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        => ViewModel.OnCanvasSizeChanged();


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


    /// <summary>
    /// Determines whether an element is a child of a specific type.
    /// </summary>
    /// <remarks>Travelling up the visual tree from the initial element and checking each parents' type.</remarks>
    private static bool IsChildOfType<T>(DependencyObject? element) where T : DependencyObject
    {
        while (element != null)
        {
            if (element is T) { return true; }
            element = VisualTreeHelper.GetParent(element);
        }
        return false;
    }
}
