using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using LunaticPlayer.Classes;
using LunaticPlayer.Client;
using LunaticPlayer.Helpers;
using LunaticPlayer.Player;

namespace LunaticPlayer
{
    /// <summary>
    /// Displays information for supplied <see cref="Song"/>
    /// </summary>
    public partial class SongDetailsWindow : Window
    {
        private readonly Song _displayedSong;

        private string _basePath = Configuration.GetInstance().Data.DataPath;

        public SongDetailsWindow(Song song)
        {
            InitializeComponent();

            DataContext = _displayedSong = song;
            Title = $"Details: {_displayedSong.ArtistName} - {_displayedSong.Title}";

            LoadCoverImage();
        }

        private void LoadCoverImage()
        {
            if (_displayedSong.AlbumArtFilename != "")
            {
                // we should have downloaded the cover image
                if (File.Exists(Path.Combine(System.IO.Path.Combine(_basePath, SongManager.ImageFolder), _displayedSong.AlbumArtFilename)))
                {
                    var bmi = new BitmapImage();
                    bmi.BeginInit();
                    bmi.UriSource = new Uri(Path.Combine(System.IO.Path.Combine(_basePath, SongManager.ImageFolder), _displayedSong.AlbumArtFilename),
                        UriKind.Relative);
                    bmi.CacheOption = BitmapCacheOption.OnLoad;
                    bmi.EndInit();

                    AlbumArt.Source = bmi;
                }
            }
            else
            {
                AlbumArtContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void CopyToClipboard_OnClick(object sender, RoutedEventArgs e)
        {
            SongActions.HandleClick(CMenuAction.CopyToClipboard, _displayedSong);
        }

        private void CopyJsonButton_OnClick(object sender, RoutedEventArgs e)
        {
            SongActions.HandleClick(CMenuAction.CopyJsonToClipboard, _displayedSong);
        }

        private void SearchOnGoogleButton_OnClick(object sender, RoutedEventArgs e)
        {
            SongActions.HandleClick(CMenuAction.SearchOnGoogle, _displayedSong);
        }

        private void SearchOnThWiki_OnClick(object sender, RoutedEventArgs e)
        {
            SongActions.HandleClick(CMenuAction.SearchOnTw, _displayedSong);
        }
    }
}
