using System;
using System.Windows.Input;
using GlobalHotKey;

namespace LunaticPlayer.Client
{
    public class MediaKeyHook
    {
        private HotKeyManager _hotKeyManager;
        private MainWindow _mainWindow;

        public bool KeysRegistered { get; set; }

        /// <summary>
        /// Subscribes to play/pause button for controlling the play button.
        /// </summary>
        /// <param name="mainWindow"></param>
        public void Subscribe(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _hotKeyManager = new HotKeyManager();

            RegisterHotkeys();
        }

        public void RegisterHotkeys()
        {
            if(_mainWindow == null || _hotKeyManager == null)
                throw new Exception("MediaKeyHook wasn't initialized properly.");

            // Make sure the player doesn't crash.
            try
            {
                _hotKeyManager.Register(Key.MediaPlayPause, ModifierKeys.None);
                _hotKeyManager.Register(Key.Pause, ModifierKeys.None);
                _hotKeyManager.Register(Key.Play, ModifierKeys.None);

                _hotKeyManager.KeyPressed += HotKeyManagerPressed;
                KeysRegistered = true;
            }
            catch (Exception e)
            {
                KeysRegistered = false;
            }
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