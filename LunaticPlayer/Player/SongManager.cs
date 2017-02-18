using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunaticPlayer.Classes;

namespace LunaticPlayer.Player
{
    public class SongManager
    {
        private GSRadioAPI.ApiClient _api;
        private Song _currentSong;

        private const int updateTolerance = 15;

        public async Task<Song> CurrentSong()
        {
            if (_currentSong == null || _currentSong.EndTime - DateTime.Now <= TimeSpan.FromSeconds(updateTolerance) || _currentSong.EndTime - DateTime.Now <= TimeSpan.FromSeconds(-updateTolerance))
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

        }

        public SongManager(GSRadioAPI.ApiClient client)
        {
            _api = client;
        }
    }
}
