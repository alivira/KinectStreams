﻿using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Net;
 using System.IO;
using BytescoutScreenCapturingLib;

namespace KinectStreams
{
    /// <summary>
    /// Interaction logic for CaptureWindow.xaml
    /// </summary>
    public partial class CaptureWindow : Window
    {
        #region Members

        Mode _mode = Mode.Color;

        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;

        bool _displayBody = false;

        #endregion

        #region Constructor

        public CaptureWindow()
        {
            InitializeComponent();
        }

        #endregion

        private static double idealThetaA = 44.43;
        private static double idealThetaB = 39.22;
        private static double idealThetaC = 56.46;

        private bool sendToServerElbow = false;
        private bool sendToServerWrist = false;
        private bool sendToServerShoulder = false;
        private bool sendToServerRest = false;

        private DispatcherTimer dt = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        private Capturer capturer;

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnLeftArm.Foreground = Brushes.White;
            btnRightArm.Foreground = Brushes.White;          
            _sensor = KinectSensor.GetDefault();

            capturer = new Capturer(); // create new screen capturer object
            //capturer.CapturingType = CaptureAreaType.catRegion; // set capturing area type to catScreen to capture whole screen
            capturer.CapturingType = CaptureAreaType.catScreen; // set capturing area type to catScreen to capture whole screen

            capturer.OutputFileName = "EntireScreenCaptured.wmv"; // set output video filename to .WVM or .AVI filename
            /*double testing = this.Width;
            double testing2 = this.ActualWidth;
            double border = (testing2 - testing) / 2;
            //double test = this.fo
            double test2 = this.Top;
            
            capturer.CaptureRectLeft = Convert.ToInt32(this.Left);
            capturer.CaptureRectTop = Convert.ToInt32(this.Top);
            */
            // set output video width and height
            capturer.OutputWidth = Convert.ToInt32(this.Width);
            capturer.OutputHeight = Convert.ToInt32(this.Height);

            // WMV and WEBM output use WMVVideoBitrate property to control output video bitrate
            // so try to increase it by x2 or x3 times if you think the output video are you are getting is laggy
            capturer.WMVVideoBitrate = capturer.WMVVideoBitrate * 2;

            // uncomment to set Bytescout Lossless Video format output video compression method
            //do not forget to set file to .avi format if you use Video Codec Name
            //capturer.CurrentVideoCodecName = "Bytescout Lossless";             


            // uncomment to enable recording of semitransparent or layered windows (Warning: may cause mouse cursor flickering)
            // capturer.CaptureTransparentControls = true;


            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }


        }

        private int increment = 0;
        private int posture = 0;

        private void dtTicker(object sender, EventArgs e)
        {
            increment++;
            increment = increment + posture;
            RestTimer.Text = increment.ToString();

            if(increment >= 60)
            {
                sendToServerRest = true;
            }

             var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://rocky-citadel-35459.herokuapp.com/sendData");
            httpWebRequest.Proxy = null;
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"shoulder\":" + sendToServerShoulder.ToString().ToLower() + "," +
               "\"elbow\":" + sendToServerElbow.ToString().ToLower() + "," +
               "\"wrist\":" + sendToServerWrist.ToString().ToLower() + "," +
               "\"end\":" + sendToServerRest.ToString().ToLower() + "}";
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

        }


        private void Window_Closed(object sender, EventArgs e)
        {
            if (_reader != null)
            {
                _reader.Dispose();
            }

            if (_sensor != null)
            {
                _sensor.Close();
            }

            // Release resources
            System.Runtime.InteropServices.Marshal.ReleaseComObject(capturer);
            capturer = null;
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == Mode.Color)
                    {
                        camera.Source = frame.ToBitmap();
                    }
                }
            }

            // Depth
            /*using (var frame = reference.DepthFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == Mode.Depth)
                    {
                        camera.Source = frame.ToBitmap();
                    }
                }
            }

            // Infrared
            using (var frame = reference.InfraredFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == Mode.Infrared)
                    {
                        camera.Source = frame.ToBitmap();
                    }
                }
            }*/

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    /*var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://rocky-citadel-35459.herokuapp.com/sendData");
                    httpWebRequest.Proxy = null;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = "{\"data\":" + sendToServer + "}";

                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }*/

                    canvas.Children.Clear();

                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                // Draw skeleton.
                                if (_displayBody)
                                {
                                    double[] theta = { };

                                    if (btnLeftArm.IsChecked == true)
                                    {
                                        canvas.DrawIdeal(true, 0.7754, 0.6845, 0.9854, Global.upperarmLength, Global.forearmLength, Global.handLength, body);
                                        theta = canvas.DrawSkeleton(body, true);

                                        if(theta != null)
                                        {
                                            txtForearmTheta.Text = "Forearm: " + theta[0];
                                            txtElbowTheta.Text = "Elbow: " + theta[1];
                                            txtWristTheta.Text = "Wrist: " + theta[2];                                        
                                        }
                                        //canvas.DrawIdeal(true, 0.7754, 0.6845, 0.9854, 0.32, 0.25, 0.18, body);
                                    }
                                    else
                                    {
                                        canvas.DrawIdeal(false, 0.7754, 0.6845, 0.9854, Global.upperarmLength, Global.forearmLength, Global.handLength, body);
                                        theta = canvas.DrawSkeleton(body, false);

                                        if (theta != null)
                                        {
                                            txtForearmTheta.Text = "Forearm: " + theta[0];
                                            txtElbowTheta.Text = "Elbow: " + theta[1];
                                            txtWristTheta.Text = "Wrist: " + theta[2];
                                        }
                                    }



                                    //Evaluate angle of the wrist
                                    if ((theta[0] >= idealThetaA - 5) && (theta[0] <= idealThetaA + 5))
                                    {
                                        posture = 0;
                                        sendToServerWrist = false;
                                    }
                                    else
                                    {
                                        posture = 1;
                                        sendToServerWrist = true;
                                    }

                                    //Evaluate angle of the elbow
                                    if ((theta[1] >= idealThetaB - 5) && (theta[1] <= idealThetaB + 5))
                                    {
                                        posture = 0;
                                        sendToServerElbow = false;
                                    }
                                    else
                                    {
                                        posture = 1;
                                        sendToServerElbow = true;
                                    }

                                    //Evaluate angle of the shoulder
                                    if ((theta[2] >= idealThetaC - 5) && (theta[2] <= idealThetaC + 5))
                                    {
                                        posture = 0;
                                        sendToServerShoulder = false;
                                    }
                                    else
                                    {
                                        posture = 1;
                                        sendToServerShoulder = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Color_Click(object sender, RoutedEventArgs e)
        {
            _mode = Mode.Color;
        }

        private void Depth_Click(object sender, RoutedEventArgs e)
        {
            _mode = Mode.Depth;
        }

        private void Infrared_Click(object sender, RoutedEventArgs e)
        {
            _mode = Mode.Infrared;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (btnLeftArm.IsChecked == false && btnRightArm.IsChecked == false)
            {
                MessageBox.Show("Please select an arm to capture", "Invalid arm capture", MessageBoxButton.OK);
            }
            else
            {
                dt.Tick += dtTicker;
                dt.Start();
                capturer.Run(); // run screen video capturing 
                _displayBody = true;
            }        
        }

        #endregion

        private void btnLeftArm_Checked(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = true;
        }

        private void btnRightArm_Checked(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _displayBody = false;
            capturer.Stop();
            dt.Stop();
        }
    }

    public enum Mode
    {
        Color,
        Depth,
        Infrared
    }
}
