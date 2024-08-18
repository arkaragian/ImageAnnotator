namespace libGeometry;

public class CoordinatesTransformer {
    /// <summary>
    /// The coordinate system based on which all other coordinate systems
    /// are defined. This is the world coordinate system.
    /// </summary>
    public required CoordinateSystem RootSystem { get; init; }

    /// <summary>
    /// A second coordinate system that is different than the root system.
    /// </summary>
    public CoordinateSystem? SecondarySystem { get; set; }

    /// <summary>
    /// The point that is affected by the transformations.
    /// </summary>
    public MathPoint? Point { get; set; }

    /// <summary>
    /// Translates a vector to the new coordinate system. This function assumes
    /// that the vector
    /// </summary>
    public CoordinatesTransformer TranslatePoint() {
        if (SecondarySystem is null) {
            return this;
        }


        if (Point is null) {
            return this;
        }
        if (Point.Dimension != SecondarySystem.Dimension) {
            throw new InvalidOperationException("The vector dimensions should be equal to the target system dimesions");
        }

        if (SecondarySystem.Location is null) {
            throw new InvalidOperationException("Target system should have defined");
        }


        Point = Point.Subtract(SecondarySystem.Location);

        return this;

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

    private static Matrix GetRotationMatrixByDegree(double degangle) {
        double angle = degangle * Math.PI / 180;
        return GetRotationMatrixByRadian(angle);
    }

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
    /// Calculates the rotation matrix between two coordinate systems
    /// <remarks>
    ///     Assumes that the systems are orthocanonical.
    /// </remarks>
    /// </summary>
    private Matrix Calculate2DRotationMatrix(CoordinateSystem target_system) {
        if (RootSystem.Dimension is not 2) {
            throw new InvalidOperationException("Root system is not 2D");
        }

        if (target_system.Dimension is not 2) {
            throw new InvalidOperationException("Target system is not 2D");
        }

        //Assuming that the coordinate system is orthocanonical we only need to
        //check a single vector.

        Vector root_direction = RootSystem.DirectionVectors[0];
        Vector target_direction = target_system.DirectionVectors[0];

        if (root_direction.HaveSameCoordinatesAs(target_direction)) {
            return GetRotationMatrixByRadian(0.0);
        } else {
            double angle_rad = root_direction.Find2DAngle(target_direction);
            return GetRotationMatrixByRadian(angle_rad);
        }
    }

    /// <summary>
    /// Transforms a point that was defined in the root coordinate system to it's target system
    /// representation.
    /// </summary>
    public CoordinatesTransformer RotatePointByAngle(double angle) {

        if (Point is null) {
            return this;
        }
        Matrix m = GetRotationMatrixByDegree(angle) * Point.ToMatrix();

        m.Print();

        MathPoint? temp = m.ToMathPoint();

        if (temp is null) {
            throw new InvalidOperationException("Conversion to math point resulted in null value!");
        } else {
            Point = temp;
        }

        return this;



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
}