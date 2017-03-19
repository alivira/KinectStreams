﻿using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace KinectStreams
{
    public static class Extensions
    {
        #region Camera

        public static ImageSource ToBitmap(this ColorFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            byte[] pixels = new byte[width * height * ((format.BitsPerPixel + 7) / 8)];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public static ImageSource ToBitmap(this DepthFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            ushort minDepth = frame.DepthMinReliableDistance;
            ushort maxDepth = frame.DepthMaxReliableDistance;

            ushort[] pixelData = new ushort[width * height];
            byte[] pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(pixelData);

            int colorIndex = 0;
            for (int depthIndex = 0; depthIndex < pixelData.Length; ++depthIndex)
            {
                ushort depth = pixelData[depthIndex];

                byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                pixels[colorIndex++] = intensity; // Blue
                pixels[colorIndex++] = intensity; // Green
                pixels[colorIndex++] = intensity; // Red

                ++colorIndex;
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        public static ImageSource ToBitmap(this InfraredFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            ushort[] frameData = new ushort[width * height];
            byte[] pixels = new byte[width * height * (format.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(frameData);

            int colorIndex = 0;
            for (int infraredIndex = 0; infraredIndex < frameData.Length; infraredIndex++)
            {
                ushort ir = frameData[infraredIndex];

                byte intensity = (byte)(ir >> 7);

                pixels[colorIndex++] = (byte)(intensity / 1); // Blue
                pixels[colorIndex++] = (byte)(intensity / 1); // Green   
                pixels[colorIndex++] = (byte)(intensity / 0.4); // Red

                colorIndex++;
            }

            int stride = width * format.BitsPerPixel / 8;

            return BitmapSource.Create(width, height, 96, 96, format, null, pixels, stride);
        }

        #endregion

        #region Body

        public static Joint ScaleTo(this Joint joint, double width, double height, float skeletonMaxX, float skeletonMaxY)
        {
            joint.Position = new CameraSpacePoint
            {
                X = Scale(width, skeletonMaxX, joint.Position.X),
                Y = Scale(height, skeletonMaxY, -joint.Position.Y),
                Z = joint.Position.Z
            };

            return joint;
        }

        public static Joint ScaleTo(this Joint joint, double width, double height)
        {
            return ScaleTo(joint, width, height, 1.0f, 1.0f);
        }

        private static float Scale(double maxPixel, double maxSkeleton, float position)
        {
            float value = (float)((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));

            if (value > maxPixel)
            {
                return (float)maxPixel;
            }

            if (value < 0)
            {
                return 0;
            }

            return value;
        }

        #endregion

        #region Drawing

        public static double DrawSkeleton(this Canvas canvas, Body body, bool leftArmChecked)
        {
            if (body == null) return 0;

            foreach (Joint joint in body.Joints.Values)
            {
                //ignore unnecessay joints
                if (joint.JointType.Equals(JointType.HipLeft)|| joint.JointType.Equals(JointType.HipRight) || joint.JointType.Equals(JointType.KneeLeft) 
                    || joint.JointType.Equals(JointType.KneeRight) || joint.JointType.Equals(JointType.AnkleLeft) || joint.JointType.Equals(JointType.AnkleRight)
                    || joint.JointType.Equals(JointType.FootLeft) || joint.JointType.Equals(JointType.FootRight) || joint.JointType.Equals(JointType.ThumbLeft)
                    || joint.JointType.Equals(JointType.ThumbRight) || joint.JointType.Equals(JointType.SpineBase))
                {
                    continue; 
                }
                else if(leftArmChecked){
                    if (joint.JointType.Equals(JointType.ElbowRight) || joint.JointType.Equals(JointType.HandRight) || joint.JointType.Equals(JointType.ShoulderRight) 
                        || joint.JointType.Equals(JointType.HandTipRight) || joint.JointType.Equals(JointType.WristRight))
                    {
                        continue;
                    }
                }
                else
                {
                    if (joint.JointType.Equals(JointType.ElbowLeft) || joint.JointType.Equals(JointType.HandLeft) || joint.JointType.Equals(JointType.ShoulderLeft)
                        || joint.JointType.Equals(JointType.HandTipLeft) || joint.JointType.Equals(JointType.WristLeft))
                    {
                        continue;
                    }
                }
                canvas.DrawPoint(joint);
            }

            canvas.DrawLine(body.Joints[JointType.Head], body.Joints[JointType.Neck]);
            canvas.DrawLine(body.Joints[JointType.Neck], body.Joints[JointType.SpineShoulder]);
            canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.SpineMid]);           

            Joint shoulder = new Joint();
            Joint elbow = new Joint();
            Joint wrist = new Joint();
            Joint hand = new Joint();

            if (leftArmChecked)
            {
                canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderLeft]);
                canvas.DrawLine(body.Joints[JointType.ShoulderLeft], body.Joints[JointType.ElbowLeft]);
                canvas.DrawLine(body.Joints[JointType.ElbowLeft], body.Joints[JointType.WristLeft]);
                canvas.DrawLine(body.Joints[JointType.WristLeft], body.Joints[JointType.HandLeft]);
                canvas.DrawLine(body.Joints[JointType.HandLeft], body.Joints[JointType.HandTipLeft]);

                shoulder = body.Joints[JointType.ShoulderLeft];
                elbow = body.Joints[JointType.ElbowLeft];
                wrist = body.Joints[JointType.WristLeft];
                hand = body.Joints[JointType.HandLeft];
 
            }
            else
            {
                canvas.DrawLine(body.Joints[JointType.SpineShoulder], body.Joints[JointType.ShoulderRight]);
                canvas.DrawLine(body.Joints[JointType.ShoulderRight], body.Joints[JointType.ElbowRight]);
                canvas.DrawLine(body.Joints[JointType.ElbowRight], body.Joints[JointType.WristRight]);
                canvas.DrawLine(body.Joints[JointType.WristRight], body.Joints[JointType.HandRight]);
                canvas.DrawLine(body.Joints[JointType.HandRight], body.Joints[JointType.HandTipRight]);

                shoulder = body.Joints[JointType.ShoulderRight];
                elbow = body.Joints[JointType.ElbowRight];
                wrist = body.Joints[JointType.WristRight];
                hand = body.Joints[JointType.HandRight];
            }

            float shoulderPoint_X = shoulder.Position.X;
            float shoulderPoint_Y = body.Joints[JointType.ShoulderLeft].Position.Y;
            float elbowPoint_X = body.Joints[JointType.ElbowLeft].Position.X;
            float elbowPoint_Y = body.Joints[JointType.ElbowLeft].Position.Y;


            float wristPoint_X = body.Joints[JointType.WristLeft].Position.X;
            float wristPoint_Y = body.Joints[JointType.WristLeft].Position.Y;

            double lengthShoulderToElbow = Math.Pow(shoulderPoint_X - elbowPoint_X, 2) + Math.Pow(shoulderPoint_Y - elbowPoint_Y, 2);
            double lengthElbowToWrist = Math.Pow(wristPoint_X - elbowPoint_X, 2) + Math.Pow(wristPoint_Y - elbowPoint_Y, 2);
            double lengthShoulderToWrist = Math.Pow(shoulderPoint_X - wristPoint_X, 2) + Math.Pow(shoulderPoint_Y - wristPoint_Y, 2);

            return Math.Round(((Math.Acos((lengthShoulderToElbow + lengthElbowToWrist - lengthShoulderToWrist) / (2 * Math.Sqrt(lengthShoulderToElbow) * Math.Sqrt(lengthElbowToWrist)))) * 180 / Math.PI),2);
            
            //canvas.DrawLine(body.Joints[JointType.HandTipLeft], body.Joints[JointType.ThumbLeft]);
            //canvas.DrawLine(body.Joints[JointType.HandTipRight], body.Joints[JointType.ThumbRight]);
            //canvas.DrawLine(body.Joints[JointType.SpineMid], body.Joints[JointType.SpineBase]);
            //canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipLeft]);
            //canvas.DrawLine(body.Joints[JointType.SpineBase], body.Joints[JointType.HipRight]);
            //canvas.DrawLine(body.Joints[JointType.HipLeft], body.Joints[JointType.KneeLeft]);
            //canvas.DrawLine(body.Joints[JointType.HipRight], body.Joints[JointType.KneeRight]);
            //canvas.DrawLine(body.Joints[JointType.KneeLeft], body.Joints[JointType.AnkleLeft]);
            //canvas.DrawLine(body.Joints[JointType.KneeRight], body.Joints[JointType.AnkleRight]);
            //canvas.DrawLine(body.Joints[JointType.AnkleLeft], body.Joints[JointType.FootLeft]);
            //canvas.DrawLine(body.Joints[JointType.AnkleRight], body.Joints[JointType.FootRight]);
        }

        public static void DrawPoint(this Canvas canvas, Joint joint)
        {
            if (joint.TrackingState == TrackingState.NotTracked) return;

            joint = joint.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            Ellipse ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Colors.LightBlue)
            };

            Canvas.SetLeft(ellipse, joint.Position.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, joint.Position.Y - ellipse.Height / 2);

            canvas.Children.Add(ellipse);
        }

        public static void DrawLine(this Canvas canvas, Joint first, Joint second)
        {
            if (first.TrackingState == TrackingState.NotTracked || second.TrackingState == TrackingState.NotTracked) return;

            first = first.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);
            second = second.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            Line line = new Line
            {
                X1 = first.Position.X,
                Y1 = first.Position.Y,
                X2 = second.Position.X,
                Y2 = second.Position.Y,
                StrokeThickness = 8,
                Stroke = new SolidColorBrush(Colors.LightBlue)
            };

            canvas.Children.Add(line);
        }

        #endregion
    }
}
