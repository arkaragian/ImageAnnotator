using SnapShotAnnotation.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SnapShotAnnotation.ViewModel {
    /// <summary>
    /// The view model that is currently displayed
    /// </summary>
    public class ReticleViewModel {

        Reticle Reticle { get; set; }

        /// <summary>
        /// The cursor highlight to show which part is being erased.
        /// </summary>
        public Ellipse eraserCursorHightlight { get; set; }

        /// <summary>
        /// Holds the current drawing state of the view
        /// </summary>
        public DrawingState State { get; set; }

        /// <summary>
        /// The point that will be used when we are starting a line but have not yet clicked the end of the line
        /// </summary>
        public Point? TransientStartPoint { get; set; }


        /// <summary>
        /// The end point that helps us paint a line before actually storing it in the reticle
        /// </summary>
        public Point? TransientEndPoint { get; set; }
        public Line? LastTransient { get; set; }
        public Line? TransientLine {
            get {
                if (TransientStartPoint is null || TransientEndPoint is null) {
                    return null;
                }
                var l = new Line {
                    StrokeThickness = 1,
                    Stroke = Brushes.Black,
                    X1 = TransientStartPoint.Value.X,
                    Y1 = TransientStartPoint.Value.Y,
                    X2 = TransientEndPoint.Value.X,
                    Y2 = TransientEndPoint.Value.Y,
                };
                l.Uid = "TransientLine";
                return l;
            }
        }

        public List<Line> LineSet {
            get {
                return Reticle.ReticleLines;
            }
        }

        public List<TextBlock> TextBlocksSet {
            get {
                return Reticle.ReticleTextBlocks;
            }
        }

        public ReticleViewModel() {
            Reticle = new Reticle();
            State = DrawingState.Idle;
        }

        public void TransitToState(DrawingState newState) {
            State = newState;
            //switch (State) {
            //    case DrawingState.Idle:
            //        switch (newState) {
            //            case DrawingState.Idle:
            //                return;
            //            case DrawingState.StartLineClicked:
            //                break;
            //        }
            //        return;
            //    case DrawingState.StartLineClicked:
            //        return;
            //    default:
            //        throw new InvalidOperationException("Invalid Operation");
            //}

        }

        public void FinishLine(Point p) {
            if(State != DrawingState.StartLineClicked) {
                throw new InvalidOperationException("Cannot finish a line if a line has not started");
            }
            var l = new Line {
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                X1 = TransientStartPoint!.Value.X,
                Y1 = TransientStartPoint.Value.Y,
                X2 = p.X,
                Y2 = p.Y,
            };
            Reticle.AddLine(l);

            //State = DrawingState.Idle;

            this.TransientStartPoint = null;
            this.TransientEndPoint = null;
            this.LastTransient = null;

        }

        public void EraseLine(Line line)
        {
            Reticle.RemoveLine(line);
        }

        public void FinishTextBlock(Canvas canvas, TextBox textBox)
        {
            if (textBox != null)
            {
                // Get the user-entered text
                string enteredText = textBox.Text;

                if (!string.IsNullOrEmpty(enteredText))
                {
                    // Create a TextBlock with the entered text
                    TextBlock textBlock = new TextBlock
                    {
                        Text = enteredText,
                        FontSize = 20,
                        Foreground = Brushes.Black
                    };

                    canvas.Children.Add(textBlock);
                    Reticle.AddTextBlock(textBlock);


                    Point position = new Point(Canvas.GetLeft(textBox), Canvas.GetTop(textBox));
                    Canvas.SetLeft(textBlock, position.X);
                    Canvas.SetTop(textBlock, position.Y);

                    // Remove the TextBox from the Canvas
                    canvas.Children.Remove(textBox);
                }
            }
        }

        public void EraseTextBlock(TextBlock textBlock)
        {
            Reticle.RemoveTextBlock(textBlock);
        }

        public void Save(string filename)
        {
            Reticle.Save(filename);
        }
        public bool Load(string filename)
        {
            Reticle? r  = Reticle.Load(filename);
            if(r is not null)
            {
                this.Reticle = r;
                return true;
            } else { return false; }
        }

        public void SaveImage(Canvas canvas, string filename)
        {
            canvas.Children.Clear();

            List<Line> lines = LineSet;
            List<TextBlock> textBlocks = TextBlocksSet;
            foreach (Line line in lines)
            {
                canvas.Children.Add(line);
            }
            foreach (TextBlock textBlock in textBlocks)
            {
                canvas.Children.Add(textBlock);
            }

            Reticle.SaveCanvasToFile(canvas, filename);

            canvas.Children.Clear();
        }
    }
}
