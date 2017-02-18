using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunaticPlayer.Classes;
using System.Windows.Media.Imaging;
using System.IO;
using System.Net;

namespace LunaticPlayer.Player
{
    public class SongManager
    {
        private GRadioAPI.ApiClient _api;
        private Song _currentSong;

        private const int updateTolerance = 2;
        private const string imageLocation = "images";
        private const string gsImageHost = "https://gensokyoradio.net/images/albums/200/";

        /// <summary>
        /// Gets the current song. If the current song is over, this function calls the LoadSong function.
        /// </summary>
        /// <returns>Currently playing song.</returns>
        public async Task<Song> CurrentSong()
        {
            if (_currentSong == null || _currentSong.EndTime - DateTime.Now <= TimeSpan.FromSeconds(updateTolerance))
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
        }

        /// <summary>
        /// Downloads the album art (if any) and returns it for usage in the UI.
        /// </summary>
        /// <returns>The current album art of the song.</returns>
        private BitmapImage UpdateCoverImage()
        {
            if (_currentSong.AlbumArtFilename != "")
            {
                if (!Directory.Exists(imageLocation))
                    Directory.CreateDirectory(imageLocation);

                // don't download the art twice
                if (!File.Exists(Path.Combine(imageLocation, _currentSong.AlbumArtFilename)))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(gsImageHost + _currentSong.AlbumArtFilename, Path.Combine(imageLocation,_currentSong.AlbumArtFilename));
                    }
                }

                var bmi = new BitmapImage();
                bmi.BeginInit();
                bmi.UriSource = new Uri(Path.Combine(imageLocation, _currentSong.AlbumArtFilename), UriKind.Relative);
                bmi.CacheOption = BitmapCacheOption.OnLoad;
                bmi.EndInit();

                return bmi;
            }

            return null;
        }

        public SongManager(GRadioAPI.ApiClient client)
        {
            _api = client;
        }
    }
}
