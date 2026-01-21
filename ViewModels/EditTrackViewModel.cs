using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Telhai.DotNet.PlayerProject.ViewModels
{
    public class EditTrackViewModel : INotifyPropertyChanged
    {
        private MusicTrack _track;
        private string _trackTitle;
        private List<string> _images;
        private string _selectedImage;

        public event PropertyChangedEventHandler? PropertyChanged;

        public EditTrackViewModel(MusicTrack track)
        {
            _track = track;
            _trackTitle = track.Title;
            _images = new List<string>(track.CustomImages);
            _selectedImage = _images.Count > 0 ? _images[0] : string.Empty;
        }

        public string TrackTitle
        {
            get => _trackTitle;
            set { if (_trackTitle != value) { _trackTitle = value; OnPropertyChanged(); } }
        }

        public string ArtistName => _track.ArtistName ?? "N/A";
        public string AlbumName => _track.AlbumName ?? "N/A";
        public string FilePath => _track.FilePath;

        public List<string> Images
        {
            get => _images;
            set { if (_images != value) { _images = value; OnPropertyChanged(); } }
        }

        public string SelectedImage
        {
            get => _selectedImage;
            set { if (_selectedImage != value) { _selectedImage = value; OnPropertyChanged(); } }
        }

        public void AddImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && !Images.Contains(imagePath))
            {
                Images.Add(imagePath);
                OnPropertyChanged(nameof(Images));
                SelectedImage = imagePath;
            }
        }

        public void RemoveImage(string imagePath)
        {
            if (Images.Contains(imagePath))
            {
                Images.Remove(imagePath);
                OnPropertyChanged(nameof(Images));
                SelectedImage = Images.Count > 0 ? Images[0] : string.Empty;
            }
        }

        public void Save()
        {
            _track.Title = TrackTitle;
            _track.CustomImages = new List<string>(Images);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
