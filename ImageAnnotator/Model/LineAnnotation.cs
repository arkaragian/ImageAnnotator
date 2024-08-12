using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;

/// <summary>
/// Holds the data to the for a line annotation
/// </summary>
public class LineAnnotation : IAnnotation {
    public string Name { get; set; } = "Line";
    public required NodeAnnotation StartPoint { get; set; }
    public required NodeAnnotation EndPoint { get; set; }
    public Geometry ToGeometry() {
        GeometryGroup gg = new();
        LineGeometry lg = new() {
            StartPoint = StartPoint.Point,
            EndPoint = EndPoint.Point
        };
        gg.Children.Add(StartPoint.ToGeometry());
        gg.Children.Add(lg);
        gg.Children.Add(EndPoint.ToGeometry());
        return gg;
    }



    public Shape ToShape() {
        return new Path {
            Data = ToGeometry(),
            Stroke = Brushes.Black,
            StrokeThickness = 4,
        };

        // return new Line() {
        //     X1 = StartPoint.Point.X,
        //     X2 = EndPoint.Point.X,
        //
        //     Y1 = StartPoint.Point.Y,
        //     Y2 = EndPoint.Point.Y,
        // };
    }
}