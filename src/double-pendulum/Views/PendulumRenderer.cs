using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace double_pendulum.Views;

/// <summary>
/// Draws the lines (rods) and ellipses (bobs) plus a trail path on the canvas each frame.
/// </summary>
public class PendulumRenderer
{
    private readonly Canvas _canvas;

    private readonly Line _line1;
    private readonly Line _line2;
    private readonly Ellipse _ellipse1;
    private readonly Ellipse _ellipse2;

    private readonly Ellipse _hangingPoint;

    private readonly Queue<(Point point, Color color)> _trailPoints;
    private readonly List<Line> _trailSegments;


    private readonly SolidColorBrush _brush1 = new SolidColorBrush(Colors.White);
    private readonly SolidColorBrush _brush2 = new SolidColorBrush(Colors.White);

    private const double Scale = 100.0;

    private int _maxTrailLength;



    public PendulumRenderer(Canvas canvas)
    {
        this._canvas = canvas;

        _line1 = new Line { Stroke = _brush1, StrokeThickness = 3 };
        _line2 = new Line { Stroke = _brush2, StrokeThickness = 3 };
        _hangingPoint = new Ellipse { Width = 10, Height = 10, Fill = Brushes.DarkGray, Stroke = Brushes.Black, StrokeThickness = 1.5 };
        _ellipse1 = new Ellipse { Fill = _brush1, Stroke = Brushes.Black, StrokeThickness = 1.5 };
        _ellipse2 = new Ellipse { Fill = _brush2, Stroke = Brushes.Black, StrokeThickness = 1.5 };

        _trailPoints = new Queue<(Point, Color)>();
        _trailSegments = new List<Line>();

        canvas.Children.Add(_line1);
        canvas.Children.Add(_line2);
        canvas.Children.Add(_hangingPoint);
        canvas.Children.Add(_ellipse1);
        canvas.Children.Add(_ellipse2);
    }



    #region Public methods

    /// <summary>
    /// Draws the pendulums at the specified positions on the canvas and records the trail of its motion.
    /// </summary>
    /// <param name="position">A vector containing the 4 cartesian coordinates of the two pendulums (X1,Y1 & X2,Y2).</param>
    public void Draw(Vector4 position)
    {
        Vector2 position1 = new Vector2(position.X, position.Y);
        Vector2 position2 = new Vector2(position.Z, position.W);

        double hangingPointX = _canvas.ActualWidth / 2;
        double hangingPointY = _canvas.ActualHeight / 4;

        float ellipse1X = (float)(hangingPointX + position1.X * Scale);
        float ellipse1Y = (float)(hangingPointY - position1.Y * Scale);

        _line1.X1 = hangingPointX;
        _line1.Y1 = hangingPointY;
        _line1.X2 = ellipse1X;
        _line1.Y2 = ellipse1Y;

        float ellipse2X = (float)(hangingPointX + position2.X * Scale);
        float ellipse2Y = (float)(hangingPointY - position2.Y * Scale);

        _line2.X1 = ellipse1X;
        _line2.Y1 = ellipse1Y;
        _line2.X2 = ellipse2X;
        _line2.Y2 = ellipse2Y;

        Canvas.SetLeft(_hangingPoint, hangingPointX - _hangingPoint.Width / 2);
        Canvas.SetTop(_hangingPoint, hangingPointY - _hangingPoint.Height / 2);

        Canvas.SetLeft(_ellipse1, ellipse1X - _ellipse1.Width / 2);
        Canvas.SetTop(_ellipse1, ellipse1Y - _ellipse1.Height / 2);
        Canvas.SetLeft(_ellipse2, ellipse2X - _ellipse2.Width / 2);
        Canvas.SetTop(_ellipse2, ellipse2Y - _ellipse2.Height / 2);

        DrawTrail(new Vector2(ellipse2X, ellipse2Y), _brush2.Color);
    }


    /// <summary>
    /// Clearing all points from the trail queue and hiding all visible trail segments.
    /// </summary>
    public void EraseTrail()
    {
        _trailPoints.Clear();
        for (int i = 0; i < _maxTrailLength - 1; i++)
        {
            _trailSegments[i].Visibility = Visibility.Hidden;
        }
    }


    /// <summary>
    /// Sets the maximum number of visible segments in the trail.
    /// </summary>
    /// <param name="length">Desired trail length.</param>
    public void SetTrailLength(int length)
    {
        int newLength = Math.Max(2, length);

        if (newLength == _maxTrailLength) { return; }

        // If trail gets shorter
        if (newLength < _maxTrailLength)
        {
            for (int i = newLength - 1; i < _trailSegments.Count; i++)
            {
                _trailSegments[i].Visibility = Visibility.Hidden;
            }
        }

        _maxTrailLength = newLength;

        // If trail gets longer
        while (_trailSegments.Count < _maxTrailLength - 1)
        {
            Line seg = new Line { StrokeThickness = 2, StrokeLineJoin = PenLineJoin.Round, Visibility = Visibility.Hidden, Stroke = new SolidColorBrush(Colors.White) };
            _canvas.Children.Insert(_canvas.Children.IndexOf(_line1), seg);
            _trailSegments.Add(seg);
        }

        while (_trailPoints.Count > _maxTrailLength)
        {
            _trailPoints.Dequeue();
        }
    }


    /// <summary>
    /// Sets the colors of the two pendulums (brushes) using the RGB values.
    /// </summary>
    public void ChangeColor(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
    {
        _brush1.Color = Color.FromRgb(r1, g1, b1);
        _brush2.Color = Color.FromRgb(r2, g2, b2);
    }


    /// <summary>
    /// Updates the radii of two ellipses based on their corresponding mass values in a parabolic way.
    /// </summary>
    public void UpdateRadii(double mass1, double mass2)
    {
        double radius1 = MassToRadius(mass1);
        double radius2 = MassToRadius(mass2);

        _ellipse1.Width = radius1;
        _ellipse1.Height = radius1;
        _ellipse2.Width = radius2;
        _ellipse2.Height = radius2;
    }

    #endregion



    #region Private (helper) methods

    /// <summary>
    /// Updates the trail of the second pendulum.
    /// </summary>
    /// <remarks>The trail has a maximum number of segments as defined by MaxTrailLength. Older ones are
    /// removed and new ones are added to ensure the desired trail length. The opacity of each segment is adjusted
    /// every frame to create a fading effect.</remarks>
    /// <param name="position">The position of the new point to add to the trail.</param>
    /// <param name="color">The color to use for that new trail segment.</param>
    private void DrawTrail(Vector2 position, Color color)
    {
        _trailPoints.Enqueue((new Point(position.X, position.Y), color));

        if (_trailPoints.Count > _maxTrailLength)
        {
            _trailPoints.Dequeue();
        }

        int segmentCount = Math.Max(0, _trailPoints.Count - 1);
        (Point point, Color color) prev = (new Point(), new Color());

        int i = 0;
        foreach (var current in _trailPoints)
        {
            if (i > 0)
            {
                _trailSegments[i - 1].X1 = prev.point.X;
                _trailSegments[i - 1].Y1 = prev.point.Y;
                _trailSegments[i - 1].X2 = current.point.X;
                _trailSegments[i - 1].Y2 = current.point.Y;
                _trailSegments[i - 1].StrokeThickness = _ellipse2.Width / 2;
                _trailSegments[i - 1].Opacity = (double)i / segmentCount;
                _trailSegments[i - 1].Stroke = new SolidColorBrush(prev.color);
                _trailSegments[i - 1].Visibility = Visibility.Visible;
            }
            prev = current;
            i++;
        }

        for (int j = segmentCount; j < _maxTrailLength - 1; j++)
        {
            _trailSegments[j].Visibility = Visibility.Hidden;
        }
    }


    /// <summary>
    /// Calculates the radius of the ellipse corresponding to a given mass using a parabolic trend.
    /// </summary>
    private double MassToRadius(double mass)
    {
        const double MinRadius = 8;
        const double MaxRadius = 30;
        const double MinMass = 1;
        const double MaxMass = 50;

        double temp = (Math.Sqrt(mass) - Math.Sqrt(MinMass)) / (Math.Sqrt(MaxMass) - Math.Sqrt(MinMass));
        return MinRadius + temp * (MaxRadius - MinRadius);
    }

    #endregion
}

