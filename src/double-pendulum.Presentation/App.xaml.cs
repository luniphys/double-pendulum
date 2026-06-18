using System.Windows;
using double_pendulum.Presentation.ViewModels;
using double_pendulum.Presentation.Views;

namespace double_pendulum.Presentation;


public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainViewModel = new MainViewModel();

        var mainWindow = new MainWindow
        {
            DataContext = mainViewModel
        };

        mainWindow.Show();
    }
}
