# src/double-pendulum.Presentation

WPF application for real-time simulation of the double pendulum. Built with MVVM architecture and powered by the `double-pendulum.Model` physics engine.

## Contents

- `Commands/RelayCommand.cs`: Generic `ICommand` implementation for binding UI actions to ViewModel logic
- `ViewModels/`:
  - `ViewModelBase.cs`: Base class implementing `INotifyPropertyChanged`
  - `MainViewModel.cs`: ViewModel managing simulation state, slider bindings and render loop
- `Views/`:
  - `MainWindow.xaml` / `MainWindow.xaml.cs`: Main application window
  - `Rendering/PendulumRenderer.cs`: Draws the pendulum on a WPF canvas
  - `Controls/QuantitySlider.xaml`: Slider controls used for all parameter inputs
- `Resources/`: Application assets
- `App.xaml` / `App.xaml.cs`: Application entry point
