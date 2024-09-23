using ImageAnnotator.Model;
using ImageAnnotator.ViewModel;
using libGeometry;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageAnnotator;

public partial class MainWindow : Window {

    /// <summary>
    /// The model of the application. All data are contained there.
    /// </summary>
    private readonly AnnotatorViewModel ViewModel;

    private readonly ScaleTransform _scaleTransform = new(1.0, 1.0);

    public MainWindow() {
        InitializeComponent();
        ViewModel = new() {
            Model = new(),
            AnnotationCanvas = AnnotationCanvas,
            GridCanvas = GridCanvas,
            CodeArea = CodeText,
        };
        Title = "Image Annotator";
        DataContext = ViewModel;
        //The status bar needs a different data context
        WindowInfo.DataContext = this;

        GridRowContainer.LayoutTransform = _scaleTransform;

        GridRowContainer.MouseWheel += Canvas_MouseWheel;

    }

    private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e) {
        // Determine the zoom direction
        if (e.Delta > 0) {
            ZoomIn();
        } else {
            ZoomOut();
        }
    }

    private void ZoomIn() {
        _scaleTransform.ScaleX += 0.1;
        _scaleTransform.ScaleY += 0.1;
    }

    private void ZoomOut() {
        if (_scaleTransform.ScaleX > 0.2) // prevent too much zoom out
        {
            _scaleTransform.ScaleX -= 0.1;
            _scaleTransform.ScaleY -= 0.1;
        }
    }

    /// <summary>
    /// Indicates if the node insertion command can be executed.
    /// </summary>
    private void InsertNodeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
        e.CanExecute = ViewModel.CanInsertNode;
    }

    /// <summary>
    /// Implements the logic of the command that is executed. At this point it set's up the application
    /// to wait for a new node click.
    /// </summary>
    private void InsertNodeCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
        ViewModel.BeginNodeInsertion();
    }

    /// <summary>
    /// Indicates if the node insertion command can be executed.
    /// </summary>
    private void InsertLineCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
        e.CanExecute = ViewModel.CanInsertLine;
    }

    /// <summary>
    /// Implements the logic of the command that is executed. At this point it set's up the application
    /// to wait for a new node click.
    /// </summary>
    private void InsertLineCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
        ViewModel.BeginLineInsertion();
    }

    /// <summary>
    /// Indicates if the node insertion command can be executed.
    /// </summary>
    private void InsertRectangleCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
        e.CanExecute = ViewModel.CanInsertRectangle;
    }

    /// <summary>
    /// Implements the logic of the command that is executed. At this point it set's up the application
    /// to wait for a new node click.
    /// </summary>
    private void InsertRectangleCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
        ViewModel.BeginRectangleInsertion();
    }

    private void TranslationCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
        e.CanExecute = AnnotationList.SelectedIndex is not -1;
        if (e.CanExecute) {
            Console.WriteLine("Will execute");
        } else {
            Console.WriteLine("Cannot execute");
        }
    }

    private void TranslateUp_Executed(object sender, ExecutedRoutedEventArgs e) {
        int index = AnnotationList.SelectedIndex;
        DoubleSize DrawingRegion = new() {
            Width = AnnotationCanvas!.Width,
            Height = AnnotationCanvas.Height,
        };
        ViewModel.TranslateIndices(index, xTranslation: 0, yTranslation: -5, DrawingRegion);
    }

    private void TranslateDown_Executed(object sender, ExecutedRoutedEventArgs e) {
        int index = AnnotationList.SelectedIndex;
        DoubleSize DrawingRegion = new() {
            Width = AnnotationCanvas!.Width,
            Height = AnnotationCanvas.Height,
        };
        ViewModel.TranslateIndices(index, xTranslation: 0, yTranslation: 5, DrawingRegion);
    }

    private void TranslateLeft_Executed(object sender, ExecutedRoutedEventArgs e) {
        int index = AnnotationList.SelectedIndex;
        DoubleSize DrawingRegion = new() {
            Width = AnnotationCanvas!.Width,
            Height = AnnotationCanvas.Height,
        };
        ViewModel.TranslateIndices(index, xTranslation: -5, yTranslation: 0, DrawingRegion);
    }

    private void TranslateRight_Executed(object sender, ExecutedRoutedEventArgs e) {
        int index = AnnotationList.SelectedIndex;
        DoubleSize DrawingRegion = new() {
            Width = AnnotationCanvas!.Width,
            Height = AnnotationCanvas.Height,
        };
        ViewModel.TranslateIndices(index, xTranslation: 5, yTranslation: 0, DrawingRegion);
    }

    private void DeleteAnnotationCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
        e.CanExecute = ViewModel.CanDeleteAnnotation;
        if (!e.CanExecute) {
            _ = MessageBox.Show("Cannot delete!");
        }
    }

    private void DeleteAnnotationCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
        //Actual Delete the annotation.
        //Find the selected annotation.
        int index = AnnotationList.SelectedIndex;
        if (index is -1) {
            return;
        }
        object? o = AnnotationList.SelectedItem;
        if (o is not null) {
            ViewModel.DeleteSelectedAnnotation((IAnnotation)o!);
        }
    }

    /// <summary>
    /// Loads an image in the ImageModel
    /// </summary>
    private void LoadImage(object sender, RoutedEventArgs e) {
        OpenFileDialog openDialog = new() {
            Title = "Select a picture",
            Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp" //+
        };

        if (openDialog.ShowDialog() is true) {
            Exception? r = ViewModel.LoadImage(openDialog.FileName);
            if (r is not null) {
                _ = MessageBox.Show(r.Message + "/n" + r.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Defer drawing grid to allow layout pass to complete
            _ = Dispatcher.BeginInvoke(new Action(() => {
                try {
                    AnnotatorViewModel.DrawGrid(GridCanvas);
                } catch (Exception ex) {
                    _ = MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }), System.Windows.Threading.DispatcherPriority.Loaded);

            //ImageViewModel.DrawGrid(GridCanvas);
        }
    }

    /// <summary>
    /// Handles all the clicks that happen in the canvas.
    /// </summary>
    private void AnnotationCanvasClick(object sender, MouseEventArgs e) {
        Console.WriteLine("Mouse Click Event!");
        //Only handle input is we are really waiting for one
        if (!ViewModel.IsWaitingForInput) {
            e.Handled = true;
            return;
        }

        Point p = e.GetPosition(AnnotationCanvas);
        MathPoint mp = new() {
            Coordinates = [p.X, p.Y]
        };
        if (ViewModel.IsWaitingForNodeInput) {
            ViewModel.ClearTransientAnnotation();
            ViewModel.InsertNode(mp);
            return;
        }

        if (ViewModel.IsWaitingForLineInput) {
            ViewModel.ClearTransientAnnotation();
            ViewModel.InsertLine(mp);
            return;
        }

        if (ViewModel.IsWaitingForRectangleInput) {
            ViewModel.ClearTransientAnnotation();
            ViewModel.InsertRectangle(mp);
            return;
        }
    }

    /// <summary>
    /// Handles any resize of the annotation canvas
    /// </summary>
    private void AnnotationCanvasResized(object sender, SizeChangedEventArgs e) {
        //Since all annotations that are painted are UI elements and in order for the
        //elemeents to be moved again it is wise to recalculate their coordinates.
        ViewModel.UpdateCoordinates(e.NewSize.Width, e.NewSize.Height);
    }


    private void ReDrawWindow(object sender, RoutedEventArgs e) {
        /// TODO: This is a viewmodel method implementation
        if (ViewModel.Model.Image is null) {
            return;
        }
        GridCanvas.Children.Clear();

        // Defer drawing grid to allow layout pass to complete
        _ = Dispatcher.BeginInvoke(new Action(() => {
            try {
                AnnotatorViewModel.DrawGrid(GridCanvas);
            } catch (Exception ex) {
                _ = MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }), System.Windows.Threading.DispatcherPriority.Loaded);
    }

    /// <summary>
    /// Exits the application
    /// </summary>
    private void Exit(object sender, RoutedEventArgs e) {
        Application.Current.Shutdown();
    }

    /// <summary>
    /// Handles the movement of the mouse over the image control. This method calculates
    /// a normalized point and feeds it to the viewmodel in order to update the Cursor
    /// Position and draw a transient annotation.
    /// </summary>
    private void Image_MouseMove(object sender, MouseEventArgs e) {
        Console.WriteLine("Mouse Move Event!");
        if (e.LeftButton == MouseButtonState.Pressed) {
            AnnotationCanvasClick(sender, e);
            return;
        }
        // Get the current mouse position. This however is not normalized to the size
        // of the image. As the window may be resized.
        Point position = e.GetPosition(ImageDisplayControl);

        System.Drawing.Size s = new() {
            Width = (int)ImageDisplayControl.ActualWidth,
            Height = (int)ImageDisplayControl.ActualHeight,
        };

        System.Drawing.Point np = new() {
            X = (int)position.X,
            Y = (int)position.Y,
        };

        ViewModel.UpdateCursorPosition(np, s);
        e.Handled = true;
    }
} //End of class