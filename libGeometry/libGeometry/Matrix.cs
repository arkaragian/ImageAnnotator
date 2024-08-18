using System.Globalization;

namespace libGeometry;

/// <summary>
/// Defines a 2d matrix
/// </summary>
public class Matrix {
    public required double[,] Data { get; set; }
    public int Rows { get; }
    public int Columns { get; }

    // Constructor to initialize the matrix with given rows and columns
    public Matrix(int rows, int columns) {
        Rows = rows;
        Columns = columns;
        //Data = new double[rows, columns];
    }

    // Indexer to access matrix elements
    public double this[int row, int col] {
        get => Data[row, col];
        set => Data[row, col] = value;
    }

    // Matrix addition
    public static Matrix operator +(Matrix a, Matrix b) {
        if (a.Rows != b.Rows || a.Columns != b.Columns) {
            throw new InvalidOperationException("Matrices must have the same dimensions for addition.");
        }

        double[,] data = new double[a.Rows, a.Columns];
        for (int i = 0; i < a.Rows; i++) {
            for (int j = 0; j < a.Columns; j++) {
                data[i, j] = a[i, j] + b[i, j];
            }
        }

        Matrix result = new(a.Rows, a.Columns) {
            Data = data
        };

        return result;
    }

    // Matrix subtraction
    public static Matrix operator -(Matrix a, Matrix b) {
        if (a.Rows != b.Rows || a.Columns != b.Columns) {
            throw new InvalidOperationException("Matrices must have the same dimensions for subtraction.");
        }

        double[,] data = new double[a.Rows, a.Columns];
        for (int i = 0; i < a.Rows; i++) {
            for (int j = 0; j < a.Columns; j++) {
                data[i, j] = a[i, j] - b[i, j];
            }
        }

        Matrix result = new(a.Rows, a.Columns) {
            Data = data
        };
        return result;
    }

    // Matrix multiplication
    public static Matrix operator *(Matrix a, Matrix b) {
        if (a.Columns != b.Rows) {
            string template = "Matrices must have compatible dimensions for multiplication. Left Matrix Columns = {0} Right Matrix Rows = {0}";
            string message = string.Format(CultureInfo.InvariantCulture, template, a.Columns, b.Rows);
            throw new InvalidOperationException(message);
        }

        double[,] data = new double[a.Rows, a.Columns];
        for (int i = 0; i < a.Rows; i++) {
            for (int j = 0; j < b.Columns; j++) {
                data[i, j] = 0;
                for (int k = 0; k < a.Columns; k++) {
                    data[i, j] += a[i, k] * b[k, j];
                }
            }
        }

        Matrix result = new(a.Rows, b.Columns) {
            Data = data
        };

        return result;
    }

    public MathPoint? ToMathPoint() {
        if (Rows is not 1 && Columns is not 1) {
            return null;
        }

        int size = Rows is 1 ? Columns : Rows;

        double[] coordinates = new double[size];

        if (Rows is 1) {
            for (int i = 0; i < Columns; i++) {
                coordinates[i] = Data[0, i];
            }
        } else {
            for (int i = 0; i < Rows; i++) {
                coordinates[i] = Data[i, 0];
            }
        }

        return new MathPoint {
            Coordinates = coordinates
        };

    }

    // Method to print the matrix
    public void Print() {
        for (int i = 0; i < Rows; i++) {
            for (int j = 0; j < Columns; j++) {
                Console.Write(Data[i, j] + "\t");
            }
            Console.WriteLine();
        }
    }

}

// // Main method for demonstration
// public static void Main(string[] args) {
//     var matrix1 = new Matrix(2, 2);
//     matrix1[0, 0] = 1;
//     matrix1[0, 1] = 2;
//     matrix1[1, 0] = 3;
//     matrix1[1, 1] = 4;
//
//     var matrix2 = new Matrix(2, 2);
//     matrix2[0, 0] = 5;
//     matrix2[0, 1] = 6;
//     matrix2[1, 0] = 7;
//     matrix2[1, 1] = 8;
//
//     var sum = matrix1 + matrix2;
//     var diff = matrix1 - matrix2;
//     var product = matrix1 * matrix2;
//
//     Console.WriteLine("Matrix 1:");
//     matrix1.Print();
//
//     Console.WriteLine("Matrix 2:");
//     matrix2.Print();
//
//     Console.WriteLine("Sum:");
//     sum.Print();
//
//     Console.WriteLine("Difference:");
//     diff.Print();
//
//     Console.WriteLine("Product:");
//     product.Print();
// }