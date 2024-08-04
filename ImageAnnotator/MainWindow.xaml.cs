using ImageAnnotator.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;

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
    private readonly ImageViewModel ImageView;

    public MainWindow() {
        InitializeComponent();
        ImageView = new() {
            ImageModel = new(),
            AnnotationCanvas = AnnotationCanvas,
            GridCanvas = GridCanvas
        };
        Title = "Image Annotator";
        DataContext = ImageView;
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
            Exception? r = ImageView.LoadImage(openDialog.FileName);
            if (r is not null) {
                _ = MessageBox.Show(r.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>
    /// Exits the application
    /// </summary>
    private void Exit(object sender, RoutedEventArgs e) {
        Application.Current.Shutdown();
    }

    /// <summary>
    /// Handles the movement of the mouse over the image control
    /// </summary>
    private void Image_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
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

        ImageView.UpdateCursorPosition(np, s);
    }

}