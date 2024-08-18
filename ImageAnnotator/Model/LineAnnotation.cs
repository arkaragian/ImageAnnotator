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
        return "";
    }

    public Geometry ToGeometry() {
        GeometryGroup gg = new();
        LineGeometry lg = new() {
            StartPoint = StartPoint.NodeImageCoordinates,
            EndPoint = EndPoint.NodeImageCoordinates
        };
        gg.Children.Add(StartPoint.ToGeometry());
        gg.Children.Add(lg);
        gg.Children.Add(EndPoint.ToGeometry());
        return gg;
    }



    ///<inheritdoc cref=IAnnotation.ToShape/>
    public Shape ToShape() {
        return new Path {
            Data = ToGeometry(),
            Stroke = Brushes.Black,
            StrokeThickness = 4,
        };
    }
}