using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LunaticPlayer.Classes;

namespace LunaticPlayer.GRadioAPI
{
    public class ApiClient
    {
        public static string StreamUrl = "http://stream.gensokyoradio.net:8000/stream/1/";
        public const string ApiUrl = "https://gensokyoradio.net/xml/";

        public StructuredApiData CurrentStructuredApiData { get; private set; }

        /// <summary>
        /// Converts the raw API data into the song class.
        /// <seealso cref="LunaticPlayer.Classes.Song">
        /// </summary>
        /// <returns>The song fetched from the API.</returns>
        public Song PlayingSong()
        {
            var sYear = 0;
            if (CurrentStructuredApiData.SongInfo["YEAR"] != "")
            {
                sYear = Convert.ToInt32(CurrentStructuredApiData.SongInfo["YEAR"]);
            }

            var sId = 0;
            if (CurrentStructuredApiData.MiscInfo["SONGID"] != "")
            {
                sId = Convert.ToInt32(CurrentStructuredApiData.MiscInfo["SONGID"]);
            }

            var sAid = 0;
            if (CurrentStructuredApiData.MiscInfo["ALBUMID"] != "")
            {
                sId = Convert.ToInt32(CurrentStructuredApiData.MiscInfo["ALBUMID"]);
            }

#if DEBUG
            Console.WriteLine("DEBUGLOG: Alle Informationen der API");
            foreach (var item in CurrentStructuredApiData.SongInfo)
                Console.WriteLine($"{item.Key}: {item.Value}");
            foreach(var item in CurrentStructuredApiData.SongTimes)
                Console.WriteLine($"{item.Key}: {item.Value}");
            foreach (var item in CurrentStructuredApiData.MiscInfo)
                Console.WriteLine($"{item.Key}: {item.Value}");
#endif

            return new Song()
            {
                Title = CurrentStructuredApiData.SongInfo["TITLE"],
                Year = sYear,
                Duration = TimeSpan.FromSeconds(Convert.ToInt32(CurrentStructuredApiData.SongTimes["DURATION"])),
                PlayedDuration = TimeSpan.FromSeconds(Convert.ToInt32(CurrentStructuredApiData.SongTimes["PLAYED"])),
                StartTime = DateTime.Now.Add(-TimeSpan.FromSeconds(Convert.ToInt32(CurrentStructuredApiData.SongTimes["PLAYED"]))),
                AlbumName = CurrentStructuredApiData.SongInfo["ALBUM"],
                ArtistName = CurrentStructuredApiData.SongInfo["ARTIST"],
                CircleName = CurrentStructuredApiData.SongInfo["CIRCLE"],
                ApiSongId = sId,
                ApiAlbumId = sAid,
                AlbumArtFilename = CurrentStructuredApiData.MiscInfo["ALBUMART"],
                CirleArtFilename = CurrentStructuredApiData.MiscInfo["CIRCLEART"]
            };
        }

        /// <summary>
        /// Downloads any data from the GensokyoRadio XML API and stores it in the class.
        /// </summary>
        /// <returns></returns>
        public async Task FetchRawApiData()
        {
            string rawXmlResult = "";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(ApiUrl))
            using (HttpContent content = response.Content)
            {
                rawXmlResult = await content.ReadAsStringAsync();

                XmlDocument xmlResult = new XmlDocument();
                xmlResult.LoadXml(rawXmlResult);

                var node = xmlResult.DocumentElement;

                var apiData = new StructuredApiData();

                List<string> sourceNodes = new List<string>() { "SERVERINFO", "SONGINFO", "SONGTIMES", "MISC" };

                foreach (var item in sourceNodes)
                {
                    var itemNode = node.SelectSingleNode("//" + item);
                    var entries = itemNode.ChildNodes;

                    foreach (XmlNode entry in entries)
                    {
                        switch (item)
                        {
                            case "SERVERINFO":
                                if (entry.InnerText == null)
                                {
                                    apiData.ServerInfo.Add(entry.Name, "0");
                                    break;
                                }
                                apiData.ServerInfo.Add(entry.Name, entry.InnerText);
                                break;
                            case "SONGINFO":
                                if (entry.InnerText == null)
                                {
                                    apiData.SongInfo.Add(entry.Name, "0");
                                    break;
                                }
                                apiData.SongInfo.Add(entry.Name, entry.InnerText);
                                break;
                            case "SONGTIMES":
                                if (entry.InnerText == null)
                                {
                                    apiData.SongTimes.Add(entry.Name, "0");
                                    break;
                                }
                                apiData.SongTimes.Add(entry.Name, entry.InnerText);
                                break;
                            case "MISC":
                                if (entry.InnerText == null)
                                {
                                    apiData.MiscInfo.Add(entry.Name, "0");
                                    break;
                                }
                                apiData.MiscInfo.Add(entry.Name, entry.InnerText);
                                break;
                            default:
                                break;
                        }
                    }
                }

                CurrentStructuredApiData = apiData;
            }
        }
    }

    public class StructuredApiData
    {
        public Dictionary<string, string> ServerInfo;
        public Dictionary<string, string> SongInfo;
        public Dictionary<string, string> SongTimes;
        public Dictionary<string, string> MiscInfo;

        public StructuredApiData()
        {
            ServerInfo = new Dictionary<string, string>();
            SongInfo = new Dictionary<string, string>();
            SongTimes = new Dictionary<string, string>();
            MiscInfo = new Dictionary<string, string>();
        }
    }
}
