using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageAnnotator.Model;
public class RectangleAnnotation : IAnnotation {
    public string Name { get; set; } = "Rectangle";
    public Drawing ToDrawing() {
        throw new System.NotImplementedException();
    }

    public Shape ToShape() {
        throw new System.NotImplementedException();
    }
}