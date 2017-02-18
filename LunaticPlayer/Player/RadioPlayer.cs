using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LunaticPlayer.Player
{
    class RadioPlayer
    {
        private MediaPlayer player;

        public bool Muted => player.Volume == 0.0;

        public double Volume => player.Volume;

        public RadioPlayer()
        {
            player = new MediaPlayer();
        }

        public void PlayFromUrl(string url)
        {
            player.Open(new Uri(url));
            player.Play();
            player.Volume = 0.5;
        }

        public void Stop()
        {
            player.Stop();
        }

        //TODO: Lautstärke richtig einstellen
        public void SetVolume(double volume)
        {
            player.Volume = volume;
        }

        public void ToggleMute(double volume)
        {
            if (player.Volume == 0.0)
            {
                player.Volume = volume / 10;
            }
            else
            {
                player.Volume = 0.0;
            }
        }
    }
}
