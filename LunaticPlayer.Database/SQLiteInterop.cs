using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunaticPlayer.Classes;
using LunaticPlayer.Database.Extensions;

namespace LunaticPlayer.Database
{
    class SQLiteInterop
    {
        private SQLiteConnection _sqliteDb;

        private string _basePath = ".\\";

        public void SetBasePath(string path)
        {
            _basePath = path;
        }

        /// <summary>
        /// Initializes the database connection and creates the required tables.
        /// </summary>
        public void Initialize()
        {
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);

            if(!File.Exists(Path.Combine(_basePath, "songhist.db")))
                SQLiteConnection.CreateFile(Path.Combine(_basePath, "songhist.db"));

            _sqliteDb = new SQLiteConnection($"Data Source={Path.Combine(_basePath, "songhist.db")};Version=3;");

            if(!IsDatabaseUsable())
                SetupDatabase();
        }

        /// <summary>
        /// Checks if the database is usable.
        /// </summary>
        /// <returns></returns>
        public bool IsDatabaseUsable()
        {
            var checkSql = "SELECT name FROM sqlite_master WHERE type='table' AND name='SongHistory';";
            SQLiteCommand checkCommand = new SQLiteCommand(checkSql, _sqliteDb);

            _sqliteDb.Open();
            var chk = (string)checkCommand.ExecuteScalar();
            _sqliteDb.Close();

            if (chk != null)
                return true;

            return false;
        }

        /// <summary>
        /// Creates the SongHistory table.
        /// </summary>
        public void SetupDatabase()
        {
            // Setup Table "SongHistory"
            var histSql = "CREATE TABLE SongHistory " +
                          "(title VARCHAR(255), year INT, " +
                          "artistName VARCHAR(255), circleName VARCHAR(255), albumName VARCHAR(255), " +
                          "startTime DATETIME, duration INT, grApiSongId INT, grApiAlbumId INT, " +
                          "albumArtFilename VARCHAR(128), circleArtFilename VARCHAR(128))";
            SQLiteCommand histCommand = new SQLiteCommand(histSql, _sqliteDb);

            // Write to database
            _sqliteDb.Open();
            histCommand.ExecuteNonQuery();
            _sqliteDb.Close();
        }

        /// <summary>
        /// Adds the song into the SongHistory table.
        /// </summary>
        /// <param name="song">The song</param>
        public void AddSong(Song song)
        {
            SQLiteCommand entry = new SQLiteCommand("INSERT INTO SongHistory (title, year, artistName, circleName, albumName, startTime, duration, grApiSongId, grApiAlbumId, albumArtFilename, circleArtFilename) " +
                                              "VALUES (@title, @year, @artistName, @circleName, @albumName, @startTime, @duration, @grApiSongId, @grApiAlbumId, @albumArtFilename, @circleArtFilename)", _sqliteDb);
            entry.Parameters.AddWithValue("@title", song.Title);
            entry.Parameters.AddWithValue("@year", song.Year);
            entry.Parameters.AddWithValue("@artistName", song.ArtistName);
            entry.Parameters.AddWithValue("@circleName", song.CircleName);
            entry.Parameters.AddWithValue("@albumName", song.AlbumName);
            entry.Parameters.AddWithValue("@startTime", song.StartTime);
            entry.Parameters.AddWithValue("@duration", song.Duration.TotalSeconds);
            entry.Parameters.AddWithValue("@grApiSongId", song.ApiSongId);
            entry.Parameters.AddWithValue("@grApiAlbumId", song.ApiAlbumId);
            entry.Parameters.AddWithValue("@albumArtFilename", song.AlbumArtFilename);
            entry.Parameters.AddWithValue("@circleArtFilename", song.CirleArtFilename);

            _sqliteDb.Open();
            entry.ExecuteNonQuery();
            _sqliteDb.Close();
        }

        /// <summary>
        /// Gets a list of all songs from the SongHistory table.
        /// </summary>
        /// <returns></returns>
        public List<Song> GetSongs()
        {
            var songs = new List<Song>();

            var querySql = "SELECT * FROM SongHistory ORDER BY startTime ASC";
            SQLiteCommand queryCommand = new SQLiteCommand(querySql, _sqliteDb);

            _sqliteDb.Open();
            SQLiteDataReader reader = queryCommand.ExecuteReader();
            while (reader.Read())
            {
                var s = new Song()
                {
                    Title = (string) reader["title"],
                    Year = (int) reader["year"],
                    ArtistName = (string) reader["artistName"],
                    CircleName = (string) reader["circleName"],
                    AlbumName = (string) reader["albumName"],
                    StartTime = (DateTime) reader["startTime"],
                    Duration = TimeSpan.FromSeconds((int) reader["duration"]),
                    ApiSongId = (int) reader["grApiSongId"],
                    ApiAlbumId = (int)reader["grApiAlbumId"],
                    AlbumArtFilename = (string) reader["albumArtFilename"],
                    CirleArtFilename = (string)reader["circleArtFilename"]
                };

                songs.Add(s);
            }
            _sqliteDb.Close();

            return songs;
        }

        public void ClearSongTable()
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM SongHistory", _sqliteDb);

            _sqliteDb.Open();
            command.ExecuteNonQuery();
            _sqliteDb.Close();

            Console.WriteLine("Deleted all entries of SongHistory table.");
        }

        public void ClearSongsOfToday()
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM SongHistory WHERE startTime>=@startTime", _sqliteDb);
            command.Parameters.AddWithValue("@startTime", DateTime.Today);

            _sqliteDb.Open();
            command.ExecuteNonQuery();
            _sqliteDb.Close();
        }

        public int GetSongCount()
        {
            var querySql = "SELECT COUNT(*) FROM SongHistory";
            SQLiteCommand queryCommand = new SQLiteCommand(querySql, _sqliteDb);
            _sqliteDb.Open();
            object temp = queryCommand.ExecuteScalar();

            int count = Convert.ToInt32(temp);
            _sqliteDb.Close();

            return count;
        }
    }
}
