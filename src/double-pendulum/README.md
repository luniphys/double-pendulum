# src/double-pendulum

WPF application simulating a double pendulum using 4th-order Runge-Kutta numerical integration.

## Contents

- `Assets/`: Images and icons for the GUI
- `Services/`: Physics simulation backend
  - `PendulumPhysics.cs`: RK4 integration engine
  - `PendulumParameters.cs`: Simulation configuration
- `Views/`: Rendering and UI controls
  - `PendulumRenderer.cs`: Draws the pendulum on the canvas
  - `Controls/QuantitySlider.xaml`: Reusable slider control for adjusting parameters
- `App.xaml` / `App.xaml.cs`: Application entry point
- `MainWindow.xaml` / `MainWindow.xaml.cs`: Main window and simulation host
