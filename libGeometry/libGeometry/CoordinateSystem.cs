namespace libGeometry;

public class CoordinateSystem {
    /// <summary>
    /// The direction vectors
    /// </summary>
    public required Vector[] DirectionVectors { get; init; }

    /// <summary>
    /// The location of the coordinate system with regards to a "root"
    /// coordinate system.
    /// </summary>
    public MathPoint? Location { get; set; }

    public int Dimension => DirectionVectors.Length;

    /// <summary>
    /// Basic constructor that may accept any number of arguments.
    /// </summary>
    /// <param name="directions">The coordinates that
    // public CoordinateSystem(params Vector[] directions) {
    //     if (directions is null || directions.Length is 0) {
    //         throw new ArgumentException("A coordinate system must have at least one dimension.");
    //     }
    //
    //     directionVectors = new Vector[directions.Length];
    //     Array.Copy(directions, directionVectors, directions.Length);
    // }

    /// <summary>
    /// The [ ] operator for setting and getting the values.:w
    /// </summary>
    public Vector this[int index] {
        get => DirectionVectors[index];
        set => DirectionVectors[index] = value;
    }
}