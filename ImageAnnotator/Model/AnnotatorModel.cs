using ImageAnnotator.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;

/// <summary>
/// The image model for the image that is loaded
/// </summary>
public class AnnotatorModel {
    /// <summary>
    /// The path of the image that is edited.
    /// </summary>
    public string? ImagePath { get; set; }

    /// <summary>
    /// The data of the image that is being annotated
    /// </summary>
    public System.Drawing.Bitmap? Image { get; set; }

    /// <summary>
    /// The list of annotations
    /// </summary>
    public List<IAnnotation> Annotations { get; private set; } = new();

    public void InsertNode(DoublePoint point) {
        NodeAnnotation na = new() {
            Point = point
        };
        Annotations.Add(na);
    }
}

/// <summary>
/// An interface for annotations
/// </summary>
public interface IAnnotation {
    Drawing ToDrawing();
    Shape ToShape();
}

public class LineAnnotation : IAnnotation {
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }
    public Drawing ToDrawing() {
        Point sp = new() { X = StartPoint.X, Y = StartPoint.Y };
        Point ep = new() { X = EndPoint.X, Y = EndPoint.Y };
        return new GeometryDrawing() {
            Brush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
            Pen = new(Brushes.Black, 2.0),
            Geometry = new LineGeometry(sp, ep)
        };
    }

    public Shape ToShape() {
        throw new System.NotImplementedException();
    }
}

public class NodeAnnotation : IAnnotation {
    public DoublePoint Point { get; set; }

    public Drawing ToDrawing() {
        EllipseGeometry eg = new() {
            Center = Point,
            RadiusX = 5,
            RadiusY = 5,
        };
        return new GeometryDrawing() {
            Brush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
            Pen = new(Brushes.Black, 2.0),
            Geometry = eg
        };
    }

    public Shape ToShape() {
        return new Ellipse() {
            Stroke = Brushes.Black,
            Fill = Brushes.DarkBlue,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Width = 5,
            Height = 5,
        };
    }
}

public class RectangleAnnotation : IAnnotation {
    public Drawing ToDrawing() {
        throw new System.NotImplementedException();
    }

    public Shape ToShape() {
        throw new System.NotImplementedException();
    }
}