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
using System.Diagnostics;
using System.Timers;
using LunaticPlayer.Classes;

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

        private bool _isPlaying;

        private Timer _interfaceTimer;

        private Song _currentSong;

        public MainWindow()
        {
            InitializeComponent();

            _radioPlayer = new Player.RadioPlayer();
            _apiClient = new ApiClient();
            _songManager = new Player.SongManager(_apiClient);

            _interfaceTimer = new Timer();
            _interfaceTimer.Interval = 1000;
            _interfaceTimer.Elapsed += ReloadInterface;
        }

        private void ReloadInterface(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => UpdateSong());
        }

        private async void UpdateSong()
        {
            DataContext = _currentSong = await _songManager.CurrentSong();
            RemainingTime.Text = _currentSong.EndDuration.ToString("mm':'ss") + " verbleibend";
            if (_currentSong.AlbumArt != null)
            {
                AlbumArt.Source = _currentSong.AlbumArt;
                AlbumArt.Width = 60;
                SongDataContainer.Width = 150;
            }
            else
            {
                AlbumArt.Width = 0;
                SongDataContainer.Width = 210;
            }
            this.Title = "LP: " + _currentSong.Title;
        }

        private void PlayButtonClicked()
        {
            if (_isPlaying)
            {
                _radioPlayer.Stop();
                _isPlaying = false;
                TBPlayButton.Description = "Play";

                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/play_mat.ico";
                TBPlayButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            }
            else
            {
                _radioPlayer.PlayFromUrl(ApiClient.StreamUrl);
                _isPlaying = true;
                TBPlayButton.Description = "Stop";

                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/pause_mat.ico";
                TBPlayButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            }

            UpdateSong();
            _interfaceTimer.Start();
        }


        // Events

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PlayButtonClicked();
        }

        private void ThumbButtonInfo_Click(object sender, EventArgs e)
        {
            PlayButtonClicked();
        }

        private void TBMuteButton_Click(object sender, EventArgs e)
        {
            _radioPlayer.ToggleMute();

            if (_radioPlayer.Muted)
            {
                TBMuteButton.Description = "Unmute";
                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/unmute_mat.ico";
                TBMuteButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            }
            else
            {
                TBMuteButton.Description = "Mute";
                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/mute_mat.ico";
                TBMuteButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
            }
        }
    }
}
