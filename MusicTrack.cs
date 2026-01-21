using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telhai.DotNet.PlayerProject.Models;

namespace Telhai.DotNet.PlayerProject
{
    public class MusicTrack
    {
        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;

        // iTunes API metadata
        public string? ArtistName { get; set; }
        public string? AlbumName { get; set; }
        public string? AlbumArtworkUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? TrackViewUrl { get; set; }
        
        // Custom images for this track
        public List<string> CustomImages { get; set; } = new List<string>();

        // Default artwork path for fallback
        private static string? _defaultArtworkPath;

        public static string DefaultArtworkPath
        {
            get
            {
                if (_defaultArtworkPath == null)
                {
                    _defaultArtworkPath = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Resources",
                        "DefaultAlbumArt.png");
                }
                return _defaultArtworkPath;
            }
        }

        /// <summary>
        /// Gets the display artwork URL - uses album artwork if available, otherwise returns default
        /// </summary>
        public string GetArtworkUrl()
        {
            if (!string.IsNullOrEmpty(AlbumArtworkUrl))
            {
                return AlbumArtworkUrl;
            }
            return DefaultArtworkPath;
        }

        // This makes sure the ListBox shows the Name, not "MyMusicPlayer.MusicTrack"
        public override string ToString()
        {
            return Title;
        }
    }

}
