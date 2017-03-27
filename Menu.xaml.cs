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
        string thetaA = "44.4";
        string thetaB = "39.2";
        string thetaC = "56.5";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            frmOpt.txtHandLength.Text = thetaA;
            frmOpt.txtUpperLength.Text = thetaB;
            frmOpt.txtForeArmLength.Text = thetaC;
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
                frmOpt.txtHandLength.Text = thetaA;
                frmOpt.txtUpperLength.Text = thetaB;
                frmOpt.txtForeArmLength.Text = thetaC;
                frmOpt.Show();
            }
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            thetaA = frmOpt.handLength.ToString();
            thetaB = frmOpt.upperarmLength.ToString();
            thetaC = frmOpt.forearmLength.ToString();
            if (thetaA.Equals("0")) {
                thetaA = "44.4";
            }
            if (thetaB.Equals("0"))
            {
                thetaB = "39.2";
            }
            if(thetaC.Equals("0"))
            {
                thetaC = "56.5";
            }

            cptWindow = new CaptureWindow(thetaA, thetaB, thetaC);
            cptWindow.Show();
            
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            hstWindow = new History();
            hstWindow.Show();
        }
    }
}
