using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

            DataContext = _songHistory;
            PopulateList();
        }

        private void PopulateList()
        {
            foreach (var song in _songHistory.SongHistory)
            {
                var item = new ListBoxItem();
                item.Content = song.Title + " - " + song.CircleArtist;
                SongList.Items.Add(item);
            }
        }
    }
}
