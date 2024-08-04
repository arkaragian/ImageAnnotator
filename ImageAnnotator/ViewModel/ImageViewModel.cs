using ImageAnnotator.Model;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ImageAnnotator.ViewModel;

public struct DoublePoint {
    public double X { get; set; }
    public double Y { get; set; }
}

/// <summary>
/// The view model that is currently displayed
/// </summary>
public class ImageViewModel : INotifyPropertyChanged {

    //Bitmap bitmap = new Bitmap(1000, 800, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);


    //private ImageModel _imageModel;

    /// <summary>
    /// The image model that this view model deals with
    /// </summary>
    public required ImageModel ImageModel { get; init; }

    /// <summary>
    /// The path of the image that the model holds
    /// </summary>
    public string? ImagePath => ImageModel.ImagePath;

    /// <summary>
    /// The path of the image that is displayed to the user
    /// </summary>
    public string ImageDisplayPath => ImageModel.ImagePath ?? "No Image";

    /// <summary>
    /// The cursor position
    /// </summary>
    public Point CursorPosition { get; set; }

    /// <summary>
    /// The normalized position
    /// </summary>
    public DoublePoint NormalizedCursorPosition { get; set; }

    /// <summary>
    /// The size of the image
    /// </summary>
    public Size ImageSize { get; set; }

    public Canvas? AnnotationCanvas { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Loads an image to the model
    /// </summary>
    /// <param name="filename">The path to the file of the image</param>
    public Exception? LoadImage(string filename) {
        try {
            ImageModel.Image = new Bitmap(filename);
            IAnnotation a = new LineAnnotation() {
                StartPoint = new() {
                    X = 10,
                    Y = 20
                },
                EndPoint = new() {
                    X = 20,
                    Y = 20
                },
            };
            ImageModel.AddAnotation(a);
            if (AnnotationCanvas is not null) {
                Line l = new() {
                    X1 = 0,
                    Y1 = 0,

                    X2 = 100,
                    Y2 = 100,

                    StrokeThickness = 1
                };
                _ = AnnotationCanvas.Children.Add(l);
            }
        } catch (Exception ex) {
            ImageModel.Image = null;
            return ex;
        }

        ImageModel.ImagePath = filename;
        ImageSize = ImageModel.Image.Size;
        OnPropertyChanged(nameof(ImageSize));
        OnPropertyChanged(nameof(ImagePath));
        OnPropertyChanged(nameof(ImageDisplayPath));

        return null;
    }

    public void UpdateCursorPosition(Point p, Size controlSize) {
        CursorPosition = p;
        //NormalizedCursorPosition = new DoublePoint() { X = (double)ImageSize.Width / (double)p.X, Y = (double)ImageSize.Height / (double)p.Y };
        //
        //The Zero of the image is the top left. For for tikz is the bottom left.
        NormalizedCursorPosition = new DoublePoint() {
            X = (double)p.X / controlSize.Width,
            Y = ((double)p.Y / controlSize.Height * -1) + 1
        };

        //To do the transformation we should assoumte a vector on the Normalti
        //

        OnPropertyChanged(nameof(CursorPosition));
        OnPropertyChanged(nameof(NormalizedCursorPosition));
    }

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Save(string filename) {
        // ImageModel.Save(filename);
    }
    //public bool Load(string filename) {
    //    try {
    //        return true;
    //    } catch {
    //        return false;
    //    }
    //    // Reticle? r = Reticle.Load(filename);
    //    // if (r is not null) {
    //    //     this.Reticle = r;
    //    //     return true;
    //    // } else { return false; }
    //}

    // public void SaveImage(Canvas canvas, string filename) {
    //     canvas.Children.Clear();
    //
    //     List<Line> lines = LineSet;
    //     List<TextBlock> textBlocks = TextBlocksSet;
    //     foreach (Line line in lines) {
    //         canvas.Children.Add(line);
    //     }
    //     foreach (TextBlock textBlock in textBlocks) {
    //         canvas.Children.Add(textBlock);
    //     }
    //
    //     Reticle.SaveCanvasToFile(canvas, filename);
    //
    //     canvas.Children.Clear();
    // }
}