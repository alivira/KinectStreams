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
using System.Windows.Forms;

namespace KinectStreams
{
    public partial class OptimizationAngleMenu : Window
    {

        public int handLength = 0;
        public int forearmLength = 0;
        public int upperarmLength = 0;


        public OptimizationAngleMenu()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void btnInput_Click(object sender, RoutedEventArgs e)
        {
            //OptimizationAngleMenu frmOpt = new OptimizationAngleMenu();
            //frmOpt.Show();
        }

        private void btnSaveUser_Click(object sender, RoutedEventArgs e)
        {
            string tempHandLength = txtHandLength.Text.Trim();
            string tempforearmLength = txtForeArmLength.Text.Trim();
            string tempUpperarmLength = txtUpperLength.Text.Trim();
            int.TryParse(tempHandLength, out handLength);
            int.TryParse(tempforearmLength, out forearmLength);
            int.TryParse(tempUpperarmLength, out upperarmLength);

            bool invalidMeasurement = false;

            if(handLength == 0)
            {
                txtHandLength.BorderBrush = System.Windows.Media.Brushes.Red;
                txtHandLength.BorderThickness = new Thickness(2.0);
                invalidMeasurement = true;
            }
            if(forearmLength == 0)
            {
                txtForeArmLength.BorderBrush = System.Windows.Media.Brushes.Red;
                txtForeArmLength.BorderThickness = new Thickness(2.0);
                invalidMeasurement = true;
            }
            if(upperarmLength == 0)
            {
                txtUpperLength.BorderBrush = System.Windows.Media.Brushes.Red;
                txtUpperLength.BorderThickness = new Thickness(2.0);
                invalidMeasurement = true;
            }
            if (invalidMeasurement)
            {
                System.Windows.Forms.MessageBox.Show("Please enter a valid measurement", "Invalid measurement", MessageBoxButtons.OK);
            }
            else
            {
                this.Hide();
            }
        }

        private void txtHandLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtHandLength.BorderBrush = System.Windows.Media.Brushes.Black;
            txtHandLength.BorderThickness = new Thickness(1.0);
        }

        private void txtForeArmLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtForeArmLength.BorderBrush = System.Windows.Media.Brushes.Black;
            txtForeArmLength.BorderThickness = new Thickness(1.0);
        }

        private void txtUpperLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtUpperLength.BorderBrush = System.Windows.Media.Brushes.Black;
            txtUpperLength.BorderThickness = new Thickness(1.0);
        }
    }
}
