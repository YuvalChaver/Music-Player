using System.Text.Json.Serialization;

namespace YuvalChaver.Telhai.DotNet.PlayerProject.Models
{
    /// <summary>
    /// Represents the response from iTunes Search API
    /// </summary>
    public class ITunesSearchResponse
    {
        [JsonPropertyName("results")]
        public List<ITunesTrack> Results { get; set; } = new List<ITunesTrack>();

        [JsonPropertyName("resultCount")]
        public int ResultCount { get; set; }
    }

    /// <summary>
    /// Represents a single track from iTunes API
    /// </summary>
    public class ITunesTrack
    {
        [JsonPropertyName("trackName")]
        public string TrackName { get; set; } = string.Empty;

        [JsonPropertyName("artistName")]
        public string ArtistName { get; set; } = string.Empty;

        [JsonPropertyName("collectionName")]
        public string CollectionName { get; set; } = string.Empty;

        [JsonPropertyName("artworkUrl100")]
        public string ArtworkUrl { get; set; } = string.Empty;

        [JsonPropertyName("trackViewUrl")]
        public string TrackViewUrl { get; set; } = string.Empty;

        [JsonPropertyName("releaseDate")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonPropertyName("trackId")]
        public long TrackId { get; set; }
    }
}
