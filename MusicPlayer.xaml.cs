using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using Telhai.DotNet.PlayerProject.Services;
using Telhai.DotNet.PlayerProject.Models;

namespace Telhai.DotNet.PlayerProject
{
    /// <summary>
    /// Interaction logic for MusicPlayer.xaml
    /// </summary>
    public partial class MusicPlayer : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer imageRotationTimer = new DispatcherTimer();
        private List<MusicTrack> library = new List<MusicTrack>();
        private bool isDragging = false;
        private const string FILE_NAME = "library.json";
        
        // iTunes Service and cancellation token for API calls
        private ITunesSearchService iTunesService = new ITunesSearchService();
        private CancellationTokenSource? currentSearchCancellation;
        
        // Currently playing track
        private MusicTrack? currentTrack;
        private int currentImageIndex = 0;

        public MusicPlayer()
        {
            //--init all Hardcoded xaml into Elements Tree
            InitializeComponent();

            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += new EventHandler(Timer_Tick);

            imageRotationTimer.Interval = TimeSpan.FromSeconds(3);
            imageRotationTimer.Tick += ImageRotationTimer_Tick;

            this.Loaded += MusicPlayer_Loaded;
        }

        private void MusicPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadLibrary();
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            // Update slider ONLY if music is loaded AND user is NOT holding the handle
            if (mediaPlayer.Source != null && mediaPlayer.NaturalDuration.HasTimeSpan && !isDragging)
            {
                sliderProgress.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliderProgress.Value = mediaPlayer.Position.TotalSeconds;
            }
        }

        // --- EMPTY PLACEHOLDERS TO MAKE IT BUILD ---
        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            // If a track is selected, play it
            if (lstLibrary.SelectedItem is MusicTrack track && currentTrack?.FilePath != track.FilePath)
            {
                PlayTrack(track);
            }
            else
            {
                // Resume playback if already loaded
                mediaPlayer.Play();
                timer.Start();
                txtStatus.Text = "Playing";
            }
        }
        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            txtStatus.Text = "Paused";
        }
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            timer.Stop();
            sliderProgress.Value = 0;
            txtStatus.Text = "Stopped";
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = sliderVolume.Value;
        }

        private void Slider_DragStarted(object sender, MouseButtonEventArgs e)
        {
            isDragging = true; // Stop timer updates
        }

        private void Slider_DragCompleted(object sender, MouseButtonEventArgs e)
        {
            isDragging = false; // Resume timer updates
            mediaPlayer.Position = TimeSpan.FromSeconds(sliderProgress.Value);
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //File Dialog to choose file from system
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "MP3 Files|*.mp3";

            //User Confirmed
            if (ofd.ShowDialog() == true)
            {
                //iterate all files selected as tring
                foreach (string file in ofd.FileNames)
                {
                    //Create Object for each filr
                    MusicTrack track = new MusicTrack
                    {
                        //Only file name
                        Title = System.IO.Path.GetFileNameWithoutExtension(file),
                        //full path
                        FilePath = file
                    };
                    library.Add(track);
                }
                UpdateLibraryUI();
                SaveLibrary();
            }
        }

        private void UpdateLibraryUI()
        {
            //Take All library list as Source to the listbox
            //diaplay tostring for inner object whithin list
            lstLibrary.ItemsSource = null;
            lstLibrary.ItemsSource = library;
        }

        private void SaveLibrary()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(library, options);
            File.WriteAllText(FILE_NAME, json);
        }

        private void LoadLibrary()
        {
            if (File.Exists(FILE_NAME))
            {
                //read File
                string json = File.ReadAllText(FILE_NAME);
                //Create List Of MusicTrack from json
                library = JsonSerializer.Deserialize<List<MusicTrack>>(json) ?? new List<MusicTrack>();
                //Show All loaded MusicTrack in List Box
                UpdateLibraryUI();
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lstLibrary.SelectedItem is MusicTrack track)
            {
                library.Remove(track);
                UpdateLibraryUI();
                SaveLibrary();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lstLibrary.SelectedItem is MusicTrack track)
            {
                EditTrackWindow editWindow = new EditTrackWindow(track);
                if (editWindow.ShowDialog() == true)
                {
                    SaveLibrary();
                    UpdateTrackDetailsUI(track);
                }
            }
        }

        private void ImageRotationTimer_Tick(object? sender, EventArgs e)
        {
            if (currentTrack?.CustomImages.Count > 0)
            {
                currentImageIndex = (currentImageIndex + 1) % currentTrack.CustomImages.Count;
                UpdateAlbumArtFromCustomImages();
            }
        }

        private void UpdateAlbumArtFromCustomImages()
        {
            if (currentTrack?.CustomImages.Count > 0)
            {
                try
                {
                    string imagePath = currentTrack.CustomImages[currentImageIndex];
                    if (File.Exists(imagePath))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(imagePath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        imgAlbumArt.Source = bitmap;
                        return;
                    }
                }
                catch { }
            }
        }

        private void LstLibrary_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstLibrary.SelectedItem is MusicTrack track)
            {
                PlayTrack(track);
            }
        }

        /// <summary>
        /// Handles single click on library item - shows track and file path info
        /// </summary>
        private void LstLibrary_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (lstLibrary.SelectedItem is MusicTrack track)
            {
                // Display track info without playing
                txtCurrentSong.Text = track.Title;
                txtFilePath.Text = $"Path: {track.FilePath}";
                
                // Display metadata if available
                if (!string.IsNullOrEmpty(track.ArtistName))
                    txtArtist.Text = track.ArtistName;
                
                if (!string.IsNullOrEmpty(track.AlbumName))
                    txtAlbum.Text = track.AlbumName;
            }
        }

        /// <summary>
        /// Plays a track and initiates asynchronous iTunes metadata search
        /// </summary>
        private async void PlayTrack(MusicTrack track)
        {
            // Store current track reference
            currentTrack = track;
            currentImageIndex = 0;
            
            // Cancel any previous iTunes search
            currentSearchCancellation?.Cancel();
            currentSearchCancellation = new CancellationTokenSource();

            // Play the local file
            mediaPlayer.Open(new Uri(track.FilePath));
            mediaPlayer.Play();
            timer.Start();
            txtCurrentSong.Text = track.Title;
            txtStatus.Text = "Playing";

            // Start image rotation if custom images exist
            if (track.CustomImages.Count > 0)
            {
                imageRotationTimer.Start();
            }
            else
            {
                imageRotationTimer.Stop();
            }

            // Search iTunes metadata asynchronously without blocking UI
            await SearchAndUpdateTrackMetadataAsync(track, currentSearchCancellation.Token);
        }

        /// <summary>
        /// Searches iTunes API for track metadata and updates UI
        /// Runs asynchronously without blocking the UI or audio playback
        /// </summary>
        private async Task SearchAndUpdateTrackMetadataAsync(MusicTrack track, CancellationToken cancellationToken)
        {
            try
            {
                // Check if metadata already cached
                if (!string.IsNullOrEmpty(track.ArtistName) || track.CustomImages.Count > 0)
                {
                    // Use cached data
                    UpdateTrackDetailsUI(track);
                    return;
                }

                // Extract search query from filename
                string searchQuery = iTunesService.ExtractSearchQuery(track.Title);
                
                // Search iTunes API asynchronously
                ITunesTrack? iTunesTrack = await iTunesService.SearchTrackAsync(searchQuery, cancellationToken);

                // Check if search was cancelled or if we're no longer playing this track
                if (cancellationToken.IsCancellationRequested || currentTrack != track)
                    return;

                if (iTunesTrack != null)
                {
                    // Update track metadata
                    track.ArtistName = iTunesTrack.ArtistName;
                    track.AlbumName = iTunesTrack.CollectionName;
                    track.AlbumArtworkUrl = iTunesTrack.ArtworkUrl;
                    track.TrackViewUrl = iTunesTrack.TrackViewUrl;

                    if (DateTime.TryParse(iTunesTrack.ReleaseDate, out DateTime releaseDate))
                    {
                        track.ReleaseDate = releaseDate;
                    }

                    // Save to cache
                    SaveLibrary();

                    // Update UI with new metadata
                    UpdateTrackDetailsUI(track);
                }
                else
                {
                    // No results found - display filename without extension
                    UpdateTrackDetailsUIError(track);
                }
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("iTunes search was cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching iTunes: {ex.Message}");
                UpdateTrackDetailsUIError(track);
            }
        }

        /// <summary>
        /// Updates the UI with track metadata from iTunes
        /// </summary>
        private void UpdateTrackDetailsUI(MusicTrack track)
        {
            // Update text information
            if (!string.IsNullOrEmpty(track.ArtistName))
            {
                txtArtist.Text = track.ArtistName;
            }

            if (!string.IsNullOrEmpty(track.AlbumName))
            {
                txtAlbum.Text = track.AlbumName;
            }

            // Display file path
            txtFilePath.Text = $"Path: {track.FilePath}";

            // Display custom images or album artwork
            if (track.CustomImages.Count > 0)
            {
                UpdateAlbumArtFromCustomImages();
            }
            else if (!string.IsNullOrEmpty(track.AlbumArtworkUrl))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(track.AlbumArtworkUrl);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    imgAlbumArt.Source = bitmap;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading album art: {ex.Message}");
                    imgAlbumArt.Source = LoadDefaultAlbumArt();
                }
            }
            else
            {
                imgAlbumArt.Source = LoadDefaultAlbumArt();
            }
        }

        /// <summary>
        /// Updates UI when no iTunes data is found
        /// </summary>
        private void UpdateTrackDetailsUIError(MusicTrack track)
        {
            txtArtist.Text = "Artist not found";
            txtAlbum.Text = "Album not found";
            txtFilePath.Text = $"Path: {track.FilePath}";
            imgAlbumArt.Source = LoadDefaultAlbumArt();
        }

        /// <summary>
        /// Loads the default album artwork image
        /// </summary>
        private BitmapImage? LoadDefaultAlbumArt()
        {
            try
            {
                string defaultArtPath = MusicTrack.DefaultArtworkPath;
                if (File.Exists(defaultArtPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(defaultArtPath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading default album art: {ex.Message}");
            }
            return null;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            //1) Create Settings Window Instance
            Settings settingsWin = new Settings();

            //2) Subscribe/register to OnScanCompleted Event
            settingsWin.OnScanCompleted += SettingsWin_OnScanCompleted;

            settingsWin.ShowDialog();

        }

        private void SettingsWin_OnScanCompleted(List<MusicTrack> newTracksEventData)
        {
            foreach (var track in newTracksEventData)
            {
                // Prevent duplicates based on FilePath
                if (!library.Any(x => x.FilePath == track.FilePath))
                {
                    library.Add(track);
                }
            }

            UpdateLibraryUI();
            SaveLibrary();
        }
    }



    //private void MusicPlayer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    //{
    //    MainWindow p = new MainWindow();
    //    p.Title = "YYYYY";
    //    p.Show();
    //}
}