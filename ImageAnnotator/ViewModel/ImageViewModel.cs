using ImageAnnotator.Model;
using System.ComponentModel;

namespace ImageAnnotator.ViewModel;

/// <summary>
/// The view model that is currently displayed
/// </summary>
public class ImageViewModel : INotifyPropertyChanged {


    private ImageModel _imageModel;

    /// <summary>
    /// The image model that this view model deals with
    /// </summary>
    public ImageModel ImageModel {
        get => _imageModel;
        set {
            _imageModel = value;
            OnPropertyChanged(nameof(ImageModel));
            OnPropertyChanged(nameof(ImagePath));
            OnPropertyChanged(nameof(ImageDisplayPath));
        }
    }

    /// <summary>
    /// The path of the image that the model holds
    /// </summary>
    public string? ImagePath {
        get => ImageModel.ImagePath;
        set {
            ImageModel.ImagePath = value;
            OnPropertyChanged(nameof(ImagePath));
            OnPropertyChanged(nameof(ImageDisplayPath));
        }
    }

    /// <summary>
    /// The path of the image that is displayed to the user
    /// </summary>
    public string ImageDisplayPath => ImageModel.ImagePath ?? "No Image";

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Save(string filename) {
        // ImageModel.Save(filename);
    }
    public bool Load(string filename) {
        // Reticle? r = Reticle.Load(filename);
        // if (r is not null) {
        //     this.Reticle = r;
        //     return true;
        // } else { return false; }
        return true;
    }

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