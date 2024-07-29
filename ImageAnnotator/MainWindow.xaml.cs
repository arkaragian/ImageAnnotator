using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using SnapShotAnnotation.Model;
using SnapShotAnnotation.ViewModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace SnapShotAnnotation
{

    public enum DrawingState
    {
        Idle,
        StartLineClicked,
        Eraser,
        Text,
    }

    public partial class MainWindow : Window
    {
        private Double zoomMax = 5;
        private Double zoomMin = 0.5;
        private Double zoomSpeed = 0.001;
        private Double zoom = 1;

        private List<Line> lines = new List<Line>();
        private Point endPoint;
        private Line currentLine;

        private TextBox textBox;

        private bool isEraserMode = false;
        private double eraserRadius = 10.0; // Adjust the radius as needed


        private ReticleViewModel ViewModel = new ReticleViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "MLT-SnapShot Annotation";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGrid(MainCanvas);
        }

        // Zoom on Mouse wheel
        private void MainCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += zoomSpeed * e.Delta; // Ajust zooming speed (e.Delta = Mouse spin value )
            if (zoom < zoomMin) { zoom = zoomMin; } // Limit Min Scale
            if (zoom > zoomMax) { zoom = zoomMax; } // Limit Max Scale

            Point mousePos = e.GetPosition(MainCanvas);

            if (zoom > 1)
            {
                MainCanvas.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y); // transform Canvas size from mouse position
            }
            else
            {
                MainCanvas.RenderTransform = new ScaleTransform(zoom, zoom); // transform Canvas size
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.FinishTextBlock(MainCanvas, textBox);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Set focus to another element to trigger LostFocus
                MainCanvas.Focus();
            }
            else if (e.Key == Key.Tab)
            {
                MainCanvas.Focus();
            }
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (ViewModel.State == DrawingState.Eraser)
            {
                Point cursorPosition = e.GetPosition(MainCanvas);
                List<Line> lines = new List<Line>();
                List<Line> linesToRemove = new List<Line>();
                List<TextBlock> textBlocks = new List<TextBlock>();
                List<TextBlock> textBlocksToRemove = new List<TextBlock>();
                lines = ViewModel.LineSet;
                textBlocks = ViewModel.TextBlocksSet;

                foreach (Line line in lines)
                {
                    if (IsLinePointWithinEraserRadius(line, cursorPosition))
                    {
                        linesToRemove.Add(line);
                    }
                }

                foreach (TextBlock textBlock in textBlocks)
                {
                    if (IsTextBlockPointWithinEraserRadius(textBlock, cursorPosition))
                    {
                        textBlocksToRemove.Add(textBlock);
                    }
                }

                // Remove the lines after the loop to avoid modifying the collection during iteration.
                foreach (Line lineToRemove in linesToRemove)
                {
                    ViewModel.EraseLine(lineToRemove);
                }

                foreach (TextBlock textBlockToRemove in textBlocksToRemove)
                {
                    ViewModel.EraseTextBlock(textBlockToRemove);
                }

                // Redraw the canvas.
                ReDraw();
            }

            if (ViewModel.State == DrawingState.Text)
            {
                // Create a TextBox dynamically
                textBox = new TextBox
                {
                    Width = 30,
                    Height = 20
                };

                // Add the TextBox to the Canvas
                MainCanvas.Children.Add(textBox);

                // Set the TextBox's position on the Canvas
                Point position = e.GetPosition(MainCanvas);
                Canvas.SetLeft(textBox, position.X);
                Canvas.SetTop(textBox, position.Y);

                textBox.PreviewKeyDown += TextBox_PreviewKeyDown;

                // Set up an event handler for when the TextBox loses focus
                textBox.LostFocus += TextBox_LostFocus;

                ViewModel.State = DrawingState.Idle;

                return;

            }

            if (ViewModel.State == DrawingState.Idle)
            {
                // If not in eraser mode, start drawing a line as before.
                Point startPoint = e.GetPosition(MainCanvas);
                ViewModel.TransientStartPoint = startPoint;
                ViewModel.State = DrawingState.StartLineClicked;
                e.Handled = false;
                return;
            }

            if (ViewModel.State == DrawingState.StartLineClicked)
            {
                Point point = e.GetPosition(MainCanvas);
                ViewModel.FinishLine(point);
                ReDraw();
                ViewModel.State = DrawingState.Idle;
                e.Handled = false;
                return;
            }

        }

        private void ReDraw()
        {

            MainCanvas.Children.Clear();

            DrawGrid(MainCanvas);
            List<Line> lines = ViewModel.LineSet;
            List<TextBlock> textBlocks = ViewModel.TextBlocksSet;
            foreach (Line line in lines)
            {
                MainCanvas.Children.Add(line);
            }
            foreach (TextBlock textBlock in textBlocks)
            {
                MainCanvas.Children.Add(textBlock);
            }
        }



        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {

            if (ViewModel.State == DrawingState.Eraser)
            {
                if (ViewModel.eraserCursorHightlight is not null)
                {
                    ViewModel.eraserCursorHightlight.MouseLeftButtonDown += MainCanvas_MouseLeftButtonDown;
                    MainCanvas.Children.Remove(ViewModel.eraserCursorHightlight);
                }

                Ellipse cursorHighlight = new Ellipse();
                cursorHighlight.Stroke = Brushes.Black;
                cursorHighlight.Fill = Brushes.Black;

                double circleDiameter = 2 * eraserRadius; // Set the circle's diameter
                cursorHighlight.Width = circleDiameter;
                cursorHighlight.Height = circleDiameter;

                Point cursorPosition = Mouse.GetPosition(MainCanvas);
                cursorHighlight.RenderTransform = new TranslateTransform(cursorPosition.X - circleDiameter / 2, cursorPosition.Y - circleDiameter / 2);


                ViewModel.eraserCursorHightlight = cursorHighlight;

                MainCanvas.Children.Add(ViewModel.eraserCursorHightlight);
            }

            if (ViewModel.State == DrawingState.StartLineClicked)
            {
                // If not in eraser mode and drawing, update the line in real-time.
                ViewModel.TransientEndPoint = e.GetPosition(MainCanvas);
                Line? l = ViewModel.TransientLine;

                if (ViewModel.LastTransient is not null)
                {
                    ViewModel.LastTransient.MouseLeftButtonDown += MainCanvas_MouseLeftButtonDown;
                    MainCanvas.Children.Remove(ViewModel.LastTransient);
                }

                if (l is not null)
                {
                    MainCanvas.Children.Add(l);
                    ViewModel.LastTransient = l;
                }

                //if (e.LeftButton == MouseButtonState.Pressed)
                //{

                //    FinishMove(ViewModel.TransientEndPoint.Value);
                //}
                //e.Handled = true;
            }
            else
            {
            }
        }

        private void DrawGrid(Canvas canvas)
        {
            double HCanvas = MainCanvas.ActualHeight;
            double WCanvas = MainCanvas.ActualWidth;

            Border border = new Border()
            {
                Width = WCanvas,
                Height = HCanvas,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.DarkOliveGreen,
                BorderThickness = new Thickness(3)
            };

            canvas.Children.Add(border);

            for (int i = 0; i < HCanvas / 20; i++)
            {
                double space = i * 20;
                Line line = new Line()
                {
                    X1 = 0,
                    X2 = WCanvas,
                    Y1 = space,
                    Y2 = space,
                    StrokeThickness = 1,
                    Stroke = Brushes.Black,

                };
                canvas.Children.Add(line);
            }

            for (int i = 0; i < WCanvas / 20; i++)
            {
                double space = i * 20;
                Line line = new Line()
                {
                    Y1 = 0,
                    Y2 = HCanvas,
                    X1 = space,
                    X2 = space,
                    StrokeThickness = 1,
                    Stroke = Brushes.Black
                };

                canvas.Children.Add(line);
            }
        }

        private bool IsLinePointWithinEraserRadius(Line line, Point point)
        {
            Point lineStart = new Point(line.X1, line.Y1);
            Point lineEnd = new Point(line.X2, line.Y2);

            double distanceFromLine = Math.Abs((lineEnd.Y - lineStart.Y) * point.X - (lineEnd.X - lineStart.X) * point.Y + lineEnd.X * lineStart.Y - lineEnd.Y * lineStart.X)
                / Math.Sqrt(Math.Pow(lineEnd.Y - lineStart.Y, 2) + Math.Pow(lineEnd.X - lineStart.X, 2));

            // Check if the distance from the point to the line is within the eraser radius.
            return distanceFromLine <= eraserRadius;
        }

        private bool IsTextBlockPointWithinEraserRadius(TextBlock textBlock, Point point)
        {
            Point textBlockStart = new Point(Canvas.GetLeft(textBlock), Canvas.GetTop(textBlock));
            double textBlockWidth = textBlock.ActualWidth;
            double textBlockHeight = textBlock.ActualHeight;

            double closestX = Math.Max(textBlockStart.X, Math.Min(point.X, textBlockStart.X + textBlockWidth));
            double closestY = Math.Max(textBlockStart.Y, Math.Min(point.Y, textBlockStart.Y + textBlockHeight));

            double distance = Math.Sqrt((point.X - closestX) * (point.X - closestX) + (point.Y - closestY) * (point.Y - closestY));

            return distance <= eraserRadius;
        }

        // Toggle eraser mode.
        private void EraserButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.State != DrawingState.Eraser)
            {
                ViewModel.State = DrawingState.Eraser;
            }
            else
            {
                ViewModel.State = DrawingState.Idle;
                ReDraw();
            }

            if (ViewModel.State != DrawingState.Eraser)
            {
                EraserButton.Content = "Eraser"; // Change button text to indicate drawing mode.
            }
            else
            {
                EraserButton.Content = "Drawing"; // Change button text to indicate eraser mode.
            }
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            ReDraw();
            ViewModel.State = DrawingState.Text;
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            MainCanvas.Background = Brushes.White; ;

            MainCanvas.Children.Clear();

            DrawGrid(MainCanvas);

            ViewModel = new ReticleViewModel();

            ViewModel.State = DrawingState.Idle;
        }

        private void LoadReticleButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() == true)
            {
                bool ok = ViewModel.Load(openFileDialog.FileName);
                if (ok)
                {
                    ReDraw();
                }
                else
                {
                    //TODO: Display an error message?
                }


            }
        }
        private void LoadSnapShotButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp"; //+
                                                                           //"JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                                                                           //"Portable Network Graphic (*.png)|*.png|" +
                                                                           //"BitMap Images (*.bmp)|*.bmp|";
            if (op.ShowDialog() == true)
            {
                string imagePath = op.FileName;

                // Clear existing children of the Canvas
                MainCanvas.Children.Clear();

                //imgPhoto.Source = new BitmapImage(new Uri(imagePath));


                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri(imagePath));

                MainCanvas.Background = imageBrush;
                ReDraw();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML files (*.xml)|*.xml";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ViewModel.Save(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
        }

        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            Canvas saveCanvas = new Canvas();

            saveCanvas.Width = 640;
            saveCanvas.Height = 480;
            saveCanvas.Background = Brushes.White;


            MainCanvas.Children.Clear();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BitMap files (*.bmp)|*.bmp";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ViewModel.SaveImage(saveCanvas, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
            ReDraw();
        }
    }
}