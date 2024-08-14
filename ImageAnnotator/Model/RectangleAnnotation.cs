using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;
public class RectangleAnnotation : IAnnotation {
    public string Name { get; set; } = "Rectangle";
    public required NodeAnnotation UpperLeftNode { get; set; }
    public required NodeAnnotation LowerRightNode { get; set; }

    public string ToCode(uint? identation) {
        return "";
    }

    public Geometry ToGeometry() {
        return new RectangleGeometry() {
            Rect = new System.Windows.Rect(UpperLeftNode.NodeImageCoordinates, LowerRightNode.NodeImageCoordinates)
        };
    }

    public Shape ToShape() {
        return new Path {
            Data = ToGeometry(),
            Stroke = Brushes.Green,
            StrokeThickness = 2,
        };
    }
}