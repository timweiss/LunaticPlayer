using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LunaticPlayer.GRadioAPI;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using LunaticPlayer.Classes;

namespace LunaticPlayer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Player.RadioPlayer _radioPlayer;
        private readonly ApiClient _apiClient;
        private readonly Player.SongManager _songManager;

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


            LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        /// <summary>
        /// Reloads the interface as the UI thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadInterface(object sender, EventArgs e)
        {
            Dispatcher.Invoke(UpdateSong);
        }

        private bool firstRun = true;
        private bool animationRun = false;

        private Song previousSong;

        /// <summary>
        /// Updates any song information and UI stuff.
        /// </summary>
        private async void UpdateSong()
        {
            if (_currentSong != null && !animationRun && _currentSong.EndDuration.TotalSeconds <= 3)
            {
                animationRun = true;
                RunFadeOutAnimation();
            }

            DataContext = _currentSong = await _songManager.CurrentSong();
            firstRun = false;

            if (_currentSong != previousSong)
            {
                if (previousSong == null)
                {
                    animationRun = false;
                    RunFadeInAnimation();
                }
                else if (previousSong.ApiSongId != _currentSong.ApiSongId) // prevents continuous fade in of same song
                {
                    animationRun = false;
                    RunFadeInAnimation();
                }
            }

            if (_isPlaying)
            {
                RemainingTime.Text = _currentSong.EndDuration.ToString("mm':'ss") + " remaining";
            }
            else
            {
                RemainingTime.Text = "Press play to start";
            }

            if (_currentSong.AlbumArt != null)
            {
                AlbumArtContainer.Padding = new Thickness(10);
                AlbumArt.Source = _currentSong.AlbumArt;
                AlbumArt.Width = 125;
                AlbumArt.Height = 125;
            }
            else
            {
                AlbumArtContainer.Padding = new Thickness(0);
                AlbumArt.Width = 0;
            }
            this.Title = $"LP: {_currentSong.Title} - {_currentSong.ArtistName}";

            previousSong = _currentSong;
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

        #region Button Events

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

        private void SongInfoButton_Click(object sender, RoutedEventArgs e)
        {
            SongDetailsWindow sdWindow = new SongDetailsWindow(_currentSong);
            sdWindow.Show();
        }

        private void OptionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow sWindow = new SettingsWindow(_songManager.SongHistory.Database);
            sWindow.Show();
        }

        private void SongListButton_OnClick(object sender, RoutedEventArgs e)
        {
            SongHistoryWindow sWindow = new SongHistoryWindow(_songManager.SongHistory);
            sWindow.Show();
        }

        #endregion

        /// <summary>
        /// Stops the radio player and closes all open windows (since PlayerInterface is the main window).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            _radioPlayer.Stop();
            Application.Current.Shutdown();
        }

        private PopupBanner _messageBanner;

        /// <summary>
        /// Is run after window is loaded and checks connection. 
        /// Displays currently playing track.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// TODO: add ability to refresh player after connection failiure
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var apiAccess = await _apiClient.CheckApiAccess();

            if (!apiAccess)
            {
                var bannerData = new PopupBannerData()
                {
                    Closable = false,
                    Level = PopupLevel.Error,
                    CloseAction = ClosePopupBanner
                };

                if (!apiAccess)
                    bannerData.Message = "Could not connect to the API";

                _messageBanner = new PopupBanner(bannerData);

                _messageBanner.Height = 40;
                _messageBanner.VerticalAlignment = VerticalAlignment.Top;
                _messageBanner.Effect = new DropShadowEffect() {BlurRadius = 20, Direction = -180};
                _messageBanner.Name = "MessageBanner";
                _messageBanner.Opacity = 0.0;

                this.MainContent.Children.Add(_messageBanner);

                Storyboard sb = this.FindResource("FadeInMessageBanner") as Storyboard;
                Storyboard.SetTarget(sb, _messageBanner);
                sb.Begin();
                HideSongInfo();
            }
            else
            {
                UpdateSong();
            }
        }

        private void HideSongInfo()
        {
            RunFadeOutAnimation();
        }

        private void ClosePopupBanner()
        {
            this.MainContent.Children.Remove(_messageBanner);
        }

        private void RunFadeOutAnimation()
        {
            Storyboard sb = this.FindResource("SongFadeOutStoryboard") as Storyboard;
            Storyboard.SetTarget(sb, this.PlayerContent);
            sb.Begin();
        }

        private void RunFadeInAnimation()
        {
            Storyboard sb = this.FindResource("SongFadeInStoryboard") as Storyboard;
            Storyboard.SetTarget(sb, this.PlayerContent);
            sb.Begin();
        }
    }
}
