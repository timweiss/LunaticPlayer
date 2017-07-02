using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LunaticPlayer.Classes;
using Newtonsoft.Json;

namespace LunaticPlayer.Helpers
{
    internal static class SongActions
    {
        /// <summary>
        /// Handles the context menu click action.
        /// </summary>
        /// <param name="action">Which action should be performed.</param>
        internal static void HandleClick(CMenuAction action, Song song)
        {
            switch (action)
            {
                case CMenuAction.CopyToClipboard:
                    Clipboard.SetText($"Artist: {song.ArtistName}, Circle: {song.CircleName}, Title: {song.Title}");
                    break;
                case CMenuAction.CopyJsonToClipboard:
                    Clipboard.SetText(JsonConvert.SerializeObject(song));
                    break;
                case CMenuAction.SearchOnGoogle:
                    System.Diagnostics.Process.Start($"https://www.google.com/search?q={song.ArtistName}+{song.Title}");
                    break;
                case CMenuAction.SearchOnTw:
                    System.Diagnostics.Process.Start($"https://en.touhouwiki.net/index.php?search={song.CircleName}");
                    break;
                case CMenuAction.ShowDetails:
                    var details = new SongDetailsWindow(song);
                    details.Show();
                    break;
            }
        }
    }
}
