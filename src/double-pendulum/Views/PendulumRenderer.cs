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
    private Canvas canvas;

    private Line line1;
    private Line line2;
    private Ellipse ellipse1;
    private Ellipse ellipse2;

    private Ellipse hangingPoint;

    private double hangingPointX;
    private double hangingPointY;

    private SolidColorBrush brush1 = new SolidColorBrush(Colors.White);
    private SolidColorBrush brush2 = new SolidColorBrush(Colors.White);

    private double scale = 100.0;

    private int MaxTrailLength;

    private Queue<(Point point, Color color)> pastPointsQueue;
    private List<Line> segmentPool;



    public PendulumRenderer(Canvas canvas)
    {
        this.canvas = canvas;

        line1 = new Line { Stroke = brush1, StrokeThickness = 3 };
        line2 = new Line { Stroke = brush2, StrokeThickness = 3 };
        hangingPoint = new Ellipse { Width = 10, Height = 10, Fill = Brushes.DarkGray, Stroke = Brushes.Black, StrokeThickness = 1.5 };
        ellipse1 = new Ellipse { Fill = brush1, Stroke = Brushes.Black, StrokeThickness = 1.5 };
        ellipse2 = new Ellipse { Fill = brush2, Stroke = Brushes.Black, StrokeThickness = 1.5 };

        pastPointsQueue = new Queue<(Point, Color)>();
        segmentPool = new List<Line>();

        canvas.Children.Add(line1);
        canvas.Children.Add(line2);
        canvas.Children.Add(hangingPoint);
        canvas.Children.Add(ellipse1);
        canvas.Children.Add(ellipse2);
    }



    #region Public methods

    /// <summary>
    /// Draws the pendulum at the specified positions on the canvas and optionally records the trail of its motion.
    /// </summary>
    /// <param name="position">A vector containing the 4 cartesian coordinates of the two pendulums (X,Y & Z,W).</param>
    /// <param name="recordTrail">Record the trail of the second mass.</param>
    public void Draw(Vector4 position, bool recordTrail = true)
    {
        Vector2 position1 = new Vector2(position.X, position.Y);
        Vector2 position2 = new Vector2(position.Z, position.W);

        hangingPointX = canvas.ActualWidth / 2;
        hangingPointY = canvas.ActualHeight / 4;

        double ellipse1X = hangingPointX + position1.X * scale;
        double ellipse1Y = hangingPointY - position1.Y * scale;

        line1.X1 = hangingPointX;
        line1.Y1 = hangingPointY;
        line1.X2 = ellipse1X;
        line1.Y2 = ellipse1Y;

        double ellipse2X = hangingPointX + position2.X * scale;
        double ellipse2Y = hangingPointY - position2.Y * scale;

        line2.X1 = ellipse1X;
        line2.Y1 = ellipse1Y;
        line2.X2 = ellipse2X;
        line2.Y2 = ellipse2Y;

        Canvas.SetLeft(hangingPoint, hangingPointX - hangingPoint.Width / 2);
        Canvas.SetTop(hangingPoint, hangingPointY - hangingPoint.Height / 2);

        Canvas.SetLeft(ellipse1, ellipse1X - ellipse1.Width / 2);
        Canvas.SetTop(ellipse1, ellipse1Y - ellipse1.Height / 2);
        Canvas.SetLeft(ellipse2, ellipse2X - ellipse2.Width / 2);
        Canvas.SetTop(ellipse2, ellipse2Y - ellipse2.Height / 2);

        DrawTrail(position2, brush2.Color, recordTrail);
    }


    public void DrawTrail(Vector2 position, Color color, bool recordTrail = true)
    {
        double screenX = hangingPointX + position.X * scale;
        double screenY = hangingPointY - position.Y * scale;

        if (recordTrail)
        {
            pastPointsQueue.Enqueue((new Point(screenX, screenY), color));

            if (pastPointsQueue.Count > MaxTrailLength)
            {
                pastPointsQueue.Dequeue();
            }
        }

        int segmentCount = Math.Max(0, pastPointsQueue.Count - 1);
        int i = 0;

        (Point point, Color color) prev = default;
        foreach (var current in pastPointsQueue)
        {
            if (i > 0)
            {
                int si = i - 1;
                segmentPool[si].X1 = prev.point.X;
                segmentPool[si].Y1 = prev.point.Y;
                segmentPool[si].X2 = current.point.X;
                segmentPool[si].Y2 = current.point.Y;
                segmentPool[si].StrokeThickness = ellipse2.Width / 2;
                segmentPool[si].Opacity = (double)i / segmentCount;
                ((SolidColorBrush)segmentPool[si].Stroke).Color = prev.color;
                segmentPool[si].Visibility = Visibility.Visible;
            }
            prev = current;
            i++;
        }

        for (int j = segmentCount; j < MaxTrailLength - 1; j++)
        {
            segmentPool[j].Visibility = Visibility.Hidden;
        }
    }


    public void EraseTrail()
    {
        pastPointsQueue.Clear();
        for (int i = 0; i < MaxTrailLength - 1; i++)
        {
            segmentPool[i].Visibility = Visibility.Hidden;
        }
    }


    public void SetTrailLength(int length)
    {
        int newLength = Math.Max(2, length); // To keep at least 1 segment

        if (newLength == MaxTrailLength) { return; }

        if (newLength < MaxTrailLength)
        {
            for (int i = newLength - 1; i < segmentPool.Count; i++)
            {
                segmentPool[i].Visibility = Visibility.Hidden;
            }
        }

        MaxTrailLength = newLength;

        while (segmentPool.Count < MaxTrailLength - 1)
        {
            Line seg = new Line { StrokeThickness = 2, StrokeLineJoin = PenLineJoin.Round, Visibility = Visibility.Hidden, Stroke = new SolidColorBrush(Colors.White) };
            canvas.Children.Insert(canvas.Children.IndexOf(line1), seg);
            segmentPool.Add(seg);
        }

        while (pastPointsQueue.Count > MaxTrailLength)
        {
            pastPointsQueue.Dequeue();
        }
    }


    /// <summary>
    /// Sets the colors of the two pendulums (brushes) using the RGB values.
    /// </summary>
    public void ChangeColor(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
    {
        brush1.Color = Color.FromRgb(r1, g1, b1);
        brush2.Color = Color.FromRgb(r2, g2, b2);
    }


    /// <summary>
    /// Updates the radii of two ellipses based on their corresponding mass values in a parabolic way.
    /// </summary>
    public void UpdateRadii(double mass1, double mass2)
    {
        ellipse1.Width = MassToRadius(mass1);
        ellipse1.Height = MassToRadius(mass1);
        ellipse2.Width = MassToRadius(mass2);
        ellipse2.Height = MassToRadius(mass2);
    }

    #endregion



    #region Private (helper) methods

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

