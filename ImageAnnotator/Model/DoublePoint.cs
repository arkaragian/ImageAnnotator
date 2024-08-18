namespace ImageAnnotator.Model;


/// <summary>
/// A simple struct to represent a point with double prescision in order
/// to not mess with windows point and drawing points etc.
/// </summary>
public struct DoublePoint {
    public required double X { get; set; }
    public required double Y { get; set; }


    public static implicit operator DoublePoint(System.Windows.Point v) {
        return new DoublePoint() {
            X = v.X,
            Y = v.Y
        };
    }


    public static implicit operator System.Windows.Point(DoublePoint v) {
        return new System.Windows.Point() {
            X = v.X,
            Y = v.Y
        };
    }
}