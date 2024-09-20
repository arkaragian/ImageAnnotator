using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model.Shapes;

/// <summary>
/// A Custom shape that is a line with the only difference that it's starting and
/// end points are colored circles.
/// </summary>
public class ColorLine : Shape {

    /// <summary>
    /// The starting point of the line.
    /// </summary>
    public required Point StartPoint { get; set; }

    /// <summary>
    /// The ending point of the line.
    /// </summary>
    public required Point EndPoint { get; set; }

    /// <summary>
    /// The drawing radus of the nodes that are pixels.
    /// </summary>
    public double CircleRadius { get; set; } = 5;

    /// <summary>
    /// The brush that will be used for the circle.
    /// </summary>
    public Brush CircleFill { get; set; } = Brushes.Red;

    ///<inheritdoc/>
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

    ///<inheritdoc/>
    protected override void OnRender(DrawingContext drawingContext) {
        // Draw the line with the default stroke
        drawingContext.DrawGeometry(brush: null, new Pen(Brushes.Black, StrokeThickness), new LineGeometry(StartPoint, EndPoint));

        // Draw the circle with the specified fill color
        drawingContext.DrawGeometry(CircleFill, null, new EllipseGeometry(StartPoint, CircleRadius, CircleRadius));

        // Draw the circle with the specified fill color
        drawingContext.DrawGeometry(CircleFill, null, new EllipseGeometry(EndPoint, CircleRadius, CircleRadius));
    }
}