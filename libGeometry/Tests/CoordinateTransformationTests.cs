using libGeometry;

namespace Tests;

[TestClass]
public class CoordinateTransformationTests {
#pragma warning disable CA1707
    [TestMethod]
    public void TranslatePoint_SimpleTranslations_ShouldTranslateCorrectly() {

        Vector x_dir = new() {
            Coordinates = new double[] { 1.0, 0, 0 }
        };

        Vector y_dir = new() {
            Coordinates = new double[] { 0, 1.0, 0 }
        };

        Vector z_dir = new() {
            Coordinates = new double[] { 0, 0, 1.0 }
        };

        CoordinateSystem root_system = new() {
            DirectionVectors = new Vector[] { x_dir, y_dir, z_dir }
        };

        CoordinateSystem secondary_system = new() {
            DirectionVectors = new Vector[] { x_dir, y_dir, z_dir },
            Location = new() {
                Coordinates = new double[] { 1.0, 1.0, 1.0 }
            }
        };

        MathPoint startPoint = new() {
            Coordinates = new double[] { 1.0, 1.0, 1.0 }
        };

        CoordinatesTransformer ct = new() {
            RootSystem = root_system,
            SecondarySystem = secondary_system,
            Point = startPoint
        };


        _ = ct.TranslatePoint();


        Assert.AreEqual(0.0, ct.Point.Coordinates[0], 0.01);
        Assert.AreEqual(0.0, ct.Point.Coordinates[1], 0.01);
        Assert.AreEqual(0.0, ct.Point.Coordinates[2], 0.01);

        secondary_system.Location = new MathPoint() {
            Coordinates = new double[] { -10.0, -10.0, -10.0 }
        };

        _ = ct.TranslatePoint();

        Assert.AreEqual(10.0, ct.Point.Coordinates[0], 0.01);
        Assert.AreEqual(10.0, ct.Point.Coordinates[1], 0.01);
        Assert.AreEqual(10.0, ct.Point.Coordinates[2], 0.01);
    }

    [TestMethod]
    public void RotatePoint_SimpleRotation_ShouldRotateCorrectly() {

        Vector x_dir = new() {
            Coordinates = new double[] { 1.0, 0, 0 }
        };

        Vector y_dir = new() {
            Coordinates = new double[] { 0, 1.0, 0 }
        };

        Vector z_dir = new() {
            Coordinates = new double[] { 0, 0, 1.0 }
        };

        CoordinateSystem root_system = new() {
            DirectionVectors = new Vector[] { x_dir, y_dir, z_dir }
        };


        MathPoint startPoint = new() {
            Coordinates = new double[] { 0.0, 1.0 }
        };

        CoordinatesTransformer ct = new() {
            RootSystem = root_system,
            Point = startPoint
        };


        _ = ct.RotatePointByAngle(90.0);


        Assert.AreEqual(1.0, ct.Point.Coordinates[0], 0.01);
        Assert.AreEqual(0.0, ct.Point.Coordinates[1], 0.01);

    }
#pragma warning restore CA1707
}