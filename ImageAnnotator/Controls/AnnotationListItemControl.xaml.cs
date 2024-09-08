using System.Windows;
using System.Windows.Controls;

namespace ImageAnnotator.Controls;

public partial class AnnotationListItemControl : UserControl {

    public string AnnotationName {
        get => (string)GetValue(AnnotationNameProperty);
        set => SetValue(AnnotationNameProperty, value);
    }

    public AnnotationListItemControl() {
        InitializeComponent();
    }

    // private static void AnnotationNameChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    //     AnnotationListItemControl? control = d as AnnotationListItemControl;
    //     if (control is not null && control.AnnotationNamexx is not null) {
    //         control.AnnotationNamexx.Content = e.NewValue.ToString();
    //     }
    // }

    public static readonly DependencyProperty AnnotationNameProperty = DependencyProperty.Register(
    "AnnotationName",
    typeof(string),
    typeof(AnnotationListItemControl),
    new PropertyMetadata(null)
    );
}