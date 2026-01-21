using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using YuvalChaver.Telhai.DotNet.PlayerProject.ViewModels;

namespace YuvalChaver.Telhai.DotNet.PlayerProject
{
    /// <summary>
    /// Interaction logic for EditTrackWindow.xaml
    /// MVVM pattern - DataContext is EditTrackViewModel
    /// </summary>
    public partial class EditTrackWindow : Window
    {
        private MusicTrack track;
        private EditTrackViewModel viewModel;

        public EditTrackWindow(MusicTrack musicTrack)
        {
            InitializeComponent();
            track = musicTrack;
            viewModel = new EditTrackViewModel(track);
            DataContext = viewModel;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnAddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files (*.*)|*.*";
            dialog.Title = "Select Image for Track";

            if (dialog.ShowDialog() == true)
            {
                string imagePath = dialog.FileName;
                viewModel.AddImage(imagePath);
                txtImageStatus.Text = $"Added: {System.IO.Path.GetFileName(imagePath)}";
            }
        }

        private void BtnRemoveImage_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;
            if (btn?.DataContext is string imagePath)
            {
                viewModel.RemoveImage(imagePath);
                txtImageStatus.Text = "Image removed";
            }
        }

        private void BtnPreviewArtwork_Click(object sender, RoutedEventArgs e)
        {
            string url = TxtArtworkUrl.Text;
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not open URL: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No artwork URL specified");
            }
        }
    }
}
