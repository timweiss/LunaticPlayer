using LunaticPlayer.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaticPlayer.Database
{
    public class Database
    {
        private readonly SQLiteInterop _dbAccess;

        public Database()
        {
            _dbAccess = new SQLiteInterop();

            _dbAccess.Initialize();
        }

        public Database(string databasePath)
        {
            _dbAccess = new SQLiteInterop();

            _dbAccess.SetBasePath(databasePath);

            _dbAccess.Initialize();
        }

        public void SetBasePath(string path)
        {
            _dbAccess.SetBasePath(path);
        }

        /// <summary>
        /// Gets all played songs from the database.
        /// </summary>
        /// <returns>The list of played songs.</returns>
        public List<Song> GetAllSongs()
        {
            return _dbAccess.GetSongs();
        }

        /// <summary>
        /// Adds a song to the database.
        /// </summary>
        /// <param name="song">The song</param>
        public void AddSong(Song song)
        {
            _dbAccess.AddSong(song);
        }

        public void RemoveAllSongs()
        {
            _dbAccess.ClearSongTable();
        }

        public void RemoveSongsOfToday()
        {
            _dbAccess.ClearSongsOfToday();
        }

        public int GetSongCount()
        {
            return _dbAccess.GetSongCount();
        }
    }
}
