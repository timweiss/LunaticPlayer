using System;
using System.Net.Http;
using System.Threading.Tasks;
using LunaticPlayer.Classes;
using LunaticPlayer.GRadioAPI.Classes;
using LunaticPlayer.GRadioAPI.Tools;
using Newtonsoft.Json;

namespace LunaticPlayer.GRadioAPI.Clients
{
    public class JsonApiClient : IApiClient
    {
        public static string StreamUrl = "http://stream.gensokyoradio.net:8000/stream/1/";
        public const string ApiUrl = "https://gensokyoradio.net/json/";

        public ApiSong CurrentApiSong { get; private set; }

        /// <summary>
        /// Converts the raw API data into the <seealso cref="Song" /> class.
        /// </summary>
        /// <returns>The song fetched from the API.</returns>
        public Song PlayingSong()
        {
            var songIdSuccess = Int32.TryParse(CurrentApiSong.SongData.SongId, out var songId);
            var albumIdSuccess = Int32.TryParse(CurrentApiSong.SongData.AlbumId, out var albumId);
            var yearSucccess = Int32.TryParse(CurrentApiSong.SongInfo.Year, out var year);

            return new Song()
            {
                Title = CurrentApiSong.SongInfo.Title,
                Year = year,
                Duration = TimeSpan.FromSeconds(Convert.ToInt32(CurrentApiSong.SongTimes.Duration)),
                PlayedDuration = TimeSpan.FromSeconds(Convert.ToInt32(CurrentApiSong.SongTimes.Played)),
                StartTime = DateTime.Now.Add(-TimeSpan.FromSeconds(Convert.ToInt32(CurrentApiSong.SongTimes.Played))),
                AlbumName = CurrentApiSong.SongInfo.Album,
                ArtistName = CurrentApiSong.SongInfo.Artist,
                CircleName = CurrentApiSong.SongInfo.Circle,
                ApiSongId = songId,
                ApiAlbumId = albumId,
                AlbumArtFilename = CurrentApiSong.Misc.AlbumArt,
                CirleArtFilename = CurrentApiSong.Misc.CircleArt
            };
        }

        /// <summary>
        /// Downloads any data from the GensokyoRadio XML API and stores it in the class.
        /// </summary>
        /// <returns></returns>
        public async Task FetchRawApiData()
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(ApiUrl))
            using (HttpContent content = response.Content)
            {
                var rawJsonResult = await content.ReadAsStringAsync();

                var jsonSong = JsonConvert.DeserializeObject<ApiSong>(rawJsonResult, new UnixDateTimeConverter());

                CurrentApiSong = jsonSong;
            }
        }

        /// <summary>
        /// Checks whether the API is reachable or not.
        /// </summary>
        /// <returns>Status</returns>
        public async Task<bool> CheckApiAccess()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(ApiUrl))
                {
                    if (response.IsSuccessStatusCode)
                        return true;

                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
