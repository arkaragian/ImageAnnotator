using ImageAnnotator.Model;
using ImageAnnotator.Tikz;
using libGeometry;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ImageAnnotator.ViewModel;


public enum InputState {
    /// <summary>
    /// waiting for user input
    /// </summary>
    Idle,

    /// <summary>
    /// Indicates that application is waiting for user input
    /// </summary>
    WaitingForInput,
}

public enum InsertionType {
    Node,
    Line,
    Rectangle
}


/// <summary>
/// The view model that is currently displayed
/// </summary>
public class AnnotatorViewModel : INotifyPropertyChanged {

    /// <summary>
    /// The image model that this view model deals with
    /// </summary>
    public required AnnotatorModel Model { get; init; }

    /// <summary>
    /// The path of the image that the model holds
    /// </summary>
    public string? ImagePath => Model.ImagePath;

    /// <summary>
    /// An ocasional status message to aid the user
    /// </summary>
    public string? StatusMessage { get; private set; }

    /// <summary>
    /// Defines the input state of the application
    /// </summary>
    public InputState CurrentInputState { get; private set; } = InputState.Idle;

    /// <summary>
    /// Defines the current awaited insertion type
    /// </summary>
    public InsertionType? CurrentInsertionType { get; private set; }

    /// <summary>
    /// The path of the image that is displayed to the user
    /// </summary>
    public string ImageDisplayPath => Model.ImagePath ?? "No Image";

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

    /// <summary>
    /// The canvas onto which all the annotations are drawn
    /// </summary>
    public Canvas? AnnotationCanvas { get; set; }

    /// <summary>
    /// The canvas where the grid is drawn
    /// </summary>
    public Canvas? GridCanvas { get; set; }

    public TextBox? CodeArea { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Indicates if the view is correctly setup to insert a node
    /// </summary>
    public bool CanInsertNode {
        get {
            if (Model.ImagePath is null) {
                return false;
            }
            return CurrentInputState is InputState.Idle;
        }
    }

    /// <summary>
    /// Indicates if the view is correctly setup to insert a node
    /// </summary>
    public bool CanInsertLine {
        get {
            if (Model.ImagePath is null) {
                return false;
            }
            return CurrentInputState is InputState.Idle;
        }
    }

    /// <summary>
    /// Indicates if the view is correctly setup to insert a node
    /// </summary>
    public bool CanInsertRectangle {
        get {
            if (Model.ImagePath is null) {
                return false;
            }
            return CurrentInputState is InputState.Idle;
        }
    }

    public bool CanDeleteAnnotation => (CurrentInputState is InputState.Idle) && Model.Annotations.Count > 0;

    /// <summary>
    /// Indicates if the view is waiting for any input input
    /// </summary>
    public bool IsWaitingForInput => CurrentInputState is InputState.WaitingForInput;

    /// <summary>
    /// Indicates if the view is waiting for a node input
    /// </summary>
    public bool IsWaitingForNodeInput => CurrentInsertionType is InsertionType.Node;

    /// <summary>
    /// Indicates if the view is waiting for a node input
    /// </summary>
    public bool IsWaitingForLineInput => CurrentInsertionType is InsertionType.Line;

    /// <summary>
    /// Indicates if the view is waiting for a node input
    /// </summary>
    public bool IsWaitingForRectangleInput => CurrentInsertionType is InsertionType.Rectangle;

    public ObservableCollection<IAnnotation> Annotations => new(Model.Annotations);

    private LineBuilder? _lineBuilder;
    private RectangleBuilder? _rectangleBuilder;

    /// <summary>
    /// Loads an image to the model
    /// </summary>
    /// <param name="filename">The path to the file of the image</param>
    public Exception? LoadImage(string filename) {
        try {
            Model.Image = new Bitmap(filename);
        } catch (Exception ex) {
            Model.Image = null;
            return ex;
        }

        Model.ImagePath = filename;
        ImageSize = Model.Image.Size;
        if (CodeArea is not null) {
            GenereateCode(CodeArea);
        }
        OnPropertyChanged(nameof(ImageSize));
        OnPropertyChanged(nameof(ImagePath));
        OnPropertyChanged(nameof(ImageDisplayPath));

        return null;
    }

    public void DeleteSelectedAnnotation(IAnnotation a) {
        Model.RemoveAnnotation(a);

        DrawAnnotations(AnnotationCanvas);

        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(CurrentInputState));
        OnPropertyChanged(nameof(CurrentInsertionType));
        OnPropertyChanged(nameof(Annotations));
    }

    /// <summary>
    /// Draws a grid to the specified canvas.
    /// </summary>
    public static void DrawGrid(Canvas? canvas) {
        if (canvas is null) {
            return;
        }

        if (canvas.Width is 0) {
            throw new InvalidOperationException("Zero canvas width!");
        }

        if (canvas.Height is 0) {
            throw new InvalidOperationException("Zero canvas height!");
        }

        double w = canvas.Width;
        double h = canvas.Height;
        for (int i = 1; i < 10; i++) {
            double step = (int)(w * i * 0.1);
            Line l = new() {
                X1 = step,
                X2 = step,

                Y1 = 0,
                Y2 = h,

                Stroke = System.Windows.Media.Brushes.Black,
                StrokeThickness = 2,
                StrokeDashArray = [2.0, 2.0]
                //Brush = System.Windows.Media.Brushes.Black

            };

            _ = canvas.Children.Add(l);
        }

        for (int i = 1; i < 10; i++) {
            double step = (int)(h * i * 0.1);
            Line l = new() {
                Y1 = step,
                Y2 = step,

                X1 = 0,
                X2 = w,

                Stroke = System.Windows.Media.Brushes.Black,
                StrokeThickness = 2,
                StrokeDashArray = [2.0, 2.0]
                //Brush = System.Windows.Media.Brushes.Black

            };

            _ = canvas.Children.Add(l);
        }
    }

    /// <summary>
    /// Updates the annotation with new coordinates based on the new control width and height
    /// </summary>
    /// <remarks>
    ///     The width and height are given independendtly to avoid bringing custom dependencies
    ///     at the wpf level.
    /// </remarks>
    public void UpdateCoordinates(double newControlWidth, double newControlHeight) {
        DoubleSize s = new() {
            Width = newControlWidth,
            Height = newControlHeight,
        };

        Model.ResizeAnnotationCoordinates(s);

        DrawAnnotations(AnnotationCanvas);
    }

    public void DrawAnnotations(Canvas? canvas) {
        if (canvas is null) {
            return;
        }

        if (canvas.Width is 0) {
            throw new InvalidOperationException("Zero canvas width!");
        }

        if (canvas.Height is 0) {
            throw new InvalidOperationException("Zero canvas height!");
        }

        canvas.Children.Clear();

        foreach (IAnnotation a in Model.Annotations) {
            _ = canvas.Children.Add(a.ToShape());
        }

        GenereateCode(CodeArea!);
    }

    public void GenereateCode(TextBox block) {
        CodeGenerator cg = new() {
            ImagePath = Model.ImagePath!,
            Annotations = Model.Annotations
        };

        string code = cg.GenerateCode();
        block.Text = code;
    }

    public static DoublePoint NormalizePoint(Point p, Size controlSize) {
        //The Zero of the image is the top left. For for tikz is the bottom left.
        DoublePoint normalizedPoint = new() {
            X = (double)p.X / controlSize.Width,
            Y = ((double)p.Y / controlSize.Height * -1) + 1
        };

        return normalizedPoint;
    }

    public void UpdateCursorPosition(Point p, Size controlSize) {
        CursorPosition = p;
        NormalizedCursorPosition = NormalizePoint(p, controlSize);

        OnPropertyChanged(nameof(CursorPosition));
        OnPropertyChanged(nameof(NormalizedCursorPosition));
    }

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public void BeginNodeInsertion() {
        StatusMessage = "Click on Image to insert node";
        CurrentInputState = InputState.WaitingForInput;
        CurrentInsertionType = InsertionType.Node;
        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(CurrentInputState));
        OnPropertyChanged(nameof(CurrentInsertionType));
        return;
    }

    public void InsertNode(MathPoint imagePoint) {
        StatusMessage = null;


        DoubleSize s = new() {
            Width = AnnotationCanvas!.Width,
            Height = AnnotationCanvas!.Height
        };

        MathPoint normalizedPoint = new() {
            Coordinates = new double[] {
                imagePoint[0]/s.Width,
                imagePoint[1]/s.Height,
            }
        };
        //DoublePoint normalizedPoint = TransformCoordinates.ToTikzCoordinates(imagePoint, s);

        Model.InsertNode(imagePoint, normalizedPoint);
        CurrentInputState = InputState.Idle;
        CurrentInsertionType = null;

        DrawAnnotations(AnnotationCanvas);

        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(CurrentInputState));
        OnPropertyChanged(nameof(CurrentInsertionType));
        OnPropertyChanged(nameof(Annotations));
        return;
    }

    public void BeginLineInsertion() {
        StatusMessage = "Click on First Location";
        CurrentInputState = InputState.WaitingForInput;
        CurrentInsertionType = InsertionType.Line;
        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(CurrentInputState));
        OnPropertyChanged(nameof(CurrentInsertionType));
        return;
    }

    public void BeginRectangleInsertion() {
        StatusMessage = "Click on the First Point";
        CurrentInputState = InputState.WaitingForInput;
        CurrentInsertionType = InsertionType.Rectangle;
        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(CurrentInputState));
        OnPropertyChanged(nameof(CurrentInsertionType));
        return;
    }

    public void InsertLine(MathPoint point) {
        //TODO: Implement two steps. One for first point and one for second point
        //
        _lineBuilder ??= new LineBuilder() {
            DrawingRegion = new DoubleSize() {
                Width = AnnotationCanvas!.Width,
                Height = AnnotationCanvas.Height,
            },
            Transformer = new() {
                //The root system is the WPF coordinate system
                RootSystem = new() {
                    DirectionVectors = new Vector[] {
                    //X direction
                    new() {
                        Coordinates = new double[] {1.0, 0.0}
                    },
                    //Y direction
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
                    //coordinates
                    Location = new() {
                        Coordinates = new double[] { 0.0, 1.0 }
                    }
                }
            }
        };

        if (!_lineBuilder.HasStartPoint) {
            _ = _lineBuilder.WithStartPoint(point);
            StatusMessage = "Enter Second Point";
            OnPropertyChanged(nameof(StatusMessage));
            return;
        }

        _ = _lineBuilder.WithEndPoint(point);

        LineAnnotation? la = _lineBuilder.Build(Model.AnnotationCounter++);

        if (la is null) {
            StatusMessage = "Could not build line annotation!";
            CurrentInputState = InputState.Idle;
            CurrentInsertionType = null;
            OnPropertyChanged(nameof(StatusMessage));
            OnPropertyChanged(nameof(CurrentInputState));
            OnPropertyChanged(nameof(CurrentInsertionType));
            return;
        }


        StatusMessage = null;
        CurrentInputState = InputState.Idle;
        CurrentInsertionType = null;
        Model.Annotations.Add(la);

        DrawAnnotations(AnnotationCanvas);

        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(CurrentInputState));
        OnPropertyChanged(nameof(CurrentInsertionType));
        OnPropertyChanged(nameof(Annotations));
        return;
    }

    public void InsertRectangle(MathPoint point) {
        //TODO: Implement two steps. One for first point and one for second point

        _rectangleBuilder ??= new RectangleBuilder() {
            DrawingRegion = new DoubleSize() {
                Width = AnnotationCanvas!.Width,
                Height = AnnotationCanvas.Height,
            },
            Transformer = new() {
                //The root system is the WPF coordinate system
                RootSystem = new() {
                    DirectionVectors = new Vector[] {
                    //X direction
                    new() {
                        Coordinates = new double[] {1.0, 0.0}
                    },
                    //Y direction
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
                    //coordinates
                    Location = new() {
                        Coordinates = new double[] { 0.0, 1.0 }
                    }
                }
            }
        };

        if (!_rectangleBuilder.HasAllRequiredPoints) {
            _ = _rectangleBuilder.AddPoint(point);
        }

        if (!_rectangleBuilder.HasAllRequiredPoints) {
            StatusMessage = "Enter Second Point";
            OnPropertyChanged(nameof(StatusMessage));
            return;
        }



        RectangleAnnotation? ra = _rectangleBuilder.Build(Model.AnnotationCounter++);

        if (ra is null) {
            StatusMessage = "Could not build rectangle annotation!";
            CurrentInputState = InputState.Idle;
            CurrentInsertionType = null;
            OnPropertyChanged(nameof(StatusMessage));
            OnPropertyChanged(nameof(CurrentInputState));
            OnPropertyChanged(nameof(CurrentInsertionType));
            return;
        }


        StatusMessage = null;
        CurrentInputState = InputState.Idle;
        CurrentInsertionType = null;
        Model.Annotations.Add(ra);

        DrawAnnotations(AnnotationCanvas);

        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(CurrentInputState));
        OnPropertyChanged(nameof(CurrentInsertionType));
        OnPropertyChanged(nameof(Annotations));
        return;
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