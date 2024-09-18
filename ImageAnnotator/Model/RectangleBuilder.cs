using libGeometry;

namespace ImageAnnotator.Model;

/// <summary>
/// Builds a line
/// </summary>
public class RectangleBuilder {
    //The points bellow have no notion on which corner of the rectangle
    //they reside on. This is determined based on their coordinates.
    public required DoubleSize DrawingRegion { get; set; }

    public required CoordinatesTransformer Transformer { get; set; }

    /// <summary>
    /// An image point
    /// </summary>
    public MathPoint? PointA { get; set; }

    /// <summary>
    /// An image point
    /// </summary>
    private MathPoint? _pointB;

    public bool HasAllRequiredPoints => PointA is not null && _pointB is not null;

    public RectangleBuilder AddPoint(MathPoint point) {
        if (PointA is null) {
            PointA = point;
        } else {
            _pointB = point;
        }
        return this;
    }


    public RectangleAnnotation? Build(int? annotationCounter) {
        if (PointA is null) {
            return null;
        }

        if (_pointB is null) {
            return null;
        }

        MathPoint UpperLeft = RectanglePointResolver.UpperLeftPoint(PointA, _pointB);
        MathPoint LowerRight = RectanglePointResolver.LowerRightPoint(PointA, _pointB);

        string a_name;
        if (annotationCounter is null) {
            a_name = "Rect";
        } else {
            a_name = $"Rect {annotationCounter}";
        }


        MathPoint UpperLeftNodeImageNormalizedPoint = new() {
            Coordinates = [
                UpperLeft[0]/DrawingRegion.Width,
                UpperLeft[1]/DrawingRegion.Height,
            ]
        };

        MathPoint LowerRigttNodeImageNormalizedPoint = new() {
            Coordinates = [
                LowerRight[0]/DrawingRegion.Width,
                LowerRight[1]/DrawingRegion.Height,
            ]
        };

        RectangleAnnotation result = new() {
            UpperLeftNode = new() {
                NodeImagePoint = UpperLeft,
                NodeImageNormalizedPoint = UpperLeftNodeImageNormalizedPoint,
                NodeTikzPoint = Transformer.TransformToSecondarySystem(UpperLeftNodeImageNormalizedPoint)
            },
            LowerRightNode = new() {
                NodeImagePoint = LowerRight,
                NodeImageNormalizedPoint = LowerRigttNodeImageNormalizedPoint,
                NodeTikzPoint = Transformer.TransformToSecondarySystem(LowerRigttNodeImageNormalizedPoint)
            },
            Name = a_name
        };

        PointA = null;
        _pointB = null;

        return result;
    }
}