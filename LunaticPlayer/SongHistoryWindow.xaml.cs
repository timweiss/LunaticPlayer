using System;
using System.Windows;
using LunaticPlayer.Classes;
using LunaticPlayer.Player;
using Newtonsoft.Json;

namespace LunaticPlayer
{
    /// <summary>
    /// Interaktionslogik für SongHistoryWindow.xaml
    /// </summary>
    public partial class SongHistoryWindow : Window
    {
        private readonly SongHistoryManager _songHistory;

        public SongHistoryWindow(SongHistoryManager shManager)
        {
            InitializeComponent();
            _songHistory = shManager;

            // DataContext = _songHistory;
            PopulateList();
        }

        /// <summary>
        /// Fills the ListBox with <see cref="Classes.Song"/> items.
        /// </summary>
        private void PopulateList()
        {
            //foreach (var song in _songHistory.SongHistory)
            //{
            //    var item = new ListBoxItem();
            //    item.Content = song.Title + " - " + song.CircleArtist;
            //    SongList.Items.Add(item);
            //}
            SongList.ItemsSource = _songHistory.SongHistory;
        }

        private void CopyItem_OnClick(object sender, RoutedEventArgs e)
        {
            HandleClick(CMenuAction.CopyToClipboard);
        }

        private void SearchOnGoogle_OnClick(object sender, RoutedEventArgs e)
        {
            HandleClick(CMenuAction.SearchOnGoogle);
        }

        private void SearchOnTouhouWiki_OnClick(object sender, RoutedEventArgs e)
        {
            HandleClick(CMenuAction.SearchOnTw);
        }

        private void CopyJson_OnClick(object sender, RoutedEventArgs e)
        {
            HandleClick(CMenuAction.CopyJsonToClipboard);
        }

        /// <summary>
        /// Handles the context menu click action.
        /// </summary>
        /// <param name="action">Which action should be performed.</param>
        private void HandleClick(CMenuAction action)
        {
            if (SongList.SelectedIndex == -1) return;

            var song = (Song)SongList.Items[SongList.SelectedIndex];

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
            }

#if DEBUG
            Console.WriteLine($"[HistoryWindow]: Action Performed - {action}");
#endif
        }
    }

    enum CMenuAction
    {
        CopyToClipboard,
        CopyJsonToClipboard,
        SearchOnGoogle,
        SearchOnTw
    }
}
