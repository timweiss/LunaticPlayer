using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using LunaticPlayer.Classes;
using LunaticPlayer.Helpers;
using LunaticPlayer.Player;

namespace LunaticPlayer
{
    /// <summary>
    /// Interaktionslogik für SongDetailsWindow.xaml
    /// </summary>
    public partial class SongDetailsWindow : Window
    {
        private readonly Song _displayedSong;

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
                if (File.Exists(Path.Combine(SongManager.ImageLocation, _displayedSong.AlbumArtFilename)))
                {
                    var bmi = new BitmapImage();
                    bmi.BeginInit();
                    bmi.UriSource = new Uri(Path.Combine(SongManager.ImageLocation, _displayedSong.AlbumArtFilename),
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
