using ImageAnnotator.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ImageAnnotator;

public enum DrawingState {
    Idle,
    StartLineClicked,
    Eraser,
    Text,
}

public partial class MainWindow : Window {

    /// <summary>
    /// The model of the image
    /// </summary>
    public readonly ImageViewModel ImageView;

    public MainWindow() {
        InitializeComponent();
        ImageView = new() {
            ImageModel = new()
        };
        Title = "Image Annotator";
        this.DataContext = ImageView;
        WindowInfo.DataContext = this;
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

            //ImageView.ImageModel.ImagePath = openDialog.FileName;
            Exception? r = ImageView.LoadImage(openDialog.FileName);

            if (r is not null) {
                MessageBox.Show(r.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //imgPhoto.Source = new BitmapImage(new Uri(imagePath));


            // ImageBrush imageBrush = new() {
            //     ImageSource = new BitmapImage(new Uri(imagePath))
            // };
            //
            // MainCanvas.Background = imageBrush;
            // ReDraw();
        } else {
            _ = MessageBox.Show("Cannot Select a Picture", "Error");
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
        //DrawGrid(MainCanvas);
    }

    private void Image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
        // Get the current mouse position relative to the Canvas
        Point position = e.GetPosition(ImageDisplayControl);

        ImageView.UpdateCursorPosition(new System.Drawing.Point() { X=(int)position.X, Y=(int)position.Y});
    }

    // Zoom on Mouse wheel
    //private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e) {
    //    zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
    //    if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
    //    if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

    //    Point mousePos = e.GetPosition(MainCanvas);

    //    if (zoom > 1) {
    //        MainCanvas.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y); // transform Canvas size from mouse position
    //    } else {
    //        MainCanvas.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
    //    }
    //}

    //private void TextBox_LostFocus(object sender, RoutedEventArgs e) {
    //    ViewModel.FinishTextBlock(MainCanvas, textBox);
    //}

    //private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e) {
    //    if (e.Key == Key.Enter) {
    //        // Set focus to another element to trigger LostFocus
    //        MainCanvas.Focus();
    //    } else if (e.Key == Key.Tab) {
    //        MainCanvas.Focus();
    //    }
    //}

    //private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

    //    if (ViewModel.State == DrawingState.Eraser) {
    //        Point cursorPosition = e.GetPosition(MainCanvas);
    //        List<Line> lines = new List<Line>();
    //        List<Line> linesToRemove = new List<Line>();
    //        List<TextBlock> textBlocks = new List<TextBlock>();
    //        List<TextBlock> textBlocksToRemove = new List<TextBlock>();
    //        lines = ViewModel.LineSet;
    //        textBlocks = ViewModel.TextBlocksSet;

    //        foreach (Line line in lines) {
    //            if (IsLinePointWithinEraserRadius(line, cursorPosition)) {
    //                linesToRemove.Add(line);
    //            }
    //        }

    //        foreach (TextBlock textBlock in textBlocks) {
    //            if (IsTextBlockPointWithinEraserRadius(textBlock, cursorPosition)) {
    //                textBlocksToRemove.Add(textBlock);
    //            }
    //        }

    //        // Remove the lines after the loop to avoid modifying the collection during iteration.
    //        foreach (Line lineToRemove in linesToRemove) {
    //            ViewModel.EraseLine(lineToRemove);
    //        }

    //        foreach (TextBlock textBlockToRemove in textBlocksToRemove) {
    //            ViewModel.EraseTextBlock(textBlockToRemove);
    //        }

    //        // Redraw the canvas.
    //        ReDraw();
    //    }

    //    if (ViewModel.State == DrawingState.Text) {
    //        // Create a TextBox dynamically
    //        textBox = new TextBox {
    //            Width = 30,
    //            Height = 20
    //        };

    //        // Add the TextBox to the Canvas
    //        MainCanvas.Children.Add(textBox);

    //        // Set the TextBox's position on the Canvas
    //        Point position = e.GetPosition(MainCanvas);
    //        Canvas.SetLeft(textBox, position.X);
    //        Canvas.SetTop(textBox, position.Y);

    //        textBox.PreviewKeyDown += TextBox_PreviewKeyDown;

    //        // Set up an event handler for when the TextBox loses focus
    //        textBox.LostFocus += TextBox_LostFocus;

    //        ViewModel.State = DrawingState.Idle;

    //        return;

    //    }

    //    if (ViewModel.State == DrawingState.Idle) {
    //        // If not in eraser mode, start drawing a line as before.
    //        Point startPoint = e.GetPosition(MainCanvas);
    //        ViewModel.TransientStartPoint = startPoint;
    //        ViewModel.State = DrawingState.StartLineClicked;
    //        e.Handled = false;
    //        return;
    //    }

    //    if (ViewModel.State == DrawingState.StartLineClicked) {
    //        Point point = e.GetPosition(MainCanvas);
    //        ViewModel.FinishLine(point);
    //        ReDraw();
    //        ViewModel.State = DrawingState.Idle;
    //        e.Handled = false;
    //        return;
    //    }

    //}

    /// <summary>
    /// Redraw the screen
    /// </summary>
    //private void ReDraw() {
    //    MainCanvas.Children.Clear();
    //    DrawGrid(MainCanvas);
    //    List<Line> lines = ViewModel.LineSet;
    //    List<TextBlock> textBlocks = ViewModel.TextBlocksSet;
    //    foreach (Line line in lines) {
    //        _ = MainCanvas.Children.Add(line);
    //    }
    //    foreach (TextBlock textBlock in textBlocks) {
    //        _ = MainCanvas.Children.Add(textBlock);
    //    }
    //}



    //private void MainCanvas_MouseMove(object sender, MouseEventArgs e) {

    //    if (ViewModel.State == DrawingState.Eraser) {
    //        if (ViewModel.eraserCursorHightlight is not null) {
    //            ViewModel.eraserCursorHightlight.MouseLeftButtonDown += MainCanvas_MouseLeftButtonDown;
    //            MainCanvas.Children.Remove(ViewModel.eraserCursorHightlight);
    //        }

    //        Ellipse cursorHighlight = new Ellipse();
    //        cursorHighlight.Stroke = Brushes.Black;
    //        cursorHighlight.Fill = Brushes.Black;

    //        double circleDiameter = 2 * eraserRadius; // Set the circle's diameter
    //        cursorHighlight.Width = circleDiameter;
    //        cursorHighlight.Height = circleDiameter;

    //        Point cursorPosition = Mouse.GetPosition(MainCanvas);
    //        cursorHighlight.RenderTransform = new TranslateTransform(cursorPosition.X - circleDiameter / 2, cursorPosition.Y - circleDiameter / 2);


    //        ViewModel.eraserCursorHightlight = cursorHighlight;

    //        MainCanvas.Children.Add(ViewModel.eraserCursorHightlight);
    //    }

    //    if (ViewModel.State == DrawingState.StartLineClicked) {
    //        // If not in eraser mode and drawing, update the line in real-time.
    //        ViewModel.TransientEndPoint = e.GetPosition(MainCanvas);
    //        Line? l = ViewModel.TransientLine;

    //        if (ViewModel.LastTransient is not null) {
    //            ViewModel.LastTransient.MouseLeftButtonDown += MainCanvas_MouseLeftButtonDown;
    //            MainCanvas.Children.Remove(ViewModel.LastTransient);
    //        }

    //        if (l is not null) {
    //            MainCanvas.Children.Add(l);
    //            ViewModel.LastTransient = l;
    //        }

    //        //if (e.LeftButton == MouseButtonState.Pressed)
    //        //{

    //        //    FinishMove(ViewModel.TransientEndPoint.Value);
    //        //}
    //        //e.Handled = true;
    //    } else {
    //    }
    //}

    /// <summary>
    /// Draws a grid for an image
    /// </summary>
    //private void DrawGrid(Canvas canvas) {
    //    double HCanvas = MainCanvas.ActualHeight;
    //    double WCanvas = MainCanvas.ActualWidth;

    //    Border border = new() {
    //        Width = WCanvas,
    //        Height = HCanvas,
    //        Background = Brushes.Transparent,
    //        BorderBrush = Brushes.DarkOliveGreen,
    //        BorderThickness = new Thickness(3)
    //    };

    //    _ = canvas.Children.Add(border);

    //    for (int i = 0; i < HCanvas / 20; i++) {
    //        double space = i * 20;
    //        Line line = new() {
    //            X1 = 0,
    //            X2 = WCanvas,
    //            Y1 = space,
    //            Y2 = space,
    //            StrokeThickness = 1,
    //            Stroke = Brushes.Black,

    //        };
    //        canvas.Children.Add(line);
    //    }

    //    for (int i = 0; i < WCanvas / 20; i++) {
    //        double space = i * 20;
    //        Line line = new Line() {
    //            Y1 = 0,
    //            Y2 = HCanvas,
    //            X1 = space,
    //            X2 = space,
    //            StrokeThickness = 1,
    //            Stroke = Brushes.Black
    //        };

    //        canvas.Children.Add(line);
    //    }
    //}

    //private bool IsLinePointWithinEraserRadius(Line line, Point point) {
    //    Point lineStart = new Point(line.X1, line.Y1);
    //    Point lineEnd = new Point(line.X2, line.Y2);

    //    double distanceFromLine = Math.Abs((lineEnd.Y - lineStart.Y) * point.X - (lineEnd.X - lineStart.X) * point.Y + lineEnd.X * lineStart.Y - lineEnd.Y * lineStart.X)
    //        / Math.Sqrt(Math.Pow(lineEnd.Y - lineStart.Y, 2) + Math.Pow(lineEnd.X - lineStart.X, 2));

    //    // Check if the distance from the point to the line is within the eraser radius.
    //    return distanceFromLine <= eraserRadius;
    //}

    //private bool IsTextBlockPointWithinEraserRadius(TextBlock textBlock, Point point) {
    //    Point textBlockStart = new Point(Canvas.GetLeft(textBlock), Canvas.GetTop(textBlock));
    //    double textBlockWidth = textBlock.ActualWidth;
    //    double textBlockHeight = textBlock.ActualHeight;

    //    double closestX = Math.Max(textBlockStart.X, Math.Min(point.X, textBlockStart.X + textBlockWidth));
    //    double closestY = Math.Max(textBlockStart.Y, Math.Min(point.Y, textBlockStart.Y + textBlockHeight));

    //    double distance = Math.Sqrt((point.X - closestX) * (point.X - closestX) + (point.Y - closestY) * (point.Y - closestY));

    //    return distance <= eraserRadius;
    //}

    // Toggle eraser mode.
    // private void EraserButton_Click(object sender, RoutedEventArgs e) {
    //     if (ViewModel.State != DrawingState.Eraser) {
    //         ViewModel.State = DrawingState.Eraser;
    //     } else {
    //         ViewModel.State = DrawingState.Idle;
    //         ReDraw();
    //     }
    //
    //     if (ViewModel.State != DrawingState.Eraser) {
    //         EraserButton.Content = "Eraser"; // Change button text to indicate drawing mode.
    //     } else {
    //         EraserButton.Content = "Drawing"; // Change button text to indicate eraser mode.
    //     }
    // }

    //private void AddTextButton_Click(object sender, RoutedEventArgs e) {
    //    ReDraw();
    //    ViewModel.State = DrawingState.Text;
    //}

    //private void LoadImageTest(object sender, RoutedEventArgs e) {
    //    MainCanvas.Background = Brushes.White; ;

    //    MainCanvas.Children.Clear();

    //    DrawGrid(MainCanvas);

    //    ViewModel = new ReticleViewModel {
    //        State = DrawingState.Idle
    //    };
    //}

    //private void LoadReticleButton_Click(object sender, RoutedEventArgs e) {
    //    OpenFileDialog openFileDialog = new OpenFileDialog();
    //    openFileDialog.Filter = "XML files (*.xml)|*.xml";

    //    if (openFileDialog.ShowDialog() == true) {
    //        bool ok = ViewModel.Load(openFileDialog.FileName);
    //        if (ok) {
    //            ReDraw();
    //        } else {
    //            //TODO: Display an error message?
    //        }


    //    }
    //}

    //private void SaveButton_Click(object sender, RoutedEventArgs e) {
    //    SaveFileDialog saveFileDialog = new SaveFileDialog();
    //    saveFileDialog.Filter = "XML files (*.xml)|*.xml";

    //    if (saveFileDialog.ShowDialog() == true) {
    //        try {
    //            ViewModel.Save(saveFileDialog.FileName);
    //        } catch (Exception ex) {
    //            Trace.WriteLine(ex.ToString());
    //        }
    //    }
    //}

    //private void SaveImageButton_Click(object sender, RoutedEventArgs e) {
    //    Canvas saveCanvas = new Canvas();

    //    saveCanvas.Width = 640;
    //    saveCanvas.Height = 480;
    //    saveCanvas.Background = Brushes.White;


    //    MainCanvas.Children.Clear();

    //    SaveFileDialog saveFileDialog = new SaveFileDialog();
    //    saveFileDialog.Filter = "BitMap files (*.bmp)|*.bmp";

    //    if (saveFileDialog.ShowDialog() == true) {
    //        try {
    //            ViewModel.SaveImage(saveCanvas, saveFileDialog.FileName);
    //        } catch (Exception ex) {
    //            Trace.WriteLine(ex.ToString());
    //        }
    //    }
    //    ReDraw();
    //}
}