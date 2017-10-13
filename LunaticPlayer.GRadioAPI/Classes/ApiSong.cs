using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunaticPlayer.GRadioAPI.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LunaticPlayer.GRadioAPI.Classes
{
    public class ApiSong
    {
        public ServerInfo ServerInfo { get; set; }
        public SongInfo SongInfo { get; set; }
        public SongTimes SongTimes { get; set; }
        public SongData SongData { get; set; }
        public MiscData Misc { get; set; }
    }

    public class ServerInfo
    {
        [JsonProperty(PropertyName = "LASTUPDATE")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime LastUpdate { get; set; }

        public int Servers { get; set; }
        public string Status { get; set; }
        public int Listeners { get; set; }
        public int Bitrate1 { get; set; }
    }

    public class SongInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string Circle { get; set; }
    }

    public class SongTimes
    {
        public string Duration { get; set; }
        public string Played { get; set; }
        public string Remaining { get; set; }

        [JsonProperty(PropertyName = "SONGSTART")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime SongStart { get; set; }

        [JsonProperty(PropertyName = "SONGEND")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime SongEnd { get; set; }
    }

    public class SongData
    {
        public string SongId { get; set; }
        public string AlbumId { get; set; }
        public string Rating { get; set; }
        public int TimesRated { get; set; }
    }

    public class MiscData
    {
        public string CircleLink { get; set; }
        public string AlbumArt { get; set; }
        public string CircleArt { get; set; }
        public string Offset { get; set; }

        [JsonProperty(PropertyName = "OFFSETTIME")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime OffsetTime { get; set; }
    }
}
