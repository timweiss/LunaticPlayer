using System.Windows.Input;
using GlobalHotKey;

namespace LunaticPlayer.Controls
{

    internal class MediaKeyHook
    {
        private HotKeyManager _hotKeyManager;
        private MainWindow _mainWindow;

        /// <summary>
        /// Subscribes to play/pause button for controlling the play button.
        /// </summary>
        /// <param name="mainWindow"></param>
        public void Subscribe(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _hotKeyManager = new HotKeyManager();
            var hotKey = _hotKeyManager.Register(Key.MediaPlayPause, ModifierKeys.None);
            
            _hotKeyManager.KeyPressed += HotKeyManagerPressed;
        }
        
        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            _mainWindow.PlayButtonClicked();
        }
    }

}