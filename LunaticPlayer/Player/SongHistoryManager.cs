using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunaticPlayer.Classes;
using Newtonsoft.Json;

namespace LunaticPlayer.Player
{
    class SongHistoryManager
    {
        public List<Song> SongHistory { get; private set; }

        public SongHistoryManager()
        {
            SongHistory = new List<Song>();

            InitializeHistory();
        }

        /// <summary>
        /// Adds a song to the history.
        /// </summary>
        /// <param name="song">The song that should be added.</param>
        public void AddSongToHistory(Song song)
        {
            SongHistory.Add(song);

            StoreHistory();
        }

        /// <summary>
        /// Loads the song history out of the JSON file.
        /// </summary>
        private void InitializeHistory()
        {
            if (System.IO.File.Exists("songhist.json"))
            {
                var json = System.IO.File.ReadAllText("songhist.json");

                SongHistory = JsonConvert.DeserializeObject<List<Song>>(json);
            }
        }

        /// <summary>
        /// Stores the song history in a JSON file. Let's just hope the files won't get too large.
        /// </summary>
        private void StoreHistory()
        {
            var json = JsonConvert.SerializeObject(SongHistory);

            if (json != null)
            {
                System.IO.File.WriteAllText("songhist.json", json);
            }
        }
    }
}
