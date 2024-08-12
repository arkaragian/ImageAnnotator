using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;

/// <summary>
/// An interface for annotations
/// </summary>
public interface IAnnotation {
    /// <summary>
    /// The name of the annotation
    /// </summary>
    string Name { get; set; }

    Geometry ToGeometry();

    /// <summary>
    /// Converts the annotation to a drawable shape that can be placed on a
    /// canvas
    /// </summary>
    Shape ToShape();
}