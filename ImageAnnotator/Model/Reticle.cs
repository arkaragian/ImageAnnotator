using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.Windows.Media.Media3D;

namespace SnapShotAnnotation.Model
{

    public struct ReticleLine
    {
        public PointF Start;
        public PointF End;

        public ReticleLine(Line l) {
            this.Start = new PointF()
            {
                X = (float)l.X1,
                Y = (float)l.Y1
            };
            this.End = new PointF()
            {
                X = (float)l.X2,
                Y = (float)l.Y2
            };
        }

        public Line ToLine() {
            return new Line()
            {
                X1 = (float)this.Start.X,
                Y1 = (float)this.Start.Y,
                X2 = (float)this.End.X,
                Y2 = (float)this.End.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
        }
    }

    public struct ReticleTextBlock
    {
        public string TextInput;
        public PointF SetPoint;

        public ReticleTextBlock(TextBlock textBlock)
        {
            this.TextInput = new string(textBlock.Text);
            this.SetPoint = new PointF()
            {
                X = (float)Canvas.GetLeft(textBlock),
                Y = (float)Canvas.GetTop(textBlock)
            };
        }

        public TextBlock ToTextBlock()
        {
            TextBlock textBlock = new TextBlock()
            {
                Text = TextInput,
                FontSize = 20,
                Foreground = Brushes.Black,
                Width = 30,
                Height = 20,
            };

            Canvas.SetLeft(textBlock, SetPoint.X);
            Canvas.SetTop(textBlock, SetPoint.Y);

            return textBlock;
        }
    }

        public class ReticleData
    {
        public List<ReticleLine> ReticleLines { get; set; }

        public List<ReticleTextBlock> ReticleTextBlocks { get; set; }

        public ReticleData()
        {
            ReticleLines = new List<ReticleLine>();
            ReticleTextBlocks = new List<ReticleTextBlock>();
        }
    }

        public class Reticle {
            /// <summary>
            /// The totality of lines that comprise the reticle.
            /// </summary>
            public List<Line> ReticleLines { get; set; }

            /// <summary>
            /// The totality of text blocks that describe the reticle.
            /// </summary>
            public List<TextBlock> ReticleTextBlocks { get; set; }


            public Reticle() {
                ReticleLines = new List<Line>();
                ReticleTextBlocks = new List<TextBlock>();
            }

            public void ConvertLine(Line line) {
                ReticleLine result = new ReticleLine(line);
            }

            public void AddLine(Line line) {
                ReticleLines.Add(line);
            }

            public void AddTextBlock(TextBlock text) {
                ReticleTextBlocks.Add(text);
            }

            public void RemoveLine(Line line) {
                ReticleLines.Remove(line);
            }

            public void RemoveTextBlock(TextBlock text) {
                ReticleTextBlocks.Remove(text);
            }


            public static Reticle? Load(string filename) {
                Reticle loaded = new Reticle();

                try {
                    XmlSerializer ser = new XmlSerializer(typeof(ReticleData));
                    
                    TextReader textReader = new StreamReader(filename);

                    ReticleData reticleData = (ReticleData)ser.Deserialize(textReader);
                    textReader.Close();

                    if (reticleData is null)
                    {
                        return null;
                    }

                    if (reticleData.ReticleLines is null) {
                        return null;
                    }
                    if (reticleData.ReticleLines.Count == 0) {
                        return null;
                    }

                    foreach (var l in reticleData.ReticleLines) {
                        loaded.AddLine(l.ToLine());
                    }

                if (reticleData.ReticleTextBlocks is not null && reticleData.ReticleTextBlocks.Count != 0) {
                    foreach (var t in reticleData.ReticleTextBlocks) {
                        loaded.AddTextBlock(t.ToTextBlock());
                    }
                }

                    return loaded;
                }
                catch (Exception ex) {
                    // Handle exceptions here, such as file not found or invalid XML.
                    // You can log the exception or take appropriate action.
                    Console.WriteLine("Error loading data: " + ex.Message);
                    return null;
                    //return null; // Return null or an empty list as appropriate for your application.
                }

                //foreach (var item in reticleLines) {
                //    AddLine(item);
                //}

            }

            public void Save(string filename) {
            //List<ReticleLine> lines = new();
            ////List<TextBlock> textBlocks = new List<TextBlock>();

            //foreach (Line line in ReticleLines)
            //{
            //    lines.Add(new ReticleLine(line));
            //}

            ReticleData reticleData = new ReticleData();
            reticleData.ReticleLines = ReticleLines.Select(line => new ReticleLine(line)).ToList();
            reticleData.ReticleTextBlocks = ReticleTextBlocks.Select(textBlock => new ReticleTextBlock(textBlock)).ToList();




            //foreach (TextBlock block in textBlocks)
            //{
            //    textBlocks.Add(block);
            //}

            XmlSerializer ser = new XmlSerializer(typeof(ReticleData));

                using (TextWriter textWriter = new StreamWriter(filename)) {
                    ser.Serialize(textWriter, reticleData);
                }

                string result = JsonSerializer.Serialize(reticleData);

                Trace.WriteLine(result);
            }


            internal void Save(object savefiledialog) {
                throw new NotImplementedException();
            }

            public void SaveCanvasToFile(Canvas canvas, string filename) {

                // Get the size of canvas
                System.Windows.Size size = new System.Windows.Size(canvas.Width, canvas.Height);
                // Measure and arrange the surface
                // VERY IMPORTANT
                canvas.Measure(size);
                canvas.Arrange(new Rect(size));

                RenderTargetBitmap renderBitmap =
                  new RenderTargetBitmap(
                    (int)size.Width,
                    (int)size.Height,
                    96,
                    96,
                    PixelFormats.Default);
                renderBitmap.Render(canvas);

                CroppedBitmap croppedBitmap = new CroppedBitmap(renderBitmap, new Int32Rect(640-480-((640-480)/2), 480-360- ((480 - 360) / 2), 480, 360));

                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawImage(croppedBitmap, new Rect(0, 0, croppedBitmap.Width, croppedBitmap.Height));
                }


                RenderTargetBitmap renderTarget = new RenderTargetBitmap(480, 360, 96, 96, PixelFormats.Default);
                renderTarget.Render(drawingVisual);

                BmpBitmapEncoder encoder = new BmpBitmapEncoder();

                MemoryStream memoryStream = new MemoryStream();


                encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                encoder.Save(memoryStream);

                // Create a file stream for saving image
                using (FileStream outStream = new FileStream(filename, FileMode.Create)) {
                    memoryStream.WriteTo(outStream);
                }
            }
        }
    }
