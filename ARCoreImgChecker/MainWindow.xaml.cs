using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ARCoreImgChecker
{
    public partial class MainWindow : Window
    {
        private string pathToExe;

        public ObservableCollection<ImageItem> items;
        public MainWindow()
        {
            InitializeComponent();
            //items = new ObservableCollection<ImageItem>
            //{
            //    new ImageItem{Id = 0, ImagePath = "/Frame 17.jpg",ImageQuality = "15"},
            //    new ImageItem{Id = 1, ImagePath = "/Frame 17.jpg",ImageQuality = "15"},
            //    new ImageItem{Id = 2, ImagePath = "/Frame 17.jpg",ImageQuality = "15"},
            //    new ImageItem{Id = 3, ImagePath = "/Frame 17.jpg",ImageQuality = "15"}
            //};
            //ImageListBox.ItemsSource = items;
        }
        private void SelectPathToExeClick(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                pathToExe = openFileDialog.FileName;
                SelectPathToExe.Content = pathToExe;
            }
        }
        private void SelectImagePathClick(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                if (SelectPathToExe.Content.ToString() == "Выберите путь к файлу arcoreimg.exe")
                    return;
                SelectImagePathButton.Content = openFileDialog.FileName;
                if (openFileDialog.FileNames.Length > 0)
                {
                    CheckImageQuality(openFileDialog.FileNames);
                }
                //else if(openFileDialog.FileNames.Length == 1)
                //{
                //    CheckImageQuality();
                //}
            }
        }

        private void CheckImageQuality()
        {
            string cmd = SelectPathToExe.Content + " eval-img --input_image_path=" + 
                SelectImagePathButton.Content.ToString().Replace(@"/", @"\").Replace("JPG", "jpg");
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "powershell.exe";
                p.StartInfo.Arguments = cmd;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                QualityText.Text = "Качество изображения : " + p.StandardOutput.ReadToEnd();
                ImageTargetSource.Source = new ImageSourceConverter().ConvertFromString(SelectImagePathButton.Content.ToString()) as ImageSource;
                p.WaitForExit();
                p.Close();
            }
        }
        private void CheckImageQuality(string[] imageurls)
        {
            ImageListBox.Items.Clear();
            List<ImageItem> imageItems = new List<ImageItem>();
            int inc = 0;
            foreach (string imageurl in imageurls)
            {
               string cmd =  SelectPathToExe.Content + " eval-img --input_image_path=" +
                imageurl.ToString().Replace(@"/", @"\").Replace("JPG", "jpg");
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "powershell.exe";
                    p.StartInfo.Arguments = cmd;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.UseShellExecute = false;
                    p.Start();
                    imageItems.Add(new ImageItem()
                    {
                        Id = inc,
                        ImagePath = imageurl,
                        ImageQuality = "Качество изображения " + p.StandardOutput.ReadToEnd()
                    });
                    p.WaitForExit();
                    p.Close();
                }
                inc++;
            }
            ImageListBox.ItemsSource = imageItems;
        }
    }
    public class ImageItem
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string ImageQuality { get; set; }
    }
}
