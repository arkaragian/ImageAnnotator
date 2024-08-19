using ImageAnnotator.Tikz;
using libGeometry;

namespace ImageAnnotator.Model;

/// <summary>
/// Builds a line
/// </summary>
public class RectangleBuilder {
    //The points bellow have no notion on which corner of the rectangle
    //they reside on. This is determined based on their coordinates.
    public required DoubleSize DrawingRegion { get; set; }

    //public required CoordinatesTransformer CoordinateTransformer { get; set; }

    private MathPoint? _pointA;
    private MathPoint? _pointB;

    public bool HasAllRequiredPoints => _pointA is not null && _pointB is not null;

    public RectangleBuilder AddPoint(MathPoint point) {
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

        MathPoint UpperLeft;
        //Determine Upper Left Point
        if (_pointA[0] < _pointB[0]) {
            if (_pointA[1] < _pointB[1]) {
                UpperLeft = _pointA;
            } else {
                UpperLeft = new MathPoint() {
                    Coordinates = new double[] {
                        _pointA[0],
                        _pointA[1]
                    }
                };
            }
        } else {
            if (_pointB[1] < _pointA[1]) {
                UpperLeft = _pointB;
            } else {
                UpperLeft = new MathPoint() {
                    Coordinates = new double[] {
                        _pointB[0],
                        _pointA[1]
                    }
                };
            }
        }

        MathPoint LowerRight;

        if (_pointA[1] > _pointB[1]) {
            if (_pointA[0] > _pointB[0]) {
                LowerRight = _pointA;
            } else {
                LowerRight = new MathPoint() {
                    Coordinates = new double[] {
                        _pointB[0],
                        _pointA[1]
                    }
                };
            }
        } else {
            if (_pointB[0] > _pointA[0]) {
                LowerRight = _pointB;
            } else {
                LowerRight = new MathPoint() {
                    Coordinates = new double[] {
                        _pointA[0],
                        _pointB[1]
                    }
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
                //NormalizedCoordinates = TransformCoordinates.ToTikzCoordinates(UpperLeft, DrawingRegion)
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