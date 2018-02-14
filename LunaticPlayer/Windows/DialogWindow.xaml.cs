using System.Windows;

namespace LunaticPlayer.Windows
{
    /// <summary>
    /// Basic dialog window
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
