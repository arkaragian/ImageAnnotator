using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;
public class RectangleAnnotation : IAnnotation {
    public string Name { get; set; } = "Rectangle";
    public required NodeAnnotation UpperLeftNode { get; set; }
    public required NodeAnnotation LowerRightNode { get; set; }

    public void ResizeCoordinates(DoubleSize newSize) {
        UpperLeftNode.ResizeCoordinates(newSize);
        LowerRightNode.ResizeCoordinates(newSize);
    }

    public string ToCode(uint? identation) {
        return "";
    }

    public Geometry ToGeometry() {
        System.Windows.Point ul = new() {
            X = UpperLeftNode.NodeImageCoordinates[0],
            Y = UpperLeftNode.NodeImageCoordinates[1],
        };

        System.Windows.Point lr = new() {
            X = LowerRightNode.NodeImageCoordinates[0],
            Y = LowerRightNode.NodeImageCoordinates[1],
        };
        return new RectangleGeometry() {
            Rect = new System.Windows.Rect(ul, lr)
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