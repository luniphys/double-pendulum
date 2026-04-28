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
    Canvas canvas;
    Ellipse hangingPoint;

    Line line1;
    Line line2;
    Ellipse ellipse1;
    Ellipse ellipse2;

    private double hangingPointX;
    private double hangingPointY;

    private double scale = 100;

    SolidColorBrush brush1 = new SolidColorBrush(Colors.White);
    SolidColorBrush brush2 = new SolidColorBrush(Colors.White);

    int MaxTrailLength;

    Queue<(Point point, Color color)> pastPointsQueue;
    List<Line> segmentPool;


    public PendulumRenderer(Canvas canvas)
    {
        this.canvas = canvas;

        line1 = new Line { Stroke = brush1, StrokeThickness = 3 };
        line2 = new Line { Stroke = brush2, StrokeThickness = 3 };
        ellipse1 = new Ellipse { Fill = brush1, Stroke = Brushes.Black, StrokeThickness = 1.5 };
        ellipse2 = new Ellipse { Fill = brush2, Stroke = Brushes.Black, StrokeThickness = 1.5 };
        hangingPoint = new Ellipse { Width = 10, Height = 10, Fill = Brushes.DarkGray, Stroke = Brushes.Black, StrokeThickness = 1.5 };

        pastPointsQueue = new Queue<(Point, Color)>();
        segmentPool = new List<Line>();

        canvas.Children.Add(line1);
        canvas.Children.Add(line2);
        canvas.Children.Add(hangingPoint);
        canvas.Children.Add(ellipse1);
        canvas.Children.Add(ellipse2);
    }


    public void Draw(Vector4 position)
    {
        line1.Visibility = Visibility.Visible;
        line2.Visibility = Visibility.Visible;
        ellipse1.Visibility = Visibility.Visible;
        ellipse2.Visibility = Visibility.Visible;
        hangingPoint.Visibility = Visibility.Visible;

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

        DrawTrail(position2, brush2.Color);
    }

    public void DrawTrail(Vector2 position, Color color)
    {
        double screenX = hangingPointX + position.X * scale;
        double screenY = hangingPointY - position.Y * scale;

        pastPointsQueue.Enqueue((new Point(screenX, screenY), color));

        if (pastPointsQueue.Count > MaxTrailLength)
        {
            pastPointsQueue.Dequeue();
        }

        var points = pastPointsQueue.ToArray();
        int segmentCount = points.Length - 1;

        for (int i = 0; i < segmentCount; i++)
        {
            segmentPool[i].X1 = points[i].point.X;
            segmentPool[i].Y1 = points[i].point.Y;
            segmentPool[i].X2 = points[i + 1].point.X;
            segmentPool[i].Y2 = points[i + 1].point.Y;
            segmentPool[i].StrokeThickness = ellipse2.Width / 2;
            segmentPool[i].Opacity = (double)(i + 1) / segmentCount;
            ((SolidColorBrush)segmentPool[i].Stroke).Color = points[i].color;
            segmentPool[i].Visibility = Visibility.Visible;
        }

        for (int i = segmentCount; i < MaxTrailLength - 1; i++)
        {
            segmentPool[i].Visibility = Visibility.Hidden;
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

    private double MassToRadius(double mass)
    {
        const double MinRadius = 8;
        const double MaxRadius = 30;
        const double MinMass = 1;
        const double MaxMass = 50;

        double temp = (Math.Sqrt(mass) - Math.Sqrt(MinMass)) / (Math.Sqrt(MaxMass) - Math.Sqrt(MinMass));
        return MinRadius + temp * (MaxRadius - MinRadius);
    }
    public void UpdateRadii(double mass1, double mass2)
    {
        ellipse1.Width = MassToRadius(mass1);
        ellipse1.Height = MassToRadius(mass1);
        ellipse2.Width = MassToRadius(mass2);
        ellipse2.Height = MassToRadius(mass2);
    }


    public void ChangeColor(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
    {
        brush1.Color = Color.FromRgb(r1, g1, b1);
        brush2.Color = Color.FromRgb(r2, g2, b2);
    }
}

