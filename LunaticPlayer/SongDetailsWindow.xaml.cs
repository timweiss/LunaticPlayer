using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using LunaticPlayer.Classes;
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
        }
    }
}
