using System;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace LunaticPlayer.Classes
{
    public class Song
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public string ArtistName { get; set; }
        public string CircleName { get; set; }
        public string AlbumName { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime => StartTime + Duration;

        public TimeSpan Duration { get; set; }
        public TimeSpan PlayedDuration { get; set; }

        public TimeSpan EndDuration => EndTime - DateTime.Now;

        public int ApiSongId { get; set; }
        public int ApiAlbumId { get; set; }

        public string AlbumArtFilename { get; set; }
        public string CirleArtFilename { get; set; }

        public string CircleArtist => $"{ArtistName} ({CircleName})";

        [JsonIgnore]
        public BitmapImage AlbumArt { get; set; }
    }
}


// Startuhrzeit: DateTime.Now - Played