﻿using ImageAnnotator.Model;
using ImageAnnotator.Model.Shapes;
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
    public MathPoint NormalizedCursorPosition { get; set; }

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

    /// <summary>
    /// A transient annotation that is drawn only during user input. That is
    /// important for
    /// </summary>
    public Shape? TransientAnotation { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Indicates if an annotation insertion can be performed.
    /// </summary>
    public bool CanMakeInsertion {
        get {
            if (Model.ImagePath is null) {
                return false;
            }
            return CurrentInputState is InputState.Idle;
        }
    }

    /// <summary>
    /// Indicates if a delete operation can be performed in any annotation.
    /// </summary>
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

    public AnnotatorViewModel() {
        NormalizedCursorPosition = new() {
            Coordinates = [0.0, 0.0]
        };
    }

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

    /// <summary>
    /// Removes the given annotation from the model and redraws the
    /// list of annotations that the model contains.
    /// </summary>
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
        //TODO: Implement canvas drawing options.
        if (canvas is null) {
            return;
        }

        if (canvas.Width is 0) {
            throw new InvalidOperationException("Zero canvas width!");
        }

        if (canvas.Height is 0) {
            throw new InvalidOperationException("Zero canvas height!");
        }

        double thick = 3.0;

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
                StrokeThickness = 1,
                StrokeDashArray = [thick, thick]
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
                StrokeThickness = 1,
                StrokeDashArray = [thick, thick]
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

    public static MathPoint NormalizePoint(Point p, Size controlSize) {
        //The Zero of the image is the top left. For for tikz is the bottom left.
        MathPoint normalizedPoint = new() {
            Coordinates = [
                (double)p.X / controlSize.Width,
                ((double)p.Y / controlSize.Height * -1) + 1
            ]
        };

        return normalizedPoint;
    }

    public void UpdateCursorPosition(Point p, Size controlSize) {
        ClearTransientAnnotation();
        //Also draw a temp item.
        if (CurrentInputState is InputState.WaitingForInput && CurrentInsertionType is not InsertionType.Node) {
            if (CurrentInsertionType is InsertionType.Rectangle) {
                if (_rectangleBuilder?.PointA is not null) {
                    MathPoint pointB = new() {
                        Coordinates = [p.X, p.Y]
                    };
                    MathPoint ul = RectanglePointResolver.UpperLeftPoint(_rectangleBuilder.PointA, pointB);
                    MathPoint lr = RectanglePointResolver.LowerRightPoint(_rectangleBuilder.PointA, pointB);
                    RectangleAnnotation r = new() {
                        UpperLeftNode = new() {
                            NodeImagePoint = ul,
                            NodeImageNormalizedPoint = new() {
                                Coordinates = [0, 0]
                            },
                            NodeTikzPoint = new() {
                                Coordinates = [0, 0]
                            }
                        },
                        LowerRightNode = new() {
                            NodeImagePoint = lr,
                            NodeImageNormalizedPoint = new() {
                                Coordinates = [0, 0]
                            },
                            NodeTikzPoint = new() {
                                Coordinates = [0, 0]
                            }
                        }
                    };
                    TransientAnotation = r.ToShape();
                }
            }

            if (CurrentInsertionType is InsertionType.Line) {
                if (_lineBuilder?.StartPoint is not null) {
                    TransientAnotation = new ColorLine() {
                        StartPoint = new System.Windows.Point() {
                            X = _lineBuilder.StartPoint[0],
                            Y = _lineBuilder.StartPoint[1],
                        },
                        EndPoint = new System.Windows.Point() {
                            X = p.X,
                            Y = p.Y
                        }
                    };
                }
            }
        } else {
            ClearTransientAnnotation();
        }

        if (TransientAnotation is not null) {
            _ = AnnotationCanvas?.Children.Add(TransientAnotation!);
        }

        CursorPosition = p;
        NormalizedCursorPosition = NormalizePoint(p, controlSize);

        OnPropertyChanged(nameof(CursorPosition));
        OnPropertyChanged(nameof(NormalizedCursorPosition));
    }

    /// <summary>
    /// Remove any transient annotation from the canvas.
    /// </summary>
    public void ClearTransientAnnotation() {
        if (TransientAnotation is not null) {
            AnnotationCanvas?.Children.Remove(TransientAnotation!);
            TransientAnotation = null;
        }
    }

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



    public void InsertNode(MathPoint imagePoint) {
        StatusMessage = null;


        DoubleSize s = new() {
            Width = AnnotationCanvas!.Width,
            Height = AnnotationCanvas!.Height
        };

        MathPoint normalizedPoint = new() {
            Coordinates = [
                imagePoint[0]/s.Width,
                imagePoint[1]/s.Height,
            ]
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

    public void CancelInsertion() {

        CurrentInputState = InputState.Idle;
        CurrentInsertionType = null;

        _rectangleBuilder = null;
        _lineBuilder = null;
        StatusMessage = null;

        OnPropertyChanged(nameof(StatusMessage));
        OnPropertyChanged(nameof(CurrentInputState));
        OnPropertyChanged(nameof(CurrentInsertionType));

        return;
    }

    /// <summary>
    /// Sets up the viewmodesl to receive additional inputs for annotations
    /// </summary>
    public void BeginInsertion(InsertionType insertion) {
        StatusMessage = insertion switch {
            InsertionType.Node => "Click on Image to insert node",
            InsertionType.Line => "Click on First Location",
            InsertionType.Rectangle => "Click on the First Point",
            _ => throw new InvalidOperationException("Invalid Enum Member"),
        };

        CurrentInputState = InputState.WaitingForInput;
        CurrentInsertionType = insertion;

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
            }
        };

        if (!_lineBuilder.HasStartPoint) {
            _ = _lineBuilder.WithStartPoint(point);
            StatusMessage = "Enter Second Point";
            CurrentInputState = InputState.WaitingForInput;
            CurrentInsertionType = InsertionType.Line;
            OnPropertyChanged(nameof(StatusMessage));
            OnPropertyChanged(nameof(CurrentInputState));
            OnPropertyChanged(nameof(CurrentInsertionType));
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

    public void TranslateIndices(int index, int xTranslation, int yTranslation, DoubleSize s) {
        Model.TranslateAnnotation(index, xTranslation, yTranslation, s);
        DrawAnnotations(AnnotationCanvas);
        OnPropertyChanged(nameof(Annotations));
    }

}