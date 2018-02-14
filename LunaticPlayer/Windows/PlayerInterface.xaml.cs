using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LunaticPlayer.GRadioAPI;
using System.Timers;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using LunaticPlayer.Classes;
using LunaticPlayer.Client;
using LunaticPlayer.Controls;
using LunaticPlayer.GRadioAPI.Clients;

namespace LunaticPlayer
{
    /// <summary>
    /// Main player window displaying current song and providing access to player controls.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Player.RadioPlayer _radioPlayer;
        private readonly Player.SongManager _songManager;

        private IApiClient _apiClient;

        private bool _isPlaying;

        private readonly ImageSource _fallbackAlbumIcon;
        private Timer _interfaceTimer;

        private Song _currentSong;
        
        /* Secondary windows */
        private SongDetailsWindow _detailsWindow;
        private SettingsWindow _settingsWindow;
        private SongHistoryWindow _historyWindow;

        private MediaKeyHook _mediaKeyHook;

        public MainWindow()
        {
            InitializeComponent();

            _radioPlayer = new Player.RadioPlayer();
            _apiClient = new JsonApiClient();
            _songManager = new Player.SongManager(_apiClient);
            
            _interfaceTimer = new Timer();
            _interfaceTimer.Interval = 1000;
            _interfaceTimer.Elapsed += ReloadInterface;

            _radioPlayer.SetVolume(Configuration.GetInstance().Data.Volume);

            LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            
            _fallbackAlbumIcon = new ImageSourceConverter()
                    .ConvertFromString("pack://application:,,,/LunaticPlayer;component/Resources/gr-album-fallback.png")
                as ImageSource;
        }

        /// <summary>
        /// Reloads interface as UI thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadInterface(object sender, EventArgs e)
        {
            Dispatcher.Invoke(UpdateSong);
        }

        private bool animationRun = false;

        private Song previousSong;

        /// <summary>
        /// Updates song information and UI stuff.
        /// </summary>
        private async void UpdateSong()
        {
            if (_currentSong != null && !animationRun && _currentSong.EndDuration.TotalSeconds <= 1)
            {
                animationRun = true;
                RunFadeOutAnimation();
            }

            DataContext = _currentSong = await _songManager.CurrentSong();

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
                RemainingTime.Text = "-" + _currentSong.EndDuration.ToString("mm':'ss");
                SongTime.Text = _currentSong.Duration.ToString("mm':'ss");
            }
            else
            {
                RemainingTime.Text = "Press play to start";
                SongTime.Text = "";
            }
            
            // If the image fails to load we will show the fallback image.
            AlbumArtContainer.Padding = new Thickness(10);
            AlbumArt.Source = _currentSong.AlbumArt ?? _fallbackAlbumIcon;
            AlbumArt.Width = 125;
            AlbumArt.Height = 125;
            this.Title = $"LP: {_currentSong.Title} - {_currentSong.ArtistName}";

            previousSong = _currentSong;
        }

        /// <summary>
        /// Starts/stops the audio stream and updates any UI stuff like buttons.
        /// 
        /// Also invoked when pressing the play/pause media button.
        /// </summary>
        public void PlayButtonClicked()
        {
            if (_isPlaying)
            {
                _radioPlayer.Stop();
                _isPlaying = false;
                TBPlayButton.Description = "Play";

                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/play_mat.ico";
                TBPlayButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

                var button = new Uri("pack://application:,,,/LunaticPlayer;component/Resources/lp-play-92.png");
                PlayButton.OpacityMask = new ImageBrush(new BitmapImage(button));
            }
            else
            {
                _radioPlayer.PlayFromUrl(ApiClient.StreamUrl);
                _radioPlayer.SetVolume(Configuration.GetInstance().Data.Volume);
                _isPlaying = true;
                TBPlayButton.Description = "Stop";

                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/pause_mat.ico";
                TBPlayButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

                var button = new Uri("pack://application:,,,/LunaticPlayer;component/Resources/lp-stop-92.png");
                PlayButton.OpacityMask = new ImageBrush(new BitmapImage(button));
            }

            UpdateSong();
            _interfaceTimer.Start();
        }

        /// <summary>
        /// Mutes / unmutes the radio stream and updates UI stuff like buttons.
        /// </summary>
        private void MuteRadioStream()
        {
            _radioPlayer.ToggleMute(Configuration.GetInstance().Data.Volume);

            if (_radioPlayer.Muted)
            {
                TBMuteButton.Description = "Unmute";
                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/unmute_mat.ico";
                TBMuteButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

                var appUri = new Uri("pack://application:,,,/LunaticPlayer;component/Resources/mute_92.png");
                MuteButton.Background = new ImageBrush(new BitmapImage(appUri));
            }
            else
            {
                TBMuteButton.Description = "Mute";
                var packUri = "pack://application:,,,/LunaticPlayer;component/Resources/mute_92.png";
                TBMuteButton.ImageSource = new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;

                var appUri = new Uri("pack://application:,,,/LunaticPlayer;component/Resources/voloff_92.png");
                MuteButton.Background = new ImageBrush(new BitmapImage(appUri));
            }
        }

        private VolumeBar volumeBar;

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
            if (_detailsWindow != null && _detailsWindow.IsVisible)
            {
                _detailsWindow.Focus();
                return;
            }

            _detailsWindow = new SongDetailsWindow(_currentSong);
            _detailsWindow.Show();
        }

        private void OptionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_settingsWindow != null && _settingsWindow.IsVisible)
            {
                _settingsWindow.Focus();
                return;
            }

            _settingsWindow = new SettingsWindow(_songManager.SongHistory.Database, _mediaKeyHook);
            _settingsWindow.Show();
        }

        private void SongListButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_historyWindow != null && _historyWindow.IsVisible)
            {
                _historyWindow.Focus();
                return;
            }
            
            _historyWindow = new SongHistoryWindow(_songManager.SongHistory);
            _historyWindow.Show();
        }

        private void VolumeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (volumeBar == null)
            {
                var vol = Configuration.GetInstance().Data.Volume;
                volumeBar = new VolumeBar(new VolumeBarData() { Volume = vol });
                volumeBar.Height = 21;
                volumeBar.Width = 100;
                volumeBar.OnValueChange = OnVolumeChange;
                volumeBar.Effect = new DropShadowEffect() { Direction = -90, Opacity = 0.4, BlurRadius = 20, };
            }

            if (this.VolumeBarContainer.Child == volumeBar)
            {
                if (volumeBar.Visibility == Visibility.Visible)
                {
                    volumeBar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    volumeBar.Visibility = Visibility.Visible;
                }

            }
            else
            {
                this.VolumeBarContainer.Child = volumeBar;
            }
        }

        #endregion

        /// <summary>
        /// Stops the radio player and closes all open windows (since PlayerInterface is the main window).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Configuration.GetInstance().Save();
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
            // check if API is usable 
            var apiAccess = await _apiClient.CheckApiAccess();

            if (!apiAccess)
            {
                // if not, try fallback API (XML)
                var fallback = new ApiClient();
                var fallbackAccess = await _apiClient.CheckApiAccess();

                // if this fails, display an error message (banner)
                if (!fallbackAccess)
                {
                    var bannerData = new PopupBannerData()
                    {
                        Closable = false,
                        Level = PopupLevel.Error,
                        CloseAction = ClosePopupBanner
                    };

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
                    _apiClient = fallback;
                }
            }
            else
            {
                UpdateSong();
            }


            _mediaKeyHook = new MediaKeyHook();
            _mediaKeyHook.Subscribe(this);
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

        private void OnVolumeChange()
        {
            _radioPlayer.SetVolume(volumeBar.Data.Volume);
            Configuration.GetInstance().Data.Volume = Math.Round(volumeBar.Data.Volume, 2);
        }

        public bool IsPlaying => _isPlaying;
    }
}
