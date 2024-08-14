using ImageAnnotator.Tikz;

namespace ImageAnnotator.Model;

/// <summary>
/// Builds a line
/// </summary>
public class RectangleBuilder {
    //The points bellow have no notion on which corner of the rectangle
    //they reside on. This is determined based on their coordinates.
    public required DoubleSize DrawingRegion { get; set; }

    private DoublePoint? _pointA;
    private DoublePoint? _pointB;

    public bool HasAllRequiredPoints => _pointA is not null && _pointB is not null;

    public RectangleBuilder AddPoint(DoublePoint point) {
        if (_pointA is null) {
            _pointA = point;
        } else {
            _pointB = point;
        }
        return this;
    }


    public RectangleAnnotation? Build(int? annotationCounter) {
        if (_pointA is null) {
            return null;
        }

        if (_pointB is null) {
            return null;
        }

        DoublePoint UpperLeft;
        //Determine Upper Left Point
        if (_pointA.Value.X < _pointB.Value.X) {
            if (_pointA.Value.Y < _pointB.Value.Y) {
                UpperLeft = _pointA.Value;
            } else {
                UpperLeft = new DoublePoint() {
                    X = _pointA.Value.X,
                    Y = _pointB.Value.Y
                };
            }
        } else {
            if (_pointB.Value.Y < _pointA.Value.Y) {
                UpperLeft = _pointB.Value;
            } else {
                UpperLeft = new DoublePoint() {
                    X = _pointB.Value.X,
                    Y = _pointA.Value.Y
                };
            }
        }

        DoublePoint LowerRight;

        if (_pointA.Value.Y > _pointB.Value.Y) {
            if (_pointA.Value.X > _pointB.Value.X) {
                LowerRight = _pointA.Value;
            } else {
                LowerRight = new DoublePoint() {
                    X = _pointB.Value.X,
                    Y = _pointA.Value.Y
                };
            }
        } else {
            if (_pointB.Value.X > _pointA.Value.X) {
                LowerRight = _pointB.Value;
            } else {
                LowerRight = new DoublePoint() {
                    X = _pointA.Value.X,
                    Y = _pointB.Value.Y
                };
            }
        }

        string a_name;
        if (annotationCounter is null) {
            a_name = "Rect";
        } else {
            a_name = $"Rect {annotationCounter}";
        }


        RectangleAnnotation result = new() {
            UpperLeftNode = new NodeAnnotation() {
                NodeImageCoordinates = UpperLeft,
                NormalizedCoordinates = TransformCoordinates.ToTikzCoordinates(UpperLeft, DrawingRegion)
            },
            LowerRightNode = new NodeAnnotation() {
                NodeImageCoordinates = LowerRight,
                NormalizedCoordinates = TransformCoordinates.ToTikzCoordinates(LowerRight, DrawingRegion)
            },
            Name = a_name
        };

        _pointA = null;
        _pointB = null;

        return result;
    }
}