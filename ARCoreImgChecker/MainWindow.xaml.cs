using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace ARCoreImgChecker
{
    public partial class MainWindow : Window
    {
        private string pathToExe;

        public ObservableCollection<ImageItem> items;
        public MainWindow()
        {
            InitializeComponent();
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
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg;*.|All files (*.*)|*.*";
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
            }
        }

        private void CheckImageQuality(string[] imageurls)
        {
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
