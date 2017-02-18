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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LunaticPlayer.GSRadioAPI;

namespace LunaticPlayer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Player.RadioPlayer _radioPlayer;
        private ApiClient _apiClient;
        private Player.SongManager _songManager;

        public MainWindow()
        {
            InitializeComponent();

            _radioPlayer = new Player.RadioPlayer();
            _apiClient = new ApiClient();
            _songManager = new Player.SongManager(_apiClient);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var rad = await _songManager.CurrentSong();

            MessageBox.Show($"Song: {rad.Title}\nAlbum: {rad.AlbumName}\nArtist: {rad.ArtistName}\nCircle: {rad.CircleName}\nDuration: {rad.Duration.ToString("mm':'ss")}\nLied endet in {rad.EndDuration.ToString("mm")}:{rad.EndDuration.ToString("ss")}");
        }
    }
}
