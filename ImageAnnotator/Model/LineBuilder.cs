using ImageAnnotator.Tikz;

namespace ImageAnnotator.Model;

/// <summary>
/// Builds a line
/// </summary>
public class LineBuilder {

    public required DoubleSize DrawingRegion { get; set; }

    private DoublePoint? _startPoint;
    private DoublePoint? _endPoint;

    public bool HasStartPoint => _startPoint is not null;
    public bool HasEndPoint => _endPoint is not null;

    public LineBuilder WithStartPoint(DoublePoint point) {
        _startPoint = point;
        return this;
    }

    public LineBuilder WithEndPoint(DoublePoint point) {
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

        LineAnnotation result = new() {
            StartPoint = new NodeAnnotation() {
                NodeImageCoordinates = _startPoint.Value,
                NormalizedCoordinates = TransformCoordinates.ToTikzCoordinates(_startPoint.Value, DrawingRegion)
            },
            EndPoint = new NodeAnnotation {
                NodeImageCoordinates = _endPoint.Value,
                NormalizedCoordinates = TransformCoordinates.ToTikzCoordinates(_endPoint.Value, DrawingRegion)
            },
            Name = a_name
        };

        _startPoint = null;
        _endPoint = null;

        return result;
    }
}