using libGeometry;

namespace ImageAnnotator.Model;

/// <summary>
/// Builds a line
/// </summary>
public class LineBuilder {

    public required DoubleSize DrawingRegion { get; set; }

    public required CoordinatesTransformer Transformer { get; set; }

    private MathPoint? _startPoint;
    private MathPoint? _endPoint;


    public MathPoint? StartPoint {
        get {
            return _startPoint;
        }
    }

    public MathPoint? EndPoint => _startPoint;
    public bool HasStartPoint => _startPoint is not null;
    public bool HasEndPoint => _endPoint is not null;

    public LineBuilder WithStartPoint(MathPoint point) {
        _startPoint = point;
        return this;
    }

    public LineBuilder WithEndPoint(MathPoint point) {
        _endPoint = point;
        return this;
    }

    public LineAnnotation? Build(int? annotationCounter) {
        if (_startPoint is null) {
            return null;
        }

        if (_endPoint is null) {
            return null;
        }

        string a_name;
        if (annotationCounter is null) {
            a_name = "Line";
        } else {
            a_name = $"Line {annotationCounter}";
        }

        MathPoint StartNormalized = new() {
            Coordinates = new double[] {
                _startPoint[0]/DrawingRegion.Width,
                _startPoint[1]/DrawingRegion.Height,
            }
        };

        MathPoint EndNormalized = new() {
            Coordinates = new double[] {
                _endPoint[0]/DrawingRegion.Width,
                _endPoint[1]/DrawingRegion.Height,
            }
        };

        LineAnnotation result = new() {
            StartPoint = new() {
                NodeImagePoint = _startPoint,
                NodeImageNormalizedPoint = StartNormalized,
                NodeTikzPoint = Transformer.TransformToSecondarySystem(StartNormalized)
            },
            EndPoint = new() {
                NodeImagePoint = _endPoint,
                NodeImageNormalizedPoint = EndNormalized,
                NodeTikzPoint = Transformer.TransformToSecondarySystem(EndNormalized)
            },
            Name = a_name
        };

        _startPoint = null;
        _endPoint = null;

        return result;
    }
}