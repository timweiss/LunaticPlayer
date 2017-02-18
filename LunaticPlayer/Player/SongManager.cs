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
        private GSRadioAPI.ApiClient _api;
        private Song _currentSong;

        private const int updateTolerance = 15;
        private const string imageLocation = "images";
        private const string gsImageHost = "https://gensokyoradio.net/images/albums/200/";

        public async Task<Song> CurrentSong()
        {
            if (_currentSong == null || _currentSong.EndTime - DateTime.Now <= TimeSpan.FromSeconds(updateTolerance))
            {
                await LoadSong();

                return _currentSong;
            }

            return _currentSong;
        }

        private async Task LoadSong()
        {
            await _api.FetchRawApiData();

            _currentSong = _api.PlayingSong();
            _currentSong.AlbumArt = UpdateCoverImage();
        }

        private BitmapImage UpdateCoverImage()
        {
            if (_currentSong.AlbumArtFilename != "")
            {
                if (!Directory.Exists(imageLocation))
                    Directory.CreateDirectory(imageLocation);

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

        public SongManager(GSRadioAPI.ApiClient client)
        {
            _api = client;
        }
    }
}
