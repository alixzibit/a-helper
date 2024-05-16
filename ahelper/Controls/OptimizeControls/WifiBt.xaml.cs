using ahelper.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace ahelper.Controls.OptimizeControls
{
    /// <summary>
    /// Interaction logic for WifiBt.xaml
    /// </summary>
    public partial class WifiBt : UserControl
    {
        WifiBtSwitch wifiBtSwitch = new WifiBtSwitch();
        public WifiBt()
        {
            InitializeComponent();
            LoadInitialToggleState();
        }

        private void LoadInitialToggleState()
        {
            ToggleWifi.IsChecked = wifiBtSwitch.IsWifiEnabled();
            ToggleBlu.IsChecked = wifiBtSwitch.IsBluetoothEnabled();
        }

        public bool IsWifiEnabled
        {
            get { return ToggleWifi.IsChecked ?? false; } }

        public bool IsBluetoothEnabled
        {
            get { return ToggleBlu.IsChecked ?? false; }
        }
    

    public void ToggleWifi_Checked(object sender, RoutedEventArgs e)
        {
            ToggleWifi.IsChecked = true;
        }

        public void ToggleWifi_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleWifi.IsChecked = false;
        }


        public void ToggleBlu_Checked(object sender, RoutedEventArgs e)
        {
            ToggleBlu.IsChecked = true;
        }

        public void ToggleBlu_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleBlu.IsChecked = false;
        }

    }
}
