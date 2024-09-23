using System.Text;
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
        StringBuilder builder = new();
        if (identation is not null) {
            for (int i = 0; i < identation; i++) {
                _ = builder.Append(' ');
            }
        }

        string s = $"""\draw {LowerRightNode.ToTikzPointCoordinates()} rectangle {UpperLeftNode.ToTikzPointCoordinates()};""";

        _ = builder.Append(s);
        return builder.ToString();
    }

    public Geometry ToGeometry() {
        System.Windows.Point ul = new() {
            X = UpperLeftNode.NodeImagePoint[0],
            Y = UpperLeftNode.NodeImagePoint[1],
        };

        System.Windows.Point lr = new() {
            X = LowerRightNode.NodeImagePoint[0],
            Y = LowerRightNode.NodeImagePoint[1],
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

    public void Translate(int x, int y, DoubleSize s) {
    }
}