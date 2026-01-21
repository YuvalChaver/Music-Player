using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using YuvalChaver.Telhai.DotNet.PlayerProject.Models;

namespace YuvalChaver.Telhai.DotNet.PlayerProject.Services
{
    /// <summary>
    /// Service for interacting with iTunes Search API
    /// Handles async HTTP requests with cancellation token support
    /// </summary>
    public class ITunesSearchService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://itunes.apple.com/search";
        private const int TIMEOUT_SECONDS = 10;

        public ITunesSearchService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
        }

        /// <summary>
        /// Searches iTunes API for track information based on search query
        /// </summary>
        /// <param name="searchQuery">Query string (artist, track name, etc.)</param>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>First matching track or null if not found</returns>
        public async Task<ITunesTrack?> SearchTrackAsync(string searchQuery, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchQuery))
                    return null;

                // Encode the search query for URL
                string encodedQuery = Uri.EscapeDataString(searchQuery.Trim());
                string url = $"{BASE_URL}?term={encodedQuery}&entity=song&limit=1";

                // Make async HTTP GET request with cancellation token
                using (var response = await _httpClient.GetAsync(url, cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"iTunes API error: {response.StatusCode}");
                        return null;
                    }

                    string content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var searchResult = JsonSerializer.Deserialize<ITunesSearchResponse>(content);

                    // Return first result if available
                    return searchResult?.Results.Count > 0 ? searchResult.Results[0] : null;
                }
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("iTunes search cancelled");
                return null;
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error in iTunes search: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Extracts search query from filename
        /// Supports formats: "Artist - Track", "Artist_Track", or just "Track Name"
        /// </summary>
        /// <param name="filename">Filename without extension</param>
        /// <returns>Extracted search query</returns>
        public string ExtractSearchQuery(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return string.Empty;

            // Replace common separators with space
            string query = Regex.Replace(filename, @"[-_\.]+", " ");

            // Remove extra whitespace
            query = Regex.Replace(query, @"\s+", " ").Trim();

            return query;
        }
    }
}
