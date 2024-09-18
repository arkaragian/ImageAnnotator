using libGeometry;

namespace ImageAnnotator.Model;

/// <summary>
/// A class that resolves which point is the Upper left and lower right
/// from a random set of two points.
/// </summary>
public class RectanglePointResolver {

    public static MathPoint UpperLeftPoint(MathPoint pointA, MathPoint pointB) {
        if (pointA[0] < pointB[0]) {
            if (pointA[1] < pointB[1]) {
                return pointA;
            } else {
                return new MathPoint() {
                    Coordinates = [
                        pointA[0],
                        pointB[1]
                    ]
                };
            }
        } else {
            if (pointB[1] < pointA[1]) {
                return pointB;
            } else {
                return new MathPoint() {
                    Coordinates = [
                        pointB[0],
                        pointA[1]
                    ]
                };
            }
        }
    }

    public static MathPoint LowerRightPoint(MathPoint pointA, MathPoint pointB) {

        if (pointA[1] > pointB[1]) {
            if (pointA[0] > pointB[0]) {
                return pointA;
            } else {
                return new MathPoint() {
                    Coordinates = [
                        pointB[0],
                        pointA[1]
                    ]
                };
            }
        } else {
            if (pointB[0] > pointA[0]) {
                return pointB;
            } else {
                return new MathPoint() {
                    Coordinates = [pointA[0], pointB[1]]
                };
            }
        }
    }
}