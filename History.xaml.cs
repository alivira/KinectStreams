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
            for(int i = listVideo.Items.Count - 1; i >= 0; i--)
            {
                listVideo.Items.RemoveAt(i);
            }
            string[] files = Directory.GetFiles(Environment.CurrentDirectory);
            foreach(string file in files)
            {
                if (file.Contains("EntireScreenCapture"))
                {
                    
                    string temp = file;
                    temp = temp.Remove(0, file.IndexOf("EntireScreenCapture"));
                    listVideo.Items.Add(temp);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if(listVideo.SelectedItems.Count == 0)
            {
                System.Windows.MessageBox.Show("Please select a video to play.", "Invalid video file", MessageBoxButton.OK);
            }
            string video = listVideo.SelectedItem.ToString();
            video = Environment.CurrentDirectory + "//" + video;
            txtTip.Text = "Play for longer";
            mediaplayer.Source = new Uri(video);
            mediaplayer.UnloadedBehavior = MediaState.Manual;
            mediaplayer.Play();
        }
    }
}
