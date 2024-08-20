namespace libGeometry;

/// <summary>
/// A class that represents a coordinate system
/// </summary>
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

    /// <summary>
    /// The dimensions of the coordinate systems
    /// </summary>
    public int Dimension => DirectionVectors.Length;

    /// <summary>
    /// The [ ] operator for setting and getting the values.:w
    /// </summary>
    public Vector this[int index] {
        get => DirectionVectors[index];
        set => DirectionVectors[index] = value;
    }
}