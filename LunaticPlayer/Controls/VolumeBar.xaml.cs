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

namespace LunaticPlayer.Controls
{
    public class VolumeBarData
    {
        public double Volume { get; set; }
    }

    /// <summary>
    /// Interaktionslogik für VolumeBar.xaml
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
