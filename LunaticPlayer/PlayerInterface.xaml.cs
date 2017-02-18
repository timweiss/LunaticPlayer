using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LunaticPlayer.GRadioAPI;
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

        /// <summary>
        /// Reloads the interface as the UI thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadInterface(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => UpdateSong());
        }

        /// <summary>
        /// Updates any song information and UI stuff.
        /// </summary>
        private async void UpdateSong()
        {
            DataContext = _currentSong = await _songManager.CurrentSong();
            RemainingTime.Text = _currentSong.EndDuration.ToString("mm':'ss") + " remaining";
            if (_currentSong.AlbumArt != null)
            {
                AlbumArtContainer.Padding = new Thickness(10);
                AlbumArt.Source = _currentSong.AlbumArt;
                AlbumArt.Width = 60;
            }
            else
            {
                AlbumArtContainer.Padding = new Thickness(0);
                AlbumArt.Width = 0;
            }
            this.Title = $"LP: {_currentSong.Title} - {_currentSong.ArtistName}";
        }

        /// <summary>
        /// Starts/stops the audio stream and updates any UI stuff like buttons.
        /// </summary>
        private void PlayButtonClicked()
        {
            if (_isPlaying)
            {
                _radioPlayer.Stop();
                _isPlaying = false;
                TBPlayButton.Description = "Play";

                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/play_mat.ico";
                TBPlayButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

                var button = new Uri("pack://application:,,,/LunaticPlayer;component/Resources/play_128.png");
                PlayButton.Background = new ImageBrush(new BitmapImage(button));
            }
            else
            {
                _radioPlayer.PlayFromUrl(ApiClient.StreamUrl);
                _isPlaying = true;
                TBPlayButton.Description = "Stop";

                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/pause_mat.ico";
                TBPlayButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

                var button = new Uri("pack://application:,,,/LunaticPlayer;component/Resources/stop_128.png");
                PlayButton.Background = new ImageBrush(new BitmapImage(button));
            }

            UpdateSong();
            _interfaceTimer.Start();
        }

        /// <summary>
        /// Mutes / unmutes the radio stream and updates UI stuff like buttons.
        /// </summary>
        private void MuteRadioStream()
        {
            _radioPlayer.ToggleMute();

            if (_radioPlayer.Muted)
            {
                TBMuteButton.Description = "Unmute";
                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/unmute_mat.ico";
                TBMuteButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

                var appUri = new Uri("pack://application:,,,/LunaticPlayer;component/Resources/unmute_92.png");
                MuteButton.Background = new ImageBrush(new BitmapImage(appUri));
            }
            else
            {
                TBMuteButton.Description = "Mute";
                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/mute_92.png";
                TBMuteButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

                var appUri = new Uri("pack://application:,,,/LunaticPlayer;component/Resources/mute_92.png");
                MuteButton.Background = new ImageBrush(new BitmapImage(appUri));
            }
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
            MuteRadioStream();
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            MuteRadioStream();
        }
    }
}
