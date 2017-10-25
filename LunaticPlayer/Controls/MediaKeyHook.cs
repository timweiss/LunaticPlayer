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
            _hotKeyManager.Register(Key.MediaPlayPause, ModifierKeys.None);
            _hotKeyManager.Register(Key.Pause, ModifierKeys.None);
            _hotKeyManager.Register(Key.Play, ModifierKeys.None);
            
            _hotKeyManager.KeyPressed += HotKeyManagerPressed;
        }
        
        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            switch (e.HotKey.Key)
            {
                case Key.MediaStop when _mainWindow.IsPlaying:
                    _mainWindow.PlayButtonClicked();
                    break;
                case Key.MediaPlayPause:
                    _mainWindow.PlayButtonClicked();
                    break;
            }
        }
    }

}