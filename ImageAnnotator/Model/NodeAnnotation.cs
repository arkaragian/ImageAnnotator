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
    public required DoublePoint NodeImageCoordinates { get; set; }

    public required DoublePoint NormalizedCoordinates { get; set; }

    public string ToCode(uint? identation) {
        StringBuilder builder = new();
        if (identation is not null) {
            for (int i = 0; i < identation; i++) {
                _ = builder.Append(' ');
            }
        }

        string no_spaces_name = Name.Replace(" ", "");

        string s = $$"""\node[anchor=south west,inner sep=0] ({{no_spaces_name}}) at ( {{NormalizedCoordinates.X.ToString("F3")}}, {{NormalizedCoordinates.Y.ToString("F3")}} ) { {{Text}} };""";

        _ = builder.Append(s);
        return builder.ToString();
    }

    public Geometry ToGeometry() {
        return new EllipseGeometry() {
            Center = NodeImageCoordinates,
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
            Margin = new Thickness(NodeImageCoordinates.X - (Width / 2.0), NodeImageCoordinates.Y - (Height / 2.0), 0, 0)
        };
    }
}