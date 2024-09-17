using System.Collections.Generic;
using libGeometry;

namespace ImageAnnotator.Model;

/// <summary>
/// The image model for the image that is loaded
/// </summary>
public class AnnotatorModel {
    /// <summary>
    /// The path of the image that is edited.
    /// </summary>
    public string? ImagePath { get; set; }

    /// <summary>
    /// An integer that keeps track of the annotations that are inserted in the
    /// model
    /// </summary>
    public int AnnotationCounter { get; set; }

    /// <summary>
    /// The data of the image that is being annotated
    /// </summary>
    public System.Drawing.Bitmap? Image { get; set; }

    /// <summary>
    /// The list of annotations
    /// </summary>
    public List<IAnnotation> Annotations { get; private set; } = [];

    private readonly CoordinatesTransformer _transformer;

    public AnnotatorModel() {
        _transformer = new() {
            //The root system is the WPF coordinate system
            RootSystem = new() {
                DirectionVectors = [
                    //X direction
                    new() {
                        Coordinates = [1.0, 0.0]
                    },
                    //Y direction
                    new() {
                        Coordinates = [0.0, -1.0]
                    }
                ]
            },
            //The tikz coordinate system
            SecondarySystem = new() {
                DirectionVectors = [
                    //X direction
                    new() {
                        Coordinates = [1.0, 0.0]
                    },
                    //Y direction
                    new() {
                        Coordinates = [0.0, 1.0]
                    }
                ],
                //Location of the tikz system defined in terms of root system
                //coordinates
                Location = new() {
                    Coordinates = [0.0, 1.0]
                }
            }
        };
    }

    /// <summary>
    /// Inserts a node to the list of annotations
    /// </summary>
    /// <param name="imageCoordinates">A point that represernts the coordinates of an image</param>
    public void InsertNode(MathPoint imageCoordinates, MathPoint normalizedCoordinates) {

        NodeAnnotation na = new() {
            NodeImagePoint = imageCoordinates,
            NodeImageNormalizedPoint = normalizedCoordinates,
            NodeTikzPoint = _transformer.TransformToSecondarySystem(normalizedCoordinates),
            Name = $"Node {AnnotationCounter++}"
        };
        Annotations.Add(na);
    }

    public void RemoveAnnotation(IAnnotation a) {
        bool ok = Annotations.Remove(a);
        if (ok) {
            // AnnotationCounter--;
        }
    }

    public void ResizeAnnotationCoordinates(DoubleSize size) {
        foreach (IAnnotation a in Annotations) {
            a.ResizeCoordinates(size);
        }
    }
}