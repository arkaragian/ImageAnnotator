namespace libGeometry;

public class Vector {
    /// <summary>
    /// The coordinates that define the vector
    /// </summary>
    public required double[] Coordinates { get; set; }

    /// <summary>
    /// Returns the dimension of the vector
    /// </summary>
    public int Dimensions => Coordinates.Length;

    /// <summary>
    /// Basic constructor that may accept any number of arguments.
    /// </summary>
    /// <param name="coordinates">The coordinates that
    // public Vector(params double[] coordinates) {
    //     if (coordinates is null || coordinates.Length is 0) {
    //         throw new ArgumentException("A vector must have at least one dimension.");
    //     }
    //
    //     Coordinates = new double[coordinates.Length];
    //     Array.Copy(coordinates, Coordinates, coordinates.Length);
    // }
    //
    public Vector() {
    }

    /// <summary>
    /// Creates a vector whose coordinates are calculated from a start and an end point.
    /// </summary>
    public Vector(MathPoint start, MathPoint end) {
        if (start.Dimension != end.Dimension) {
            throw new ArgumentException("Point dimensions should be equal");
        }

        Coordinates = new double[start.Dimension];
        for (int i = 0; i < start.Dimension; i++) {
            Coordinates[i] = end[i] - start[i];
        }
    }

    /// <summary>
    /// Creates a vector assuming that the start point is a zero point.
    /// </summary>
    public Vector(MathPoint point) {
        Coordinates = new double[point.Dimension];
        Array.Copy(point.Coordinates, Coordinates, point.Coordinates.Length);
    }

    /// <summary>
    /// The [ ] operator for setting and getting the values.:w
    /// </summary>
    public double this[int index] {
        get => Coordinates[index];
        set => Coordinates[index] = value;
    }

    /// <summary>
    /// The magnitude of the vector.
    /// </summary>
    public double Magnitude() {
        //is the square root along all the
        return Math.Sqrt(Coordinates.Sum(e => e * e));
    }

    /// <summary>
    /// Finds an angle between vectors. This assumes the the vactors are 2D vectors.
    /// </summary>
    public double Find2DAngle(Vector other) {
        if (Dimensions is not 2) {
            throw new ArgumentException("Vectors must have the same dimensions in order for the angle to be calculated.");
        }

        if (other.Dimensions is not 2) {
            throw new ArgumentException("Vectors must have the same dimensions in order for the angle to be calculated.");
        }

        double numerator = (Coordinates[0] * other.Coordinates[0]) + (Coordinates[1] * other.Coordinates[1]);
        double denominator = Magnitude() * other.Magnitude();

        return Math.Acos(numerator / denominator);
    }

    /// <summary>
    /// Returns a new vector that is the sum of this vector and the given argument
    /// </summary>
    public Vector Add(Vector other) {
        if (Dimensions != other.Dimensions) {
            throw new ArgumentException("Vectors must have the same dimensions to be added.");
        }

        double[] result = new double[Dimensions];
        for (int i = 0; i < Dimensions; i++) {
            result[i] = this[i] + other[i];
        }

        return new Vector() {
            Coordinates = result
        };
    }

    /// <summary>
    /// Returns a new vector that is the difference of this vector and the given
    /// argument
    /// </summary>
    public Vector Subtract(Vector other) {
        if (Dimensions != other.Dimensions) {
            throw new ArgumentException("Vectors must have the same dimensions to be subtracted.");
        }

        double[] result = new double[Dimensions];
        for (int i = 0; i < Dimensions; i++) {
            result[i] = this[i] - other[i];
        }

        return new Vector() {
            Coordinates = result
        };
    }

    /// <summary>
    /// Indicates if this vector has the same coordinates as the other vector
    /// </summary>
    public bool HaveSameCoordinatesAs(Vector other) {
        if (Coordinates.Length != other.Coordinates.Length) {
            return false;
        }

        for (int i = 0; i < Coordinates.Length; i++) {
            if (Coordinates[i] == other.Coordinates[i]) {
                continue;
            } else {
                return false;
            }
        }

        return true;
    }

    public double DotProduct(Vector other) {
        if (Dimensions != other.Dimensions) {
            throw new ArgumentException("Vectors must have the same dimensions to calculate the dot product.");
        }

        double result = 0;
        for (int i = 0; i < Dimensions; i++) {
            result += this[i] * other[i];
        }

        return result;
    }

    /// <summary>
    /// Returns a new vector that is the result of scalar multiplication with
    /// all the coordinates of the vector.
    /// </summary>
    public Vector ScalarMultiply(double scalar) {
        double[] result = new double[Dimensions];
        for (int i = 0; i < Dimensions; i++) {
            result[i] = this[i] * scalar;
        }

        return new Vector() {
            Coordinates = result
        };
    }

    public override string ToString() {
        return $"[{string.Join(", ", Coordinates)}]";
    }

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

// public class Program {
//     public static void Main() {
//         // Create two 3D vectors
//         Vector v1 = new Vector(1, 2, 3);
//         Vector v2 = new Vector(4, 5, 6);
//
//         // Add the vectors
//         Vector v3 = v1.Add(v2);
//         Console.WriteLine($"v1 + v2 = {v3}");
//
//         // Subtract the vectors
//         Vector v4 = v1.Subtract(v2);
//         Console.WriteLine($"v1 - v2 = {v4}");
//
//         // Dot product
//         double dotProduct = v1.DotProduct(v2);
//         Console.WriteLine($"v1 • v2 = {dotProduct}");
//
//         // Scalar multiplication
//         Vector v5 = v1.ScalarMultiply(2);
//         Console.WriteLine($"v1 * 2 = {v5}");
//
//         // Magnitude of v1
//         double magnitude = v1.Magnitude();
//         Console.WriteLine($"|v1| = {magnitude}");
//     }
// }