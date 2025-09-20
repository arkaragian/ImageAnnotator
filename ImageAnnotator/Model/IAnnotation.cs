using libGeometry;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;

/// <summary>
/// An interface for annotations
/// </summary>
public interface IAnnotation {
    /// <summary>
    /// The name of the annotation. That is the name that is used by the application.
    /// </summary>
    string Name { get; set; }

    List<MathPoint> SnapPoints { get; }

    /// <summary>
    /// Recalculates the coordinates based on a new control size.
    /// </summary>
    void ResizeCoordinates(DoubleSize newSize);

    /// <summary>
    /// Converts the annotation to a geometry that can be used for rendering
    /// the data on screen.
    /// </summary>
    Geometry ToGeometry();

    /// <summary>
    /// Converts the annotation to a drawable shape that can be placed on a
    /// canvas
    /// </summary>
    Shape ToShape();

    /// <summary>
    /// Converts the annotation to tikz code.
    /// </summary>
    /// <param name="identation">
    /// Represents the itendation that may preceede the code in terms of
    /// number of spaces.
    /// </param>
    string ToCode(uint? identation);


    void Translate(int xTranslation, int yTranslation, DoubleSize size);
}