using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;

namespace ImageAnnotator.Model;

/// <summary>
/// The image model for the image that is loaded
/// </summary>
public class ImageModel {
    /// <summary>
    /// The path of the image that is edited.
    /// </summary>
    public string? ImagePath { get; set; }

    /// <summary>
    /// The data of the image that is being annotated
    /// </summary>
    public Bitmap? Image { get; set; }

    /// <summary>
    /// The list of annotations
    /// </summary>
    public List<IAnnotation> Annotations { get; private set; } = new();

    public void AddAnotation(IAnnotation a) {
        Annotations.Add(a);
    }
}

/// <summary>
/// An interface for annotations
/// </summary>
public interface IAnnotation {
    Drawing ToDrawing();
}

public class LineAnnotation : IAnnotation {
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }
    public Drawing ToDrawing() {
        System.Windows.Point sp = new() { X = StartPoint.X, Y = StartPoint.Y };
        System.Windows.Point ep = new() { X = EndPoint.X, Y = EndPoint.Y };
        return new GeometryDrawing() {
            Brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0)),
            Pen = new(System.Windows.Media.Brushes.Black, 2.0),
            Geometry = new LineGeometry(sp, ep)
        };
    }
}

public class RectangleAnnotation : IAnnotation {
    public void ToDrawing() {
        throw new System.NotImplementedException();
    }
}