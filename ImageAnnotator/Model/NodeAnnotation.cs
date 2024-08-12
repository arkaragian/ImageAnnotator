using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;

public class NodeAnnotation : IAnnotation {
    public string Name { get; set; } = "Node";
    public DoublePoint Point { get; set; }

    public Geometry ToGeometry() {
        return new EllipseGeometry() {
            Center = Point,
            RadiusX = 5,
            RadiusY = 5,
        };
    }

    public Shape ToShape() {
        int Width = 10;
        int Height = 10;
        return new Ellipse() {
            Stroke = Brushes.Black,
            Fill = Brushes.DarkBlue,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Width = Width,
            Height = Height,
            Margin = new Thickness(Point.X - (Width / 2.0), Point.Y - (Height / 2.0), 0, 0)
        };
    }
}