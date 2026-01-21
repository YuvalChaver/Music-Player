using System.Windows;
using Microsoft.Win32;
using Telhai.DotNet.PlayerProject.ViewModels;

namespace Telhai.DotNet.PlayerProject
{
    public partial class EditTrackWindow : Window
    {
        private EditTrackViewModel _viewModel;

        public EditTrackWindow(MusicTrack track)
        {
            InitializeComponent();
            _viewModel = new EditTrackViewModel(track);
            DataContext = _viewModel;
            UpdateImagePreview();
        }

        private void BtnAddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
            
            if (ofd.ShowDialog() == true)
            {
                _viewModel.AddImage(ofd.FileName);
                UpdateImagePreview();
            }
        }

        private void BtnRemoveImage_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_viewModel.SelectedImage))
            {
                _viewModel.RemoveImage(_viewModel.SelectedImage);
                UpdateImagePreview();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Save();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void UpdateImagePreview()
        {
            if (!string.IsNullOrEmpty(_viewModel.SelectedImage) && System.IO.File.Exists(_viewModel.SelectedImage))
            {
                try
                {
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new System.Uri(_viewModel.SelectedImage);
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    imgPreview.Source = bitmap;
                }
                catch { }
            }
            else
            {
                imgPreview.Source = null;
            }
        }
    }
}
