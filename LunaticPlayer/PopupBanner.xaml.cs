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
    /// Interaktionslogik für PopupBanner.xaml
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
