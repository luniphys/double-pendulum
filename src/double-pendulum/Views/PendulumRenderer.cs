using System.Numerics;
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
    Line line1;
    Line line2;
    Ellipse ellipse1;
    Ellipse ellipse2;

    public PendulumRenderer(Canvas canvas)
    {
        this.canvas = canvas;

        line1 = new Line {Stroke = Brushes.White, StrokeThickness = 3};
        line2 = new Line { Stroke = Brushes.White, StrokeThickness = 3 };
        ellipse1 = new Ellipse { Width = 20, Height = 20, Fill = Brushes.White };
        ellipse2 = new Ellipse { Width = 20, Height = 20, Fill = Brushes.White };

        canvas.Children.Add(line1);
        canvas.Children.Add(line2);
        canvas.Children.Add(ellipse1);
        canvas.Children.Add(ellipse2);
    }

    public void Draw(Vector2 position1, Vector2 position2)
    {
        double hangingPointX = canvas.ActualWidth / 2;
        double hangingPointY = canvas.ActualHeight / 8;

        double scale = 100;

        double ellipse1X = hangingPointX + position1.X * scale;
        double ellipse1Y = hangingPointY - position1.Y * scale;

        line1.X1 = hangingPointX;
        line1.Y1 = hangingPointY;
        line1.X2 = ellipse1X;
        line1.Y2 = ellipse1Y;

        double ellipse2X = ellipse1X + 1;
        double ellipse2Y = ellipse1X + 1;

        line2.X1 = ellipse1X;
        line2.Y1 = ellipse1Y;
        line2.X2 = ellipse2X;
        line2.Y2 = ellipse2Y;

        Canvas.SetLeft(ellipse1, ellipse1X - ellipse1.Width / 2);
        Canvas.SetTop(ellipse1, ellipse1Y - ellipse1.Height / 2);
        Canvas.SetLeft(ellipse2, ellipse2X - ellipse2.Width / 2);
        Canvas.SetTop(ellipse2, ellipse2Y - ellipse2.Height / 2);
    }

    public void ClearCanvas()
    {
        canvas.Children.Clear();
    }
}

