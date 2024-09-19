using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;

/// <summary>
/// A shape that represents a line with with it's two nodes colored.
/// </summary>
public class ColorLine : Shape {

    public required Point StartPoint { get; set; }
    public required Point EndPoint { get; set; }
    public double CircleRadius { get; set; } = 5;

    // Add a new property for the circle's fill color
    public Brush CircleFill { get; set; } = Brushes.Red;

    protected override Geometry DefiningGeometry {
        get {

            EllipseGeometry start = new() {
                Center = StartPoint,
                RadiusX = 5,
                RadiusY = 5
            };

            LineGeometry lg = new() {
                StartPoint = StartPoint,
                EndPoint = EndPoint,
            };

            EllipseGeometry end = new() {
                Center = EndPoint,
                RadiusX = 5,
                RadiusY = 5
            };

            GeometryGroup gg = new();
            gg.Children.Add(start);
            gg.Children.Add(lg);
            gg.Children.Add(end);

            return gg;
        }
    }

    protected override void OnRender(DrawingContext drawingContext) {

        // Draw the line with the default stroke
        drawingContext.DrawGeometry(brush: null, new Pen(Brushes.Black, StrokeThickness), new LineGeometry(StartPoint, EndPoint));

        // Draw the circle with the specified fill color
        drawingContext.DrawGeometry(CircleFill, null, new EllipseGeometry(StartPoint, CircleRadius, CircleRadius));


        // Draw the circle with the specified fill color
        drawingContext.DrawGeometry(CircleFill, null, new EllipseGeometry(EndPoint, CircleRadius, CircleRadius));
    }
}