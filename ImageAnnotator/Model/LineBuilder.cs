using System;

namespace ImageAnnotator.Model;

public class LineBuilder {
    private DoublePoint? _startPoint;
    private DoublePoint? _endPoint;

    public LineBuilder WithStart(DoublePoint point) {
        _startPoint = point;
        return this;
    }

    public LineBuilder WithEnd(DoublePoint point) {
        _endPoint = point;
        return this;
    }

    public LineAnnotation Build() {
        if (_startPoint is null) {
        }

        if (_endPoint is null) {
        }
        throw new NotImplementedException();
    }
}