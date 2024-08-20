using libGeometry;

namespace Tests;

[TestClass]
public class VectorTests {
#pragma warning disable CA1707
    [TestMethod]
    public void Find2DAngle_SameVector_ShouldReturnZero() {
        Vector v1 = new() {
            Coordinates = new double[] { 1.0, 0.0 }
        };

        Vector v2 = new() {
            Coordinates = new double[] { 1.0, 0.0 }
        };

        double actual = v1.Find2DAngle(v2);
        Assert.AreEqual(0.0, actual, 0.001);
    }

    [TestMethod]
    public void Find2DAngle_SameVector_ShouldReturn45Degrees() {
        Vector v1 = new() {
            Coordinates = new double[] { 0.0, 1.0 }
        };

        Vector v2 = new() {
            Coordinates = new double[] { 0.707, 0.707 }
        };

        double actual = v1.Find2DAngle(v2);
        //0.785398 is 45 degrees to rads
        Assert.AreEqual(0.785398, actual, 0.001);
    }

    [TestMethod]
    public void Find2DAngle_SameVector_ShouldReturnMinus45Degrees() {
        Vector v1 = new() {
            Coordinates = new double[] { 0.707, 0.707 }
        };

        Vector v2 = new() {
            Coordinates = new double[] { 0.0, 1.0 }
        };

        double actual = v1.Find2DAngle(v2);
        //0.785398 is 45 degrees to rads
        Assert.AreEqual(-0.785398, actual, 0.001);
    }
#pragma warning restore CA1707
}