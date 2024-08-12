using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;

public class LineAnnotation : IAnnotation {
    public string Name { get; set; } = "Line";
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }
    public Drawing ToDrawing() {
        Point sp = new() { X = StartPoint.X, Y = StartPoint.Y };
        Point ep = new() { X = EndPoint.X, Y = EndPoint.Y };
        return new GeometryDrawing() {
            Brush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
            Pen = new(Brushes.Black, 2.0),
            Geometry = new LineGeometry(sp, ep)
        };
    }

    public Shape ToShape() {
        throw new System.NotImplementedException();
    }
}