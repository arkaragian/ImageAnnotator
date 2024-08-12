namespace ImageAnnotator.Model;

public class LineBuilder {
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

    public LineAnnotation? Build() {
        if (_startPoint is null) {
            return null;
        }

        if (_endPoint is null) {
            return null;
        }

        LineAnnotation result = new() {
            StartPoint = _startPoint.Value,
            EndPoint = _endPoint.Value,
        };

        _startPoint = null;
        _endPoint = null;

        return result;
    }
}