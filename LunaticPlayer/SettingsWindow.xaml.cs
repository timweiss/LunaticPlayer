using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LunaticPlayer.Client;
using LunaticPlayer.Player;

namespace LunaticPlayer
{
    internal class SettingsViewModel : INotifyPropertyChanged
    {
        private int _imageCount;
        private int _songCount;

        public Version AppVersion { get; set; }
        public int SongCount
        {
            get => _songCount;
            set
            {
                _songCount = value;
                OnPropertyChanged("SongCount");
            }
        }

        public int ImageCount {
            get => _imageCount;
            set
            {
                _imageCount = value;
                OnPropertyChanged("ImageCount");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Database.Database _database;
        private SettingsViewModel _viewModel;

        private string _basePath = Configuration.GetInstance().Data.DataPath;

        public SettingsWindow(Database.Database database)
        {
            _database = database;
            _viewModel = new SettingsViewModel();
            _viewModel.AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            DataContext = _viewModel;
            InitializeComponent();
            this.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            //Storyboard sb = this.FindResource("FadeAppInfo") as Storyboard;
            //Storyboard.SetTarget(sb, this.AppInfo);
            //sb.Begin();
            ReloadStats();
            
        }

        private void ReloadStats()
        {
            _viewModel.SongCount = _database.GetSongCount();
            _viewModel.ImageCount = Directory.GetFiles(System.IO.Path.Combine(_basePath, SongManager.ImageFolder), "*", SearchOption.TopDirectoryOnly)
                .Length;

            // DataContext = _viewModel;
        }

        private void DeleteAllCoverImagesButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(System.IO.Path.Combine(_basePath, SongManager.ImageFolder)))
            {
                foreach (string filename in Directory.EnumerateFiles(System.IO.Path.Combine(_basePath, SongManager.ImageFolder)))
                {
                    Console.WriteLine("Deleting File " + filename);
                    File.Delete(filename);
                }

                ReloadStats();
            }
        }

        private void DeleteCoverImagesOfLast30Days_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(System.IO.Path.Combine(_basePath, SongManager.ImageFolder)))
            {
                foreach (string filename in Directory.EnumerateFiles(System.IO.Path.Combine(_basePath, SongManager.ImageFolder)))
                {
                    if (File.GetCreationTime(filename) <= DateTime.Now - TimeSpan.FromDays(30))
                    {
                        Console.WriteLine("Deleting File " + filename);
                        File.Delete(filename);
                    }
                }

                ReloadStats();
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void DeleteDatabaseEntriesButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Deleting all songs from SongHistory");
            _database.RemoveAllSongs();
            ReloadStats();
        }

        private void DeleteDatabaseEntriesTodayButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Deleting all songs of today from SongHistory");
            _database.RemoveSongsOfToday();
            ReloadStats();
        }
    }
}
