using libGeometry;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;

public class NodeAnnotation : IAnnotation {
    /// <summary>
    /// The name of the node
    /// </summary>
    public string Name { get; set; } = "Node";

    /// <summary>
    /// The text that the node may contain
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// The location on the image that the node is placed on
    /// </summary>
    public required MathPoint NodeImagePoint { get; set; }

    /// <summary>
    /// The normalized image coordinates
    /// </summary>
    public required MathPoint NodeImageNormalizedPoint { get; set; }

    /// <summary>
    /// The tikz coordinates
    /// </summary>
    public required MathPoint NodeTikzPoint { get; set; }

    public void ResizeCoordinates(DoubleSize newSize) {
        //Since we have normalized coordinates we can use those and the new
        //size to deduce the non-normalized coordinates. But the normalized
        //coordinates
        //DoublePoint local_wpf_normalized = Tikz.TransformCoordinates.ToWPFCoordinates(NormalizedCoordinates);

        double x = NodeImageNormalizedPoint[0] * newSize.Width;
        double y = NodeImageNormalizedPoint[1] * newSize.Height;
        NodeImagePoint[0] = x;
        NodeImagePoint[1] = y;
    }

    public string ToTikzPointCoordinates() {
        return $"( {NodeTikzPoint[0]:F3}, {NodeTikzPoint[1]:F3} )";

    }

    public string ToCode(uint? identation) {
        StringBuilder builder = new();
        if (identation is not null) {
            for (int i = 0; i < identation; i++) {
                _ = builder.Append(' ');
            }
        }

        string no_spaces_name = Name.Replace(" ", "");

        string s = $$"""\node[anchor=south west,inner sep=0] ({{no_spaces_name}}) at ( {{NodeTikzPoint[0]:F3}}, {{NodeTikzPoint[1]:F3}} ) { {{Text}} };""";

        _ = builder.Append(s);
        return builder.ToString();
    }

    public Geometry ToGeometry() {
        return new EllipseGeometry() {
            Center = new Point() {
                X = NodeImagePoint[0],
                Y = NodeImagePoint[1]
            },
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
            Margin = new Thickness(NodeImagePoint[0] - (Width / 2.0), NodeImagePoint[1] - (Height / 2.0), 0, 0)
        };
    }

    public void Translate(int xTranslation, int yTranslation, DoubleSize size) {
        NodeImagePoint[0] = NodeImagePoint[0] + xTranslation;
        NodeImagePoint[1] = NodeImagePoint[1] + yTranslation;

        NodeImageNormalizedPoint[0] = NodeImagePoint[0] / size.Width;
        NodeImageNormalizedPoint[1] = NodeImagePoint[1] / size.Height;

        //TODO: Calculate the tikz point for this we need all the logic of the
        //coordinate transformation.

    }
}