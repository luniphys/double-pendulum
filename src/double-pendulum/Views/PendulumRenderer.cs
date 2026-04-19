using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace double_pendulum.Views;

/// <summary>
/// Draws the lines (rods) and ellipses (bobs) on the canvas each frame.
/// </summary>

public class PendulumRenderer
{
    Canvas canvas;
    Ellipse hangingPoint;

    Line line1;
    Line line2;
    Ellipse ellipse1;
    Ellipse ellipse2;

    SolidColorBrush brush1 = new SolidColorBrush(Colors.White);
    SolidColorBrush brush2 = new SolidColorBrush(Colors.White);

    public PendulumRenderer(Canvas canvas)
    {
        this.canvas = canvas;

        line1 = new Line { Stroke = Brushes.White, StrokeThickness = 3};
        line2 = new Line { Stroke = Brushes.White, StrokeThickness = 3 };
        ellipse1 = new Ellipse { Fill = brush1 };
        ellipse2 = new Ellipse { Fill = brush2 };
        hangingPoint = new Ellipse { Width = 10, Height = 10, Fill = Brushes.Gray };

        canvas.Children.Add(line1);
        canvas.Children.Add(line2);
        canvas.Children.Add(hangingPoint);
        canvas.Children.Add(ellipse1);
        canvas.Children.Add(ellipse2);
    }


    public void Draw(Vector4 position)
    {
        SetVisibility(Visibility.Visible);

        Vector2 position1 = new Vector2(position.X, position.Y);
        Vector2 position2 = new Vector2(position.Z, position.W);

        double hangingPointX = canvas.ActualWidth / 2;
        double hangingPointY = canvas.ActualHeight / 4;

        double scale = 100;

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
    }


    private void SetVisibility(Visibility visibility)
    {
        line1.Visibility = visibility;
        line2.Visibility = visibility;
        ellipse1.Visibility = visibility;
        ellipse2.Visibility = visibility;
        hangingPoint.Visibility = visibility;
    }
    public void Hide()
    {
        SetVisibility(Visibility.Hidden);
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
    public void DrawTail()
    {

    }
}

