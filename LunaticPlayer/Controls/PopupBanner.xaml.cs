using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LunaticPlayer
{
    /// <summary>
    /// Data supplied to the <seealso cref="PopupBanner"/>.
    /// </summary>
    public class PopupBannerData
    {
        /// <summary>
        /// The message displayed in the banner.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Whether the banner should be closable.
        /// </summary>
        public bool Closable { get; set; }

        /// <summary>
        /// The level of importance (Info, Error,...)
        /// </summary>
        public PopupLevel Level { get; set; }

        /// <summary>
        /// The action that should run when the close button is clicked.
        /// </summary>
        public Action CloseAction { get; set; }
    }

    public enum PopupLevel
    {
        Error,
        Info
    }

    /// <summary>
    /// A small banner to show important messages inside of <see cref="MainWindow"/>.
    /// </summary>
    public partial class PopupBanner : UserControl
    {
        private PopupBannerData _data;

        public PopupBanner()
        {
            InitializeComponent();
        }

        public PopupBanner(PopupBannerData data)
        {
            InitializeComponent();

            DataContext = _data = data;
            SetupIcon();
        }

        /// <summary>
        /// Switches between the error and info icon.
        /// </summary>
        private void SetupIcon()
        {
            switch (_data.Level)
            {
                case PopupLevel.Error:
                    MessageIcon.Source = new BitmapImage(new Uri(@"/LunaticPlayer;component/Resources/error_black_92.png", UriKind.Relative));
                    break;
                case PopupLevel.Info:
                    MessageIcon.Source = new BitmapImage(new Uri(@"/LunaticPlayer;component/Resources/info_black_92.png", UriKind.Relative));
                    break;
            }
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _data.CloseAction?.Invoke();
        }
    }
}
