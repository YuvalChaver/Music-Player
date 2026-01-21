using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YuvalChaver.Telhai.DotNet.PlayerProject.ViewModels
{
    /// <summary>
    /// MVVM ViewModel for editing track information
    /// Manages track metadata and custom images
    /// </summary>
    public class EditTrackViewModel : INotifyPropertyChanged
    {
        private MusicTrack _track;
        private string _title = string.Empty;
        private string _artistName = string.Empty;
        private string _albumName = string.Empty;
        private string _albumArtworkUrl = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ArtistName
        {
            get => _artistName;
            set
            {
                if (_artistName != value)
                {
                    _artistName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AlbumName
        {
            get => _albumName;
            set
            {
                if (_albumName != value)
                {
                    _albumName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AlbumArtworkUrl
        {
            get => _albumArtworkUrl;
            set
            {
                if (_albumArtworkUrl != value)
                {
                    _albumArtworkUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> CustomImages { get; set; } = new ObservableCollection<string>();

        public EditTrackViewModel(MusicTrack track)
        {
            _track = track;
            Title = track.Title;
            ArtistName = track.ArtistName ?? string.Empty;
            AlbumName = track.AlbumName ?? string.Empty;
            AlbumArtworkUrl = track.AlbumArtworkUrl ?? string.Empty;

            // Populate custom images
            foreach (var image in track.CustomImages)
            {
                CustomImages.Add(image);
            }
        }

        /// <summary>
        /// Saves changes back to the original track object
        /// </summary>
        public void SaveChanges()
        {
            _track.Title = Title;
            _track.ArtistName = ArtistName;
            _track.AlbumName = AlbumName;
            _track.AlbumArtworkUrl = AlbumArtworkUrl;
            
            // Update custom images
            _track.CustomImages.Clear();
            foreach (var image in CustomImages)
            {
                _track.CustomImages.Add(image);
            }
        }

        /// <summary>
        /// Adds an image path to custom images
        /// </summary>
        public void AddImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && !CustomImages.Contains(imagePath))
            {
                CustomImages.Add(imagePath);
            }
        }

        /// <summary>
        /// Removes an image from custom images
        /// </summary>
        public void RemoveImage(string imagePath)
        {
            CustomImages.Remove(imagePath);
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
