using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;

namespace KinectStreams
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Window
    {
        public History()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] files = Directory.GetFiles(Environment.CurrentDirectory);
            foreach(string file in files)
            {
                if (file.Contains("EntireScreenCapture"))
                {
                    listVideo.Items.Add(file);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
