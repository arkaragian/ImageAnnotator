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
    /// The image data
    /// </summary>
    public Bitmap? Image { get; set; }
}