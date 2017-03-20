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
        public static double idealTheta = 39.22;

        public DispatcherTimer dt = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        public int sendToServer = 0;

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

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnLeftArm.Foreground = Brushes.White;
            btnRightArm.Foreground = Brushes.White;          
            _sensor = KinectSensor.GetDefault();
            btnStop.IsEnabled = false;
            


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
            if(increment >= 15)
            {
                sendToServer = 2;
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
            using (var frame = reference.DepthFrameReference.AcquireFrame())
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
            }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {                    
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://rocky-citadel-35459.herokuapp.com/sendData");
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
                                    double theta = 0;
                                    if (btnLeftArm.IsChecked == true)
                                    {
                                        double[] jointPositions = canvas.getPoints(body, true);
                                        txtShoulderX.Text = jointPositions[0].ToString();
                                        txtShoulderY.Text = jointPositions[1].ToString();
                                        txtElbowX.Text = jointPositions[2].ToString();
                                        txtElbowY.Text = jointPositions[3].ToString();
                                        theta = canvas.DrawSkeleton(body, true);
                                        txtUpperToForearmTheta.Text = "Theta: " + theta;
                                    }
                                    else
                                    {
                                        double[] jointPositions = canvas.getPoints(body, false);
                                        txtShoulderX.Text = jointPositions[0].ToString();
                                        txtShoulderY.Text = jointPositions[1].ToString();
                                        txtElbowX.Text = jointPositions[2].ToString();
                                        txtElbowY.Text = jointPositions[3].ToString();
                                        theta = canvas.DrawSkeleton(body, true);
                                        txtUpperToForearmTheta.Text = "Theta: " + theta;
                                    }
                                    if((theta >= idealTheta - 5) && (theta <= idealTheta + 5))
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

        private void Body_Click(object sender, RoutedEventArgs e)
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
                btnMenu.IsEnabled = false;
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

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            dt.Stop();
            _displayBody = !_displayBody;
            btnMenu.IsEnabled = true;
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }

    public enum Mode
    {
        Color,
        Depth,
        Infrared
    }
}
