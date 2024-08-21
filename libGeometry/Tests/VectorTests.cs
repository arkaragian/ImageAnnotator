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
    public void Find2DAngle_VectorsIn45DegreesToEachOther_ShouldReturn45Degrees() {
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
    public void Find2DAngle_VectorsIn45DegreesToEachOtherReverse_ShouldReturn45Degrees() {
        Vector v1 = new() {
            Coordinates = new double[] { 0.0, 1.0 }
        };

        Vector v2 = new() {
            Coordinates = new double[] { 0.707, 0.707 }
        };

        double actual1 = v1.Find2DAngle(v2);
        double actual2 = v2.Find2DAngle(v1);
        //0.785398 is 45 degrees to rads
        Assert.AreEqual(actual1, actual2);
    }

    [TestMethod]
    public void FindSigned2DAngle_SameVector_ShouldReturnZero() {
        //Vector at 45 degrees
        Vector v1 = new() {
            Coordinates = new double[] { 0.707, 0.707 }
        };

        //The second vector is vertical upwards
        Vector v2 = new() {
            Coordinates = new double[] { 0.707, 0.707 }
        };

        double actual = v1.FindSigned2DAngle(v2);
        //0.785398 is 45 degrees to rads
        Assert.AreEqual(0.0, actual, 0.001);
    }

    [TestMethod]
    public void FindSigned2DAngle_VectorsAtMinus45Deg_ShouldReturnMinus45Degrees() {
        //Vector at 45 degrees
        Vector v1 = new() {
            Coordinates = new double[] { 0.707, 0.707 }
        };

        //The second vector is vertical upwards
        Vector v2 = new() {
            Coordinates = new double[] { 0.0, 1.0 }
        };

        double actual = v1.FindSigned2DAngle(v2);
        //0.785398 is 45 degrees to rads
        Assert.AreEqual(-0.785398, actual, 0.001);
    }

    [TestMethod]
    public void FindSigned2DAngle_OpositeVectors_ShouldReturn180() {
        //Vector at 45 degrees
        Vector v1 = new() {
            Coordinates = new double[] { 0.0, 1.0 }
        };

        //The second vector is vertical upwards
        Vector v2 = new() {
            Coordinates = new double[] { 0.0, -1.0 }
        };

        double actual = v1.FindSigned2DAngle(v2);
        //0.785398 is 45 degrees to rads
        Assert.AreEqual(Math.PI, actual, 0.001);
    }
#pragma warning restore CA1707
}