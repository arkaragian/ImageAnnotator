using ImageAnnotator.Model;

namespace ImageAnnotator.Tikz;

public static class TransformCoordinates {
    /// <summary>
    /// Normalizes the values of the point to the values of the
    /// </summary>
    public static DoublePoint ToTikzCoordinates(DoublePoint point, DoubleSize controlSize) {
        //The Zero of the image is the top left. For for tikz is the bottom left.
        DoublePoint normalizedPoint = new() {
            X = point.X / controlSize.Width,
            Y = (point.Y / controlSize.Height * -1) + 1
        };

        return normalizedPoint;
    }
}