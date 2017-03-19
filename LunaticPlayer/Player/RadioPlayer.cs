using System;
using System.Windows.Media;

namespace LunaticPlayer.Player
{
    class RadioPlayer
    {
        private readonly MediaPlayer _player;

        public bool Muted => _player.Volume == 0.0;

        public double Volume => _player.Volume;

        public RadioPlayer()
        {
            _player = new MediaPlayer();
        }

        public void PlayFromUrl(string url)
        {
            _player.Open(new Uri(url));
            _player.Play();
            _player.Volume = 0.5;
        }

        public void Stop()
        {
            _player.Stop();
        }

        //TODO: Lautstärke richtig einstellen
        public void SetVolume(double volume)
        {
            _player.Volume = volume;
        }

        public void ToggleMute()
        {
            _player.Volume = _player.Volume == 0.0 ? 0.5 : 0.0;
        }
    }
}
