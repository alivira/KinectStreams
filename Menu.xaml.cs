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

namespace KinectStreams
{
    public partial class Menu : Window
    {
        public OptimizationAngleMenu frmOpt = new OptimizationAngleMenu();
        public CaptureWindow cptWindow;
        public History hstWindow;
        string thetaA = "";
        string thetaB = "";
        string thetaC = "";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void btnInput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmOpt.Show();
            }
            catch
            {
                frmOpt = new OptimizationAngleMenu();
                frmOpt.Show();
            }
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            thetaA = frmOpt.handLength.ToString();
            thetaB = frmOpt.upperarmLength.ToString();
            thetaC = frmOpt.forearmLength.ToString();
            if (thetaA.Equals("0") || thetaB.Equals("0") || thetaC.Equals("0"))
            {
                MessageBox.Show("Please input the user angles.", "Error", MessageBoxButton.OK);
            }
            else
            {
                cptWindow = new CaptureWindow(thetaA, thetaB, thetaC);
                cptWindow.Show();
            }
            
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            hstWindow = new History();
            hstWindow.Show();
        }
    }
}
