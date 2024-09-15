namespace libGeometry;

/// <summary>
/// A class that manages transformation between two coordinate systems
/// </summary>
public class CoordinatesTransformer {
    /// <summary>
    /// The coordinate system based on which all other coordinate systems
    /// are defined. This is the world coordinate system.
    /// </summary>
    public required CoordinateSystem RootSystem { get; init; }

    /// <summary>
    /// A second coordinate system that is different than the root system.
    /// </summary>
    public required CoordinateSystem SecondarySystem { get; set; }

    /// <summary>
    /// Recalculates the input point coordinates based on the location of the
    /// secondary system
    /// </summary>
    private MathPoint TranslatePoint(MathPoint point) {

        if (SecondarySystem.Location is null) {
            throw new InvalidOperationException("Target system should have defined");
        }


        return point.Subtract(SecondarySystem.Location);

        //To translate we just use the location of the new coordinate system and
        //add it's components to the input point.

        //https://www.songho.ca/math/homogeneous/homogeneous.html
        //
        //The common notation is to use w=1 for points and w=0 for vectors.
        //The reason is that points can be translated but vectors cannot. You
        //can change the length of a vector or its direction but all vectors
        //with the same length/direction are considered equal, regardless their
        //"starting position". So you can simply use the origin for all vectors.
        //Setting w=0 and multiplying the translation matrix by the vector will
        //result in the same vector.
        //Matrix translation_matrix = new(input.Dimension + 1, 1);
    }

    // private static Matrix GetRotationMatrixByDegree(double degangle) {
    //     double angle = degangle * Math.PI / 180;
    //     return GetRotationMatrixByRadian(angle);
    // }

    private static Matrix GetRotationMatrixByRadian(double angle) {
        //Assumes that clockwise is the positive direction
        Matrix rotation_matrix = new(rows: 2, columns: 2) {
            Data = new double[2, 2] {
                { Math.Cos(angle), Math.Sin(angle)},
                { -Math.Sin(angle), Math.Cos(angle)},
            }
        };
        return rotation_matrix;
    }

    /// <summary>
    /// Indicates if the secondary system is a reflection of the root system
    /// in any axis. If there is a reflection the function will also return the
    /// direction index of the reflected axis.
    /// </summary>
    private (bool reflected, List<int>? axis) IsReflected() {
        if (RootSystem.Dimension != SecondarySystem.Dimension) {
            return (false, null);
        }
        List<int> result = [];
        for (int i = 0; i < RootSystem.Dimension; i++) {
            double angle = RootSystem.DirectionVectors[i].Find2DAngle(SecondarySystem.DirectionVectors[i]);
            if (Math.Abs(angle - Math.PI) < 0.001) {
                result.Add(i);
            }
        }
        if (result.Count > 0) {
            return (true, result);
        }
        return (false, null);
    }

    /// <summary>
    /// Calculates the rotation matrix between two coordinate systems
    /// <remarks>
    ///     Assumes that the systems are orthocanonical.
    /// </remarks>
    /// </summary>
    private Matrix Calculate2DRotationMatrix() {
        if (RootSystem.Dimension is not 2) {
            throw new InvalidOperationException("Root system is not 2D");
        }

        if (SecondarySystem.Dimension is not 2) {
            throw new InvalidOperationException("Target system is not 2D");
        }

        //Check to see if we have reflected systems. Because reflection cannot be
        //represented as a rotation. At least before we move to more exotic mathematics.
        //Eg reflection along the x axis would assume a rotation of 180 deg around the
        //x axis. But the 2d rotation matrix is along the z axis.

        //Assuming that the coordinate system is orthocanonical we only need to
        //check a single vector.

        Vector root_direction = RootSystem.DirectionVectors[0];
        Vector target_direction = SecondarySystem.DirectionVectors[0];

        if (root_direction.HaveSameCoordinatesAs(target_direction)) {
            if (RootSystem.DirectionVectors[1].HaveSameCoordinatesAs(SecondarySystem.DirectionVectors[1])) {
                return GetRotationMatrixByRadian(0.0);
            } else {
                int sign = Math.Sign(RootSystem.DirectionVectors[1].Coordinates[1]) * Math.Sign(SecondarySystem.DirectionVectors[1].Coordinates[1]);
                double angle_rad = RootSystem.DirectionVectors[1].FindSigned2DAngle(SecondarySystem.DirectionVectors[1]);
                Console.WriteLine("Angle Rad = {0}", angle_rad);
                return GetRotationMatrixByRadian(angle_rad);
            }
        } else {
            double angle_rad = root_direction.FindSigned2DAngle(target_direction);
            return GetRotationMatrixByRadian(angle_rad);
        }
    }


    /// <summary>
    /// Calculates a reflection matrix based on the index of the reflected direction vector.
    /// </summary>
    private static Matrix CalculateReflectionMatrix(List<int> reflectionIndex) {
        Matrix m = new(rows: 2, columns: 1) {
            Data = new double[2, 1]{
                {1},
                {1}
            }
        };

        foreach (int index in reflectionIndex) {
            m.Data[index, 0] = -1;
        }


        return m;
    }

    /// <summary>
    /// Transforms a point that was defined in the root coordinate system to it's target system
    /// representation.
    /// </summary>
    public MathPoint TransformToSecondarySystem(MathPoint point) {
        point.ToMatrix().Print();
        Console.WriteLine();

        point = TranslatePoint(point);

        (bool reflected, List<int>? axis) = IsReflected();
        if (reflected) {
            //TODO: Handle reflection here axis determines a transformwation matrix
            //We do a hadamand product for this matrix.
            if (axis is null) {
                throw new InvalidOperationException("Reflected system did not provide axis");
            }
            Matrix reflectionMatrix = CalculateReflectionMatrix(axis);
            Matrix reflectedPoint = point.ToMatrix().ElementWisePriduct(reflectionMatrix);
            MathPoint? temp2 = reflectedPoint.ToMathPoint();

            if (temp2 is null) {
                throw new InvalidOperationException("Conversion to math point resulted in null value!");
            } else {
                return temp2;
            }
        }

        Matrix m = Calculate2DRotationMatrix() * point.ToMatrix();
        MathPoint? temp = m.ToMathPoint();

        if (temp is null) {
            throw new InvalidOperationException("Conversion to math point resulted in null value!");
        } else {
            return temp;
        }
    }

    /// <summary>
    /// Transforms a point that was defined in the root coordinate system to it's target system
    /// representation.
    /// </summary>
    // public CoordinatesTransformer RotatePointByAngle(double angle) {
    //
    //     if (Point is null) {
    //         return this;
    //     }
    //     Matrix m = GetRotationMatrixByDegree(angle) * Point.ToMatrix();
    //
    //     m.Print();
    //
    //     MathPoint? temp = m.ToMathPoint();
    //
    //     if (temp is null) {
    //         throw new InvalidOperationException("Conversion to math point resulted in null value!");
    //     } else {
    //         Point = temp;
    //     }
    //
    //     return this;
    //
    //
    //
    //     //To translate we just use the location of the new coordinate system and
    //     //add it's components to the input point.
    //
    //     //https://www.songho.ca/math/homogeneous/homogeneous.html
    //     //
    //     //The common notation is to use w=1 for points and w=0 for vectors.
    //     //The reason is that points can be translated but vectors cannot. You
    //     //can change the length of a vector or its direction but all vectors
    //     //with the same length/direction are considered equal, regardless their
    //     //"starting position". So you can simply use the origin for all vectors.
    //     //Setting w=0 and multiplying the translation matrix by the vector will
    //     //result in the same vector.
    //     //Matrix translation_matrix = new(input.Dimension + 1, 1);
    // }
}