using System;
using System.Collections.Generic;
using System.Linq;
using LunaticPlayer.Classes;
using LunaticPlayer.Client;
using LunaticPlayer.Database;
using Newtonsoft.Json;

namespace LunaticPlayer.Player
{
    /// <summary>
    /// Manages and initializes the song history.
    /// </summary>
    public class SongHistoryManager
    {
        public List<Song> SongHistory { get; private set; }
        public Database.Database Database { get; set; }

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
            if (SongHistory.Any())
            {
                if (((SongHistory.Last().StartTime - song.StartTime) < TimeSpan.FromSeconds(1)) &&
                    !((SongHistory.Last().StartTime - song.StartTime) < TimeSpan.FromSeconds(-1)))
                {
                    return;
                }
            }

            SongHistory.Add(song);

            StoreHistory(song);
        }

        /// <summary>
        /// Loads the song history out of the database.
        /// </summary>
        private void InitializeHistory()
        {
            Database = new Database.Database(Configuration.GetInstance().Data.DataPath);

            SongHistory = Database.GetAllSongs();
        }

        /// <summary>
        /// Adds the last song to the database.
        /// </summary>
        private void StoreHistory(Song lastSong)
        {
            Database.AddSong(lastSong);
        }

        /// <summary>
        /// Loads the song history from the JSON file. 
        /// This should not be used in favor of storing history in the database.
        /// </summary>
        [Obsolete]
        private void InitializeHistoryJson()
        {
            if (System.IO.File.Exists("songhist.json"))
            {
                var json = System.IO.File.ReadAllText("songhist.json");

                SongHistory = JsonConvert.DeserializeObject<List<Song>>(json);
            }
        }

        /// <summary>
        /// Stores the current state of the song history in the JSON file.
        /// As with <see cref="InitializeHistoryJson"/>, this shouldn't be used in favor of the database.
        /// </summary>
        [Obsolete]
        private void StoreHistoryJson()
        {
            var json = JsonConvert.SerializeObject(SongHistory);

            if (json != null)
            {
                System.IO.File.WriteAllText("songhist.json", json);
            }
        }
    }
}
