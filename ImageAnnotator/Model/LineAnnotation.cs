using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;

/// <summary>
/// Holds the data to the for a line annotation
/// </summary>
public class LineAnnotation : IAnnotation {
    public string Name { get; set; } = "Line";
    public required NodeAnnotation StartPoint { get; set; }
    public required NodeAnnotation EndPoint { get; set; }
    public Drawing ToDrawing() {
        Point sp = new() { X = StartPoint.X, Y = StartPoint.Y };
        Point ep = new() { X = EndPoint.X, Y = EndPoint.Y };

        GeometryGroup gg = new();

        return new GeometryDrawing() {
            Brush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
            Pen = new(Brushes.Black, 2.0),
            Geometry = new LineGeometry(sp, ep)
        };
    }



    public Shape ToShape() {
        //TODO: Figure out how to draw multiple shapes.
        int Width = 10;
        int Height = 10;
        return new Ellipse() {
            Stroke = Brushes.Black,
            Fill = Brushes.DarkBlue,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Width = Width,
            Height = Height,
            //TODO: Decide between start point and end point based on their relative coordinates
            Margin = new Thickness(StartPoint.X - (Width / 2.0), StartPoint.Y - (Height / 2.0), 0, 0)
        };
    }
}