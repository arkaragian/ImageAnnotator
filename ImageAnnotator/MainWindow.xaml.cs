using ImageAnnotator.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;

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
    private readonly AnnotatorViewModel ViewModel;

    public MainWindow() {
        InitializeComponent();
        ViewModel = new() {
            ImageModel = new(),
            AnnotationCanvas = AnnotationCanvas,
            GridCanvas = GridCanvas
        };
        Title = "Image Annotator";
        DataContext = ViewModel;
        WindowInfo.DataContext = this;
    }

    private void InsertNodeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
        e.CanExecute = ViewModel.CurrentInputState is InputState.Idle or InputState.WaitingForInput;
    }

    private void InsertNodeCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
        //Implement command logic here
        ViewModel.InsertionAction(InsertionType.Node, point: null);
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

    private void InsertionAction(object sender, MouseEventArgs e) {
        //The point at which the insertion will happen
        Point p = e.GetPosition(AnnotationCanvas);
        ViewModel.InsertionAction(InsertionType.Node, p);
    }


    private void ReDrawWindow(object sender, RoutedEventArgs e) {
        /// TODO: This is a viewmodel method implementation
        if (ViewModel.ImageModel.Image is null) {
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

        ViewModel.UpdateCursorPosition(np, s);
    }

}