using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
using LunaticPlayer.Player;

namespace LunaticPlayer
{
    internal class SettingsViewModel
    {
        public Version AppVersion { get; set; }
    }

    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Database.Database _database;

        public SettingsWindow(Database.Database database)
        {
            _database = database;
            var viewModel = new SettingsViewModel();
            viewModel.AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void DeleteAllCoverImagesButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(SongManager.ImageLocation))
            {
                foreach (string filename in Directory.EnumerateFiles(SongManager.ImageLocation))
                {
                    Console.WriteLine("Deleting File " + filename);
                    File.Delete(filename);
                }
            }
        }

        private void DeleteCoverImagesOfLast30Days_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(SongManager.ImageLocation))
            {
                foreach (string filename in Directory.EnumerateFiles(SongManager.ImageLocation))
                {
                    if (File.GetCreationTime(filename) <= DateTime.Now - TimeSpan.FromDays(30))
                    {
                        Console.WriteLine("Deleting File " + filename);
                        File.Delete(filename);
                    }
                }
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
        }

        private void DeleteDatabaseEntriesTodayButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Deleting all songs of today from SongHistory");
            _database.RemoveSongsOfToday();
        }
    }
}
