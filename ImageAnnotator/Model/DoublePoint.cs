namespace ImageAnnotator.Model;

public struct DoublePoint {
    public double X { get; set; }
    public double Y { get; set; }


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