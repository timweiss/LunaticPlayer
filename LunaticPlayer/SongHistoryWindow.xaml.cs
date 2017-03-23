using System.Windows;
using LunaticPlayer.Player;

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
    }
}
