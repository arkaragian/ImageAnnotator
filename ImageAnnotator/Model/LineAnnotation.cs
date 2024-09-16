using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;

/// <summary>
/// A class that the represents a line annotation.
/// </summary>
public class LineAnnotation : IAnnotation {
    public string Name { get; set; } = "Line";
    public required NodeAnnotation StartPoint { get; set; }
    public required NodeAnnotation EndPoint { get; set; }


    public void ResizeCoordinates(DoubleSize newSize) {
        StartPoint.ResizeCoordinates(newSize);
        EndPoint.ResizeCoordinates(newSize);
    }

    public string ToCode(uint? identation) {
        StringBuilder builder = new();
        if (identation is not null) {
            for (int i = 0; i < identation; i++) {
                _ = builder.Append(' ');
            }
        }

        string s = $"""\draw {StartPoint.ToTikzPointCoordinates()} -- {EndPoint.ToTikzPointCoordinates()};""";

        _ = builder.Append(s);
        return builder.ToString();
    }

    public Geometry ToGeometry() {
        GeometryGroup gg = new();
        LineGeometry lg = new() {
            StartPoint = new() {
                X = StartPoint.NodeImagePoint[0],
                Y = StartPoint.NodeImagePoint[1],
            },
            EndPoint = new() {
                X = EndPoint.NodeImagePoint[0],
                Y = EndPoint.NodeImagePoint[1],
            }
        };
        gg.Children.Add(StartPoint.ToGeometry());
        gg.Children.Add(lg);
        gg.Children.Add(EndPoint.ToGeometry());
        return gg;
    }



    ///<inheritdoc cref=IAnnotation.ToShape/>
    public Shape ToShape() {
        // Path path = new() {
        //     Data = ToGeometry(),
        //     Stroke = Brushes.Black,
        //     StrokeThickness = 4,
        // };
        // return path;
        //
        return new ColorLine() {
            StartPoint = new() {
                X = StartPoint.NodeImagePoint[0],
                Y = StartPoint.NodeImagePoint[1],
            },
            EndPoint = new() {
                X = EndPoint.NodeImagePoint[0],
                Y = EndPoint.NodeImagePoint[1],
            },
            StrokeThickness = 2
        };
    }
}