using System.Windows.Input;

namespace ImageAnnotator;

public static class AnnotatorCommands {
    public static readonly RoutedUICommand InsertNode = new("Inserts a Node to the Model", "InsertNode", typeof(AnnotatorCommands));
    public static readonly RoutedUICommand InsertLine = new("Inserts a Line to the Model", "InsertLine", typeof(AnnotatorCommands));
    public static readonly RoutedUICommand InsertRectangle = new("Inserts a Rectangle to the Model", "InsertRectangle", typeof(AnnotatorCommands));
    public static readonly RoutedUICommand DeleteAnnotation = new("Deletes an anotation from the model", "DeleteAnnotation", typeof(AnnotatorCommands));
    public static readonly RoutedUICommand TranslateUp = new("Translates an annotation upwards", "TranslateUp", typeof(AnnotatorCommands));
    public static readonly RoutedUICommand TranslateDown = new("Translates an annotation Down", "TranslateDown", typeof(AnnotatorCommands));
    public static readonly RoutedUICommand TranslateLeft = new("Translates an annotation Left", "TranslateLeft", typeof(AnnotatorCommands));
    public static readonly RoutedUICommand TranslateRight = new("Translates an annotation Right", "TranslateRight", typeof(AnnotatorCommands));

    public static readonly RoutedUICommand CancelInput = new("Cancels Input", "CancelInput", typeof(AnnotatorCommands));


    public static readonly RoutedUICommand ZoomIn = new("Zooms Canvas Out", "ZoomIn", typeof(AnnotatorCommands));
    public static readonly RoutedUICommand ZoomOut = new("Zooms Canvas In", "ZoomOut", typeof(AnnotatorCommands));
}