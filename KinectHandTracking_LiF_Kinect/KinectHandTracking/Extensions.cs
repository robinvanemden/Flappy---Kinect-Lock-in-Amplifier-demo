using Microsoft.Kinect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KinectHandTracking
{
    public static class Extensions
    {
        #region Body

        public static Point Scale(this Joint joint, CoordinateMapper mapper)
        {
            var point = new Point();

            var colorPoint = mapper.MapCameraPointToColorSpace(joint.Position);
            point.X = float.IsInfinity(colorPoint.X) ? 0.0 : colorPoint.X;
            point.Y = float.IsInfinity(colorPoint.Y) ? 0.0 : colorPoint.Y;

            return point;
        }

        #endregion Body

        #region Camera

        public static ImageSource ToBitmap(this ColorFrame frame)
        {
            var width = frame.FrameDescription.Width;
            var height = frame.FrameDescription.Height;
            var format = PixelFormats.Bgr32;

            var pixels = new byte[width * height * ((format.BitsPerPixel + 7) / 8)];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
                frame.CopyRawFrameDataToArray(pixels);
            else
                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);

            var stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public static ImageSource ToBitmap(this DepthFrame frame)
        {
            var width = frame.FrameDescription.Width;
            var height = frame.FrameDescription.Height;
            var format = PixelFormats.Bgr32;

            var minDepth = frame.DepthMinReliableDistance;
            var maxDepth = frame.DepthMaxReliableDistance;

            var pixelData = new ushort[width * height];
            var pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(pixelData);

            var colorIndex = 0;
            foreach (var depth in pixelData)
            {
                var intensity = (byte) (depth >= minDepth && depth <= maxDepth ? depth : 0);

                pixels[colorIndex++] = intensity; // Blue
                pixels[colorIndex++] = intensity; // Green
                pixels[colorIndex++] = intensity; // Red

                ++colorIndex;
            }

            var stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public static ImageSource ToBitmap(this InfraredFrame frame)
        {
            var width = frame.FrameDescription.Width;
            var height = frame.FrameDescription.Height;
            var format = PixelFormats.Bgr32;

            var frameData = new ushort[width * height];
            var pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(frameData);

            var colorIndex = 0;
            foreach (var ir in frameData)
            {
                var intensity = (byte) (ir >> 7);

                pixels[colorIndex++] = (byte) (intensity / 1); // Blue
                pixels[colorIndex++] = (byte) (intensity / 1); // Green
                pixels[colorIndex++] = (byte) (intensity / 0.4); // Red

                colorIndex++;
            }

            var stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        #endregion Camera

        #region Drawing

        public static void DrawSkeleton(this Canvas canvas, Body body, CoordinateMapper mapper)
        {
            if (body == null) return;

            canvas.DrawPoint(body.Joints[JointType.Head], mapper);
            canvas.DrawPoint(body.Joints[JointType.Neck], mapper);
            canvas.DrawPoint(body.Joints[JointType.SpineShoulder], mapper);
            canvas.DrawPoint(body.Joints[JointType.ShoulderLeft], mapper);
            canvas.DrawPoint(body.Joints[JointType.ShoulderRight], mapper);
            canvas.DrawPoint(body.Joints[JointType.ElbowLeft], mapper);
            canvas.DrawPoint(body.Joints[JointType.ElbowRight], mapper);
            canvas.DrawPoint(body.Joints[JointType.WristLeft], mapper);
            canvas.DrawPoint(body.Joints[JointType.WristRight], mapper);

            canvas.DrawLine(body.Joints[JointType.Head], body.Joints[JointType.Neck], mapper);
            canvas.DrawLine(body.Joints[JointType.Neck], body.Joints[JointType.SpineShoulder], mapper);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderRight], mapper);
            canvas.DrawLine(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight], mapper);
            canvas.DrawLine(body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight], mapper);
            canvas.DrawLine(body.Joints[JointType.WristLeft], body.Joints[JointType.HandLeft], mapper);
            canvas.DrawLine(body.Joints[JointType.WristRight], body.Joints[JointType.HandRight], mapper);
        }

        public static void DrawPoint(this Canvas canvas, Joint joint, CoordinateMapper mapper)
        {
            if (joint.TrackingState == TrackingState.NotTracked) return;

            var point = joint.Scale(mapper);

            var ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.LightBlue)
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        public static Point DrawHand(this Canvas canvas, Joint hand, CoordinateMapper mapper)
        {
            if (hand.TrackingState == TrackingState.NotTracked) return new Point(float.NaN, float.NaN);

            var point = hand.Scale(mapper);

            var ellipse = new Ellipse
            {
                Width = 100,
                Height = 100,
                //Stroke = new SolidColorBrush(Colors.White),
                Stroke = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                StrokeThickness = 4
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);

            return point;
        }

        public static Point DrawHead(this Canvas canvas, Joint head, CoordinateMapper mapper)
        {
            var point = head.Scale(mapper);

            var ellipse = new Ellipse
            {
                Width = 100,
                Height = 100,
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 4
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);

            return point;
        }

        public static void DrawHandText(this Canvas canvas, Joint hand, CoordinateMapper mapper)
        {
            var point = hand.Scale(mapper);

            var textBlock = new TextBlock {FontSize = 46};

            var handText = "(" + (int) point.X + "," + (int) point.Y + ")";
            textBlock.Text = handText;
            textBlock.Foreground = new SolidColorBrush(Colors.White);

            Canvas.SetLeft(textBlock, point.X - 100);
            Canvas.SetTop(textBlock, point.Y - 200);

            canvas.Children.Add(textBlock);
        }

        public static void DrawThumb(this Canvas canvas, Joint thumb, CoordinateMapper mapper)
        {
            if (thumb.TrackingState == TrackingState.NotTracked) return;

            var point = thumb.Scale(mapper);

            var ellipse = new Ellipse
            {
                Width = 40,
                Height = 40,
                Fill = new SolidColorBrush(Colors.LightBlue),
                Opacity = 0.7
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        public static void DrawLine(this Canvas canvas, Joint first, Joint second, CoordinateMapper mapper)
        {
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked)
                return;

            var firstPoint = first.Scale(mapper);
            var secondPoint = second.Scale(mapper);

            if (firstPoint.X > 0 && firstPoint.Y > 0 && secondPoint.X > 0 && secondPoint.Y > 0)
            {
                var line = new Line
                {
                    X1 = firstPoint.X,
                    Y1 = firstPoint.Y,
                    X2 = secondPoint.X,
                    Y2 = secondPoint.Y,
                    StrokeThickness = 8,
                    Stroke = new SolidColorBrush(Colors.White)
                };

                canvas.Children.Add(line);
            }
        }

        public static void DrawLineRed(this Canvas canvas, Joint first, Joint second, CoordinateMapper mapper)
        {
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked)
                return;

            var firstPoint = first.Scale(mapper);
            var secondPoint = second.Scale(mapper);

            var line = new Line
            {
                X1 = firstPoint.X,
                Y1 = firstPoint.Y,
                X2 = secondPoint.X,
                Y2 = secondPoint.Y,
                StrokeThickness = 8,
                Stroke = new SolidColorBrush(Colors.Red)
            };

            var len =
                (int) Math.Sqrt(Math.Pow(secondPoint.Y - firstPoint.Y, 2) + Math.Pow(secondPoint.X - firstPoint.X, 2));

            canvas.Children.Add(line);

            var textBlock = new TextBlock {FontSize = 46};

            var text = "(" + len + ")";
            textBlock.Text = text;
            textBlock.Foreground = new SolidColorBrush(Colors.Red);

            Canvas.SetLeft(textBlock, firstPoint.X + 200);
            Canvas.SetTop(textBlock, firstPoint.Y - 200);

            canvas.Children.Add(textBlock);
        }

        #endregion Drawing
    }
}