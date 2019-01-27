using System;
using System.Threading.Tasks;
using LunaticPlayer.Classes;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using LunaticPlayer.Client;

namespace LunaticPlayer.Player
{
    public class SongManager
    {
        private readonly GRadioAPI.IApiClient _api;
        private Song _currentSong;

        public readonly SongHistoryManager SongHistory;

        private const int UpdateTolerance = 2;
        public const string ImageFolder = "images";
        public string ImageLocation = Path.Combine(Configuration.GetInstance().Data.DataPath, ImageFolder);
        private const string GrImageHost = "https://gensokyoradio.net/images/albums/200/";

        /// <summary>
        /// Gets the current song. If the current song is over, this function calls the LoadSong function.
        /// </summary>
        /// <returns>Currently playing song.</returns>
        public async Task<Song> CurrentSong()
        {
            if (_currentSong == null || _currentSong.EndTime - DateTime.Now <= TimeSpan.FromSeconds(UpdateTolerance))
            {
                await LoadSong();

                return _currentSong;
            }

            return _currentSong;
        }

        /// <summary>
        /// This function loads the current song of the API into the SongManager.
        /// </summary>
        /// <returns></returns>
        private async Task LoadSong()
        {
            await _api.FetchRawApiData();

            _currentSong = _api.PlayingSong();
            _currentSong.AlbumArt = UpdateCoverImage();

            SongHistory.AddSongToHistory(_currentSong);
        }

        /// <summary>
        /// Downloads the album art (if any) and returns it for usage in the UI.
        /// </summary>
        /// <returns>The current album art of the song.</returns>
        private BitmapImage UpdateCoverImage()
        {
            if (_currentSong.AlbumArtFilename != "")
            {
                if (!Directory.Exists(ImageLocation))
                    Directory.CreateDirectory(ImageLocation);

                // don't download the art twice
                if (!File.Exists(Path.Combine(ImageLocation, _currentSong.AlbumArtFilename)))
                {
                    using (var client = new WebClient())
                    {
                        var imageUri = Path.Combine(ImageLocation, _currentSong.AlbumArtFilename);

                        try
                        {
                            client.DownloadFile(GrImageHost + _currentSong.AlbumArtFilename, imageUri);
                        }
                        catch (Exception exception)
                        {
                            if (exception is WebException webException)
                            {
                                Console.WriteLine(webException.Status);
                            }
                            Console.WriteLine($"Could not download cover image:\n{imageUri}");
                        }
                    }
                }

                var bmi = new BitmapImage();
                bmi.BeginInit();
                bmi.UriSource = new Uri(Path.Combine(ImageLocation, _currentSong.AlbumArtFilename), UriKind.Relative);
                bmi.CacheOption = BitmapCacheOption.OnLoad;
                bmi.EndInit();

                return bmi;
            }

            return null;
        }

        public SongManager(GRadioAPI.IApiClient client)
        {
            _api = client;
            SongHistory = new SongHistoryManager();
            
        }
    }
}
