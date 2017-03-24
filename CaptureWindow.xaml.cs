using Microsoft.Kinect;
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

        private static double idealTheta = 39.22;

        private int sendToServer = 0;

        private DispatcherTimer dt = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnLeftArm.Foreground = Brushes.White;
            btnRightArm.Foreground = Brushes.White;          
            _sensor = KinectSensor.GetDefault();
            



            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }


        }

        private int increment = 0;

        private void dtTicker(object sender, EventArgs e)
        {
            increment++;
            RestTimer.Text = increment.ToString();

            if(increment >= 60)
            {
                sendToServer = 2;
            }

             var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://rocky-citadel-35459.herokuapp.com/sendData");
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
                                        theta = canvas.DrawSkeleton(body, false);
                                        if(theta != null)
                                        {
                                            txtForearmTheta.Text = "Forearm: " + theta[0];
                                            txtElbowTheta.Text = "Elbow: " + theta[1];
                                            txtWristTheta.Text = "Wrist: " + theta[2];
                                        }
                                        //canvas.DrawIdeal(false, 0.7754, 0.6845, 0.9854, 0.32, 0.25, 0.18, body);
                                    }

                                    if ((theta[1] >= idealTheta - 5) && (theta[1] <= idealTheta + 5))
                                    {
                                        sendToServer = 0;
                                    }
                                    else
                                    {
                                        sendToServer = 1;
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
            dt.Tick += dtTicker;
            dt.Start();
            if (btnLeftArm.IsChecked == false && btnRightArm.IsChecked == false)
            {
                MessageBox.Show("Please select an arm to capture", "Invalid arm capture", MessageBoxButton.OK);
            }
            else
            {
                _displayBody = !_displayBody;
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
    }

    public enum Mode
    {
        Color,
        Depth,
        Infrared
    }
}
