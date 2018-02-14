using System;
using System.Windows;
using System.Windows.Controls;

namespace LunaticPlayer.Controls
{
    public class VolumeBarData
    {
        public double Volume { get; set; }
    }

    /// <summary>
    /// Simple volume bar
    /// </summary>
    public partial class VolumeBar : UserControl
    {
        public VolumeBarData Data { get; set; }
        public Action OnValueChange { get; set; }

        public VolumeBar(VolumeBarData data)
        {
            InitializeComponent();

            DataContext = Data = data;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Data.Volume = VolumeSlider.Value;

            OnValueChange?.Invoke();
        }
    }
}
