using ahelper.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace ahelper.Controls
{
    /// <summary>
    /// Interaction logic for Optimize.xaml
    /// </summary>
    public partial class Optimize : UserControl
    {
        WifiBtSwitch wifiBtSwitch = new WifiBtSwitch();

        public Optimize()
        {
            InitializeComponent();
        }

        //private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.Source is TabControl) // Check if the event sender is actually the TabControl
        //    {
        //        var tabControl = sender as TabControl;
        //        if (tabControl.SelectedItem == servicesm_tab)
        //        {
        //            // Asynchronously load services and then focus the DataGrid
        //            await ServiceControl.LoadServicesAsync();  // Load services asynchronously
        //            ServiceControl.FocusDataGrid();  // Focus the DataGrid after services are loaded
        //        }
        //    }
        //}
        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl) // Check if the event sender is actually the TabControl
            {
                var tabControl = sender as TabControl;

                // First, check if the controls are initialized before attempting to set their visibility
                if (wifiBtControl != null)
                    wifiBtControl.Visibility = Visibility.Collapsed;
                if (ServiceControl != null)
                    ServiceControl.Visibility = Visibility.Collapsed;
                if (Syscleancontrol != null)
                    Syscleancontrol.Visibility = Visibility.Collapsed;

                // Then, based on the selected tab, make the appropriate control visible
                if (tabControl.SelectedItem == wifibt_tab && wifiBtControl != null)
                {
                    wifiBtControl.Visibility = Visibility.Visible;
                }
                else if (tabControl.SelectedItem == servicesm_tab && ServiceControl != null)
                {
                    ServiceControl.Visibility = Visibility.Visible;
                    // Asynchronously load services and then focus the DataGrid only if the control is not null
                    await ServiceControl.LoadServicesAsync(); // Load services asynchronously
                    ServiceControl.FocusDataGrid(); // Focus the DataGrid after services are loaded
                }
                else if (tabControl.SelectedItem == systemcleantab && Syscleancontrol != null)

                {
                    Syscleancontrol.Visibility = Visibility.Visible;
                }
            }
        }

        private SysCleanTw sysCleanTw = new SysCleanTw();

        //private async void optimizebtn_Click(object sender, RoutedEventArgs e)
        //{
        //    Opt_dialog dialog = new Opt_dialog();
        //    dialog.Show();  // Open the dialog non-modally to allow updates

        //    bool wifiEnabled = wifiBtControl.IsWifiEnabled;
        //    bool bluEnabled = wifiBtControl.IsBluetoothEnabled;

        //    // Perform the WiFi and Bluetooth operations and update the dialog
        //    await PerformWirelessOperations(wifiEnabled, bluEnabled, dialog);

        //    // Attempt to stop selected services regardless of which tab is active
        //    if (ServiceControl.Visibility == Visibility.Visible || ServiceControl.Visibility == Visibility.Collapsed)
        //    {
        //        dialog.UpdateStatus("Stopping selected services..");
        //        var stopResults = await ServiceControl.StopSelectedServicesAsync();  // This should now work
        //        int stoppedCount = stopResults.Count(kv => kv.Value);
        //        Dispatcher.Invoke(() => {
        //            dialog.UpdateStatus($"{stoppedCount} services stopped successfully.");
        //        });
        //    }
        //}

        private async void optimizebtn_Click(object sender, RoutedEventArgs e)
        {
            Opt_dialog dialog = new Opt_dialog();
            dialog.Show(); // Open the dialog non-modally to allow updates

            bool wifiEnabled = wifiBtControl.IsWifiEnabled;
            bool bluEnabled = wifiBtControl.IsBluetoothEnabled;

            await PerformWirelessOperations(wifiEnabled, bluEnabled, dialog);

            // Attempt to stop selected services regardless of which tab is active
            if (ServiceControl.Visibility == Visibility.Visible || ServiceControl.Visibility == Visibility.Collapsed)
            {
                dialog.UpdateStatus("Stopping selected services..");
                var stopResults = await ServiceControl.StopSelectedServicesAsync();
                int stoppedCount = stopResults.Count(kv => kv.Value);
                Dispatcher.Invoke(() => { dialog.UpdateStatus($"{stoppedCount} services stopped successfully."); });
            }

            // Perform system cleaning operations based on toggle states
            await PerformCleaningOperations(dialog);
            await PerformSysTweakOperations(dialog);
        }

        private async Task PerformCleaningOperations(Opt_dialog dialog)
        {
            sysCleanTw.ResetMemoryCleaned();

            if (Syscleancontrol.CleanSystemToggle.IsChecked ?? false)
            {
                sysCleanTw.CleanSystemTempFiles(true, true, true); // Example call

            }

            if (Syscleancontrol.Dx9Toggle.IsChecked ?? false)
            {
                sysCleanTw.CleanAmdGpuCache(true, false, false);
                dialog.UpdateStatus("DirectX 9 cache cleaned.");
            }

            if (Syscleancontrol.Dx11Toggle.IsChecked ?? false)
            {
                sysCleanTw.CleanAmdGpuCache(false, false, true);
                dialog.UpdateStatus("DirectX 11 cache cleaned.");
            }

            if (Syscleancontrol.Dx12Toggle.IsChecked ?? false)
            {
                sysCleanTw.CleanAmdGpuCache(false, true, false);
                dialog.UpdateStatus("DirectX 12 cache cleaned.");
            }

            string totalCleaned = sysCleanTw.GetFormattedTotalMemoryCleaned();
            Dispatcher.Invoke(() => { dialog.UpdateStatus($"Total memory cleaned: {totalCleaned}"); });

        }

        private async Task PerformSysTweakOperations(Opt_dialog dialog)
        {

            if (Syscleancontrol.ToggleCoreIso.IsChecked.HasValue && Syscleancontrol.ToggleCoreIso.IsChecked.Value)
            {
                sysCleanTw.ToggleCoreIsolation(true);
                dialog.UpdateStatus("Core isolation enabled.");
            }
            else
            {
                sysCleanTw.ToggleCoreIsolation(false);
                dialog.UpdateStatus("Core isolation disabled.");
            }


            if (Syscleancontrol.ToggleDNS.IsChecked ?? false)
            {
                sysCleanTw.FlushDns(true);
                dialog.UpdateStatus("DNS cache was reset.");
            }

            if (Syscleancontrol.ToggleWinsock.IsChecked ?? false)
            {
                sysCleanTw.ResetWinsock(true);
                dialog.UpdateStatus("WINSOCK was reset.");
            }
        }




        private async Task PerformWirelessOperations(bool wifiEnabled, bool bluEnabled, Opt_dialog dialog)
        {
            await Task.Run(() =>
            {
                wifiBtSwitch.ToggleWifi(wifiEnabled);
                Dispatcher.Invoke(() => { dialog.UpdateStatus("WiFi " + (wifiEnabled ? "enabled" : "disabled")); });
            });

            await Task.Run(() =>
            {
                wifiBtSwitch.ToggleBluetooth(bluEnabled);
                Dispatcher.Invoke(() => { dialog.UpdateStatus("Bluetooth " + (bluEnabled ? "enabled" : "disabled")); });
            });
        }
    }
}