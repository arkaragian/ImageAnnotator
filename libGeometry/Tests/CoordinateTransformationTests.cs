using libGeometry;

namespace Tests;

[TestClass]
public class CoordinateTransformationTests {
#pragma warning disable CA1707
    [TestMethod]
    public void TranslatePoint_SimpleTranslations_ShouldTranslateCorrectly() {

        CoordinatesTransformer Transformer = new() {
            //The root system is the WPF coordinate system
            RootSystem = new() {
                DirectionVectors = new Vector[] {
                    //X positive direction points towards the right
                    new() {
                        Coordinates = new double[] {1.0, 0.0}
                    },
                    //Y positive direction points downwards
                    new() {
                        Coordinates = new double[] {0.0, -1.0}
                    }
                }
            },
            //The tikz coordinate system
            SecondarySystem = new() {
                DirectionVectors = new Vector[] {
                    //X direction
                    new() {
                        Coordinates = new double[] {1.0, 0.0}
                    },
                    //Y direction
                    new() {
                        Coordinates = new double[] {0.0, 1.0}
                    }
                },
                //Location of the tikz system defined in terms of root system
                //coordinates and the positive sign depents on the direction
                //is based on the
                Location = new() {
                    Coordinates = new double[] { 3.0, 4.0 }
                }
            }
        };


        //Defined in the root coordinate system
        MathPoint startPoint = new() {
            Coordinates = new double[] { 5.0, 2.0 }
        };


        MathPoint actual = Transformer.TransformToSecondarySystem(startPoint);


        Assert.AreEqual(2.0, actual.Coordinates[0], 0.01);
        Assert.AreEqual(2.0, actual.Coordinates[1], 0.01);
    }

    [TestMethod]
    public void RotatePoint_SimpleRotation_ShouldRotateCorrectly() {

        //     Vector x_dir = new() {
        //         Coordinates = new double[] { 1.0, 0, 0 }
        //     };
        //
        //     Vector y_dir = new() {
        //         Coordinates = new double[] { 0, 1.0, 0 }
        //     };
        //
        //     Vector z_dir = new() {
        //         Coordinates = new double[] { 0, 0, 1.0 }
        //     };
        //
        //     CoordinateSystem root_system = new() {
        //         DirectionVectors = new Vector[] { x_dir, y_dir, z_dir }
        //     };
        //
        //
        //     MathPoint startPoint = new() {
        //         Coordinates = new double[] { 0.0, 1.0 }
        //     };
        //
        //     CoordinatesTransformer ct = new() {
        //         RootSystem = root_system,
        //         Point = startPoint
        //     };
        //
        //
        //     _ = ct.RotatePointByAngle(90.0);
        //
        //
        //     Assert.AreEqual(1.0, ct.Point.Coordinates[0], 0.01);
        //     Assert.AreEqual(0.0, ct.Point.Coordinates[1], 0.01);
        //
        }
#pragma warning restore CA1707
    }