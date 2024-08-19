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
    public required MathPoint NodeImageCoordinates { get; set; }

    public required MathPoint NormalizedCoordinates { get; set; }

    public void ResizeCoordinates(DoubleSize newSize) {
        //Since we have normalized coordinates we can use those and the new
        //size to deduce the non-normalized coordinates. But the normalized
        //coordinates
        //DoublePoint local_wpf_normalized = Tikz.TransformCoordinates.ToWPFCoordinates(NormalizedCoordinates);
        MathPoint local_wpf_normalized = new() {
            Coordinates = new double[] { 0.0, 0.0 }
        };

        double x = local_wpf_normalized[0] * newSize.Width;
        double y = local_wpf_normalized[1] * newSize.Height;
        NodeImageCoordinates = new MathPoint() {
            Coordinates = new double[] { x, y }
        };
    }

    public string ToCode(uint? identation) {
        StringBuilder builder = new();
        if (identation is not null) {
            for (int i = 0; i < identation; i++) {
                _ = builder.Append(' ');
            }
        }

        string no_spaces_name = Name.Replace(" ", "");

        string s = $$"""\node[anchor=south west,inner sep=0] ({{no_spaces_name}}) at ( {{NormalizedCoordinates[0].ToString("F3")}}, {{NormalizedCoordinates[1].ToString("F3")}} ) { {{Text}} };""";

        _ = builder.Append(s);
        return builder.ToString();
    }

    public Geometry ToGeometry() {
        return new EllipseGeometry() {
            Center = new Point() {
                X = NodeImageCoordinates[0],
                Y = NodeImageCoordinates[1]
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
            Margin = new Thickness(NodeImageCoordinates.X - (Width / 2.0), NodeImageCoordinates.Y - (Height / 2.0), 0, 0)
        };
    }
}