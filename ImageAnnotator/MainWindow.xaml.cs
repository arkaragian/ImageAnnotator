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
            Exception? r = ImageView.LoadImage(openDialog.FileName);
            if (r is not null) {
                _ = MessageBox.Show(r.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        } else {
            _ = MessageBox.Show("Cannot Select a Picture", "Error");
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
        // Get the current mouse position relative to the Canvas
        Point position = e.GetPosition(ImageDisplayControl);

        ImageView.UpdateCursorPosition(new System.Drawing.Point() { X = (int)position.X, Y = (int)position.Y });
    }

}