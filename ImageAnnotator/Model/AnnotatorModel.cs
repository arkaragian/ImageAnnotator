using System.Collections.Generic;

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

    /// <summary>
    /// Inserts a node to the list og annotations
    /// </summary>
    public void InsertNode(DoublePoint point) {
        NodeAnnotation na = new() {
            Point = point
        };
        Annotations.Add(na);
    }
}


