using double_pendulum.Model;
using double_pendulum.Presentation.Commands;
using double_pendulum.Presentation.Views.Controls;
using double_pendulum.Presentation.Views.Rendering;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace double_pendulum.Presentation.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    #region Private fields

    // Backing fields
    private double _length1 = 2.0;
    private double _length2 = 1.0;
    private double _mass1 = 50.0;
    private double _mass2 = 20.0;
    private double _angle1 = 105.0;
    private double _angle2 = 130.0;
    private double _damp = 2.0;

    private double _simSpeed = 1.0;
    private double _trailLength = 5.0;

    private bool _showColors = true;

    private bool _isRunning = false;


    // Other
    private PendulumPhysics? _pendulum = null;
    private PendulumRenderer? _renderer = null;

    private double _stepAccumulator;
    private TimeSpan _lastRenderTime = TimeSpan.Zero;

    #endregion



    public MainViewModel()
    {
        StartCommand = new RelayCommand(
            execute:    _ => StartMethod(),
            canExecute: _ => !_isRunning);

        ResetCommand = new RelayCommand(
            execute:    _ => ResetMethod(),
            canExecute: _ => _isRunning);

        SpeedResetCommand = new RelayCommand(
            execute:    _ => SpeedResetMethod(),
            canExecute: _ => true);
    }



    #region Public Properties

    public double Length1 { get => _length1; set => SetField(ref _length1, value); }
    public double Length2 { get => _length2; set => SetField(ref _length2, value); }
    public double Mass1 { get => _mass1; set => SetField(ref _mass1, value); }
    public double Mass2 { get => _mass2; set => SetField(ref _mass2, value); }
    public double Angle1 { get => _angle1; set => SetField(ref _angle1, value); }
    public double Angle2  { get => _angle2; set => SetField(ref _angle2, value); }
    public double Damp { get => _damp; set => SetField(ref _damp, value); }

    public double SimSpeed { get => _simSpeed; set => SetField(ref _simSpeed, value); }
    public double TrailLength { get => _trailLength; set => SetField(ref _trailLength, value); }

    public bool ShowColors { get => _showColors; set => SetField(ref _showColors, value); }

    public bool IsRunning { get => _isRunning; set => SetField(ref _isRunning, value); }
    public bool IsNotRunning => !_isRunning;


    // Commands
    public ICommand StartCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand SpeedResetCommand { get; }

    #endregion





    #region Private methods

    // Command methods
    private void StartMethod()
    {
        _isRunning = true;

        PendulumParameters parameters = BuildParameters();
        _pendulum = new PendulumPhysics(parameters);

        _stepAccumulator = 0.0;
        _lastRenderTime = TimeSpan.MinValue;

        CompositionTarget.Rendering += OnRendering;
    }

    private void ResetMethod()
    {
        _isRunning = false;

        _renderer?.EraseTrail();

        CompositionTarget.Rendering -= OnRendering;
        DrawPreview();
    }

    private void SpeedResetMethod()
    {
        _simSpeed = 1.0;
    }



    // Helper methods

    /// <summary>
    /// Fires at every frame (synced to GPU) to advance pendulum simulation and redraw its state.
    /// Also updates pendulums color depenging on ColorCheckBox.
    /// </summary>
    private void OnRendering(object? sender, EventArgs e)
    {
        var renderArgs = (RenderingEventArgs)e;

        if (_lastRenderTime == TimeSpan.MinValue) // .MinValue is special first frame value. Checking if its first frame -> Save time & skip.
        {
            _lastRenderTime = renderArgs.RenderingTime;
            return;
        }

        if (renderArgs.RenderingTime == _lastRenderTime) { return; } // Avoid double event firing per frame

        double elapsedMs = (renderArgs.RenderingTime - _lastRenderTime).TotalMilliseconds;
        _lastRenderTime = renderArgs.RenderingTime;

        _stepAccumulator += elapsedMs * _simSpeed * 0.65;

        while (_stepAccumulator >= 1.0)
        {
            _pendulum?.Step();
            _stepAccumulator -= 1.0;
        }

        _renderer?.Draw(_pendulum.GetPosition());

        UpdatePendulumColor();
    }


    /// <summary>
    /// Create new instance of the PendulumParameters class using the slider values.
    /// </summary>
    private PendulumParameters BuildParameters()
    {
        return new PendulumParameters(
            (float)Length1,
            (float)Length2,
            (float)Mass1,
            (float)Mass2,
            (float)Angle1,
            (float)Angle2,
            (float)Damp);
    }


    /// <summary>
    /// Draws a preview of the double pendulum with the initial slider values
    /// </summary>
    /// <remarks>Ensures blue/red color or white depending on ColorCheckBox. Sets radii
    /// corresponding to pendulums masses.</remarks>
    private void DrawPreview()
    {
        if (_renderer is null) { return; }

        _renderer.EraseTrail();

        if (_showColors) { _renderer.ChangeColor(0, 0, 255, 0, 0, 255); }
        else { _renderer.ChangeColor(255, 255, 255, 255, 255, 255); }

        _renderer.UpdateRadii(Mass1, Mass2);

        PendulumParameters parameters = BuildParameters();
        PendulumPhysics previewPendulum = new PendulumPhysics(parameters);
        _renderer.Draw(previewPendulum.GetPosition());
    }


    /// <summary>
    /// Updates the pendulum's color based on its angular velocities and the state of the color checkbox.
    /// </summary>
    private void UpdatePendulumColor()
    {
        if (_showColors)
        {
            _renderer?.ChangeColor(255, 255, 255, 255, 255, 255);
            return;
        }

        const double maxAngularVelocity = 10.0;

        double angularVelocity1 = Math.Min(Math.Abs(_pendulum.State.Z), maxAngularVelocity);
        byte red1 = (byte)(angularVelocity1 / maxAngularVelocity * 255);

        double angularVelocity2 = Math.Min(Math.Abs(_pendulum.State.W), maxAngularVelocity);
        byte red2 = (byte)(angularVelocity2 / maxAngularVelocity * 255);

        _renderer?.ChangeColor(red1, 0, (byte)(255 - red1), red2, 0, (byte)(255 - red2));
    }

    #endregion

}
