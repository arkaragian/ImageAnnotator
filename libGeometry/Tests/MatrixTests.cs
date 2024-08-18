using libGeometry;

namespace Tests;

[TestClass]
public class MatrixTests {
#pragma warning disable CA1707
    [TestMethod]
    public void ToMatrix_SimplePoint_ShouldConvertCorrectly() {
        MathPoint p = new() {
            Coordinates = new double[] { 2.0, 3.0, 4.0 }
        };

        Matrix m = p.ToMatrix();
        Assert.AreEqual(1, m.Columns);
        Assert.AreEqual(3, m.Rows);
    }

    [TestMethod]
    public void ToMathPoint_OneDimensionalMatrix_ShouldConvertCorrectly() {
        Matrix m = new(rows: 2, columns: 1) {
            Data = new double[2, 1] {
                { 1.0 },
                { 2.0 }
            }
        };
        MathPoint? point = m.ToMathPoint();
        Assert.IsNotNull(point);
    }
#pragma warning restore CA1707
}