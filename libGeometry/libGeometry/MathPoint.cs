namespace libGeometry;

/// <summary>
/// A class that can be used to represent a point. This may a general n-dimensional
/// point
/// </summary>
public class MathPoint {
    /// <summary>
    /// The name of coordinate system where the point bellongs to.
    /// </summary>
    public string? CoordinateSystemName { get; set; }

    /// <summary>
    /// The set of numbers that define the coordinates of a point.
    /// </summary>
    public required double[] Coordinates { get; set; }

    /// <summary>
    /// Gets the dimension of the point
    /// </summary>
    public int Dimension => Coordinates.Length;

    /// <summary>
    /// The [ ] operator for setting and getting the values.:w
    /// </summary>
    public double this[int index] {
        get => Coordinates[index];
        set => Coordinates[index] = value;
    }

    /// <summary>
    /// Returns a new MathPoint that is the sum of this point and the given argument
    /// </summary>
    public MathPoint Add(MathPoint other) {
        if (Dimension != other.Dimension) {
            throw new ArgumentException("Vectors must have the same dimensions to be added.");
        }

        double[] result = new double[Dimension];
        for (int i = 0; i < Dimension; i++) {
            result[i] = this[i] + other[i];
        }

        return new MathPoint() {
            Coordinates = result
        };
    }

    /// <summary>
    /// Returns a new point with coordinates subtracted.
    /// </summary>
    public MathPoint Subtract(MathPoint other) {
        if (Dimension != other.Dimension) {
            throw new ArgumentException("Vectors must have the same dimensions to be added.");
        }

        double[] result = new double[Dimension];
        for (int i = 0; i < Dimension; i++) {
            result[i] = this[i] - other[i];
        }

        return new MathPoint() {
            Coordinates = result
        };
    }

    /// <summary>
    /// Converts a math point to a vertical matrix
    /// </summary>
    public Matrix ToMatrix() {

        //Set the matrix values
        double[,] data = new double[Coordinates.Length, 1];
        int r = 0;
        foreach (double v in Coordinates) {
            data[r++, 0] = v;
        }

        Matrix m = new(rows: Coordinates.Length, columns: 1) {
            Data = data
        };

        return m;
    }
}