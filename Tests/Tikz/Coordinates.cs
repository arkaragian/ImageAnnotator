using ImageAnnotator.Model;
using ImageAnnotator.Tikz;

namespace Tests.Tikz;

[TestClass]
public class Coordinates {
    [TestMethod]
    public void Transform() {

        // DoublePoint p = new() {
        //     X = 90.0,
        //     Y = 90.0
        // };
        //
        // DoubleSize s = new() {
        //     Width = 100.0,
        //     Height = 100.0,
        // };
        //
        // DoublePoint actual = TransformCoordinates.ToTikzCoordinates(p, s);
        //
        // DoublePoint expected = new() {
        //     X = 0.9,
        //     Y = 0.1
        // };
        //
        // Assert.AreEqual(expected.X, actual.X, 0.01);
        // Assert.AreEqual(expected.Y, actual.Y, 0.01);

    }

    [TestMethod]
    public void Transform_Back() {

        // DoublePoint p = new() {
        //     X = 90.0,
        //     Y = 90.0
        // };
        //
        // DoubleSize s = new() {
        //     Width = 100.0,
        //     Height = 100.0,
        // };
        //
        // DoublePoint actual = TransformCoordinates.ToTikzCoordinates(p, s);
        //
        // DoublePoint expected = new() {
        //     X = 0.9,
        //     Y = 0.1
        // };
        //
        // Assert.AreEqual(expected.X, actual.X, 0.01);
        // Assert.AreEqual(expected.Y, actual.Y, 0.01);
        //
        //
        // DoublePoint expected2 = new() {
        //     X = 0.9,
        //     Y = 0.9
        // };
        //
        // DoublePoint actual2 = TransformCoordinates.ToWPFCoordinates(actual);
        //
        // Assert.AreEqual(expected2.X, actual2.X, 0.01);
        // Assert.AreEqual(expected2.Y, actual2.Y, 0.01);

    }
}