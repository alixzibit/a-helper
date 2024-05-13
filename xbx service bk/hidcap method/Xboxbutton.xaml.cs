using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HidSharp;

namespace ahelper.Controls
{
    public partial class XboxButton : UserControl
    {
        private DispatcherTimer timer;
        private bool isGameBarOpen = false;
        private HidDeviceLoader loader = new HidDeviceLoader();
        private HidStream hidStream;
        private CancellationTokenSource cancellationTokenSource;

        private const byte VK_LWIN = 0x5B;
        private const byte VK_G = 0x47;
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;
        private const byte VK_SHIFT = 0x10;
        private const byte VK_TAB = 0x09;

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public XboxButton()
        {
            InitializeComponent();
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(160) };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (ToggleOverride.IsChecked == true)
            {
                StartMonitoringDeviceAsync();
            }
        }

        //private async void StopServiceAndMonitorDeviceAsync()
        //{
        //    UpdateStatusLabel("Attempting to stop service...");
        //    bool serviceStopped = await Task.Run(() => StopService("ArmouryCrateSEService"));
        //    if (!serviceStopped)
        //    {
        //        UpdateStatusLabel("Failed to stop service.");
        //        return;
        //    }

        //    UpdateStatusLabel("Service stopped. Starting device monitoring...");
        //    bool monitoringStarted = await Task.Run(() => MonitorDevice());
        //    if (!monitoringStarted)
        //    {
        //        UpdateStatusLabel("Failed to start monitoring.");
        //        return;
        //    }

        //    UpdateStatusLabel("Monitoring started successfully.");
        //}

        private bool StopService(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);
                if (service.Status != ServiceControllerStatus.Stopped)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(5000));
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping service: {ex.Message}");
            }
            return false;
        }

        private void ListDevices()
        {
            try
            {
                var deviceList = DeviceList.Local;
                var hidDeviceList = deviceList.GetHidDevices()
                    .Select(d => {
                        try
                        {
                            return new DeviceInfo
                            {
                                FriendlyName = d.GetFriendlyName(),
                                DevicePath = d.DevicePath
                            };
                        }
                        catch (Exception ex)
                        {
                            return new DeviceInfo
                            {
                                FriendlyName = "[Unknown Device - Access Error]",
                                DevicePath = d.DevicePath
                            };
                        }
                    })
                    .ToList();

                //listBox.ItemsSource = hidDeviceList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to list devices: {ex.Message}");
            }
        }


        private async Task<bool> StartMonitoringDeviceAsync()
        {
            // Device path as formatted in ListDevices and selected in startcap_Click
            string targetDevicePath = @"\\?\hid#vid_0b05&pid_1abe&mi_02&col01#8&d1252f4&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}";

            try
            {
                // Display available devices for debugging
                var deviceList = DeviceList.Local.GetHidDevices().ToList();
                foreach (var dev in deviceList)
                    try
                    {
                        Debug.WriteLine($"{dev.GetFriendlyName()} - {dev.DevicePath}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error accessing device info: {ex.Message}");
                    }


                // Using the exact device path to find and open the device
                var device = deviceList.FirstOrDefault(d => d.DevicePath == targetDevicePath);
                if (device == null)
                {
                    UpdateStatusLabel("Device not found.");
                    return false;
                }

                try
                {
                    if (device.TryOpen(out HidStream stream))
                    {
                        // Successfully opened the device
                        bool readSuccess = await ReadFromDeviceAsync(device, stream);
                        stream.Close();
                        return readSuccess;
                    }
                    else
                    {
                        UpdateStatusLabel("Failed to open device. It may be in use or disconnected.");
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatusLabel($"Failed to open device: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusLabel($"Error accessing device list: {ex.Message}");
            }
            return false;
        }


        private async Task<bool> ReadFromDeviceAsync(HidDevice device, HidStream stream)
        {
            byte[] readBuffer = new byte[device.GetMaxInputReportLength()];
            while (true) // Continuous monitoring loop
            {
                try
                {
                    // Wait for data with a specified timeout, handling timeouts gracefully
                    Task<int> readTask = stream.ReadAsync(readBuffer, 0, readBuffer.Length);
                    
                    int bytesRead = await readTask; // This could throw if timeout occurs

                    if (bytesRead > 0)
                    {
                        // Process the read data
                        if (readBuffer[1] == 0x38)  // Check specific condition for button press
                        {
                            HandleOverlayToggleAsync();
                            continue; // Continue listening even after handling action
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No data read, device may be disconnected.");
                        continue; // Continue listening
                    }
                }
                catch (SystemException ex)
                {
                    Debug.WriteLine($"Read operation timed out, continuing to monitor...: {ex.Message}");
                    continue; // Continue monitoring in case of timeout
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error reading from device: {ex.Message}");
                    break; // Break on other exceptions
                }
            }

            stream.Close(); // Make sure to close the stream outside the loop
            return false;  // Return false indicating monitoring ended due to an error
        }



        private bool RestartService(string serviceName)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30)); // Use a reasonable timeout
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error restarting service: {ex.Message}");
                return false;
            }
            return false; // Return false if the service is not in a stopped state or other issues
        }

        //private void HandleOverlayToggle()
        //{
        //    // Existing logic to toggle overlays
        //    if (ToggleOverride_Steam.IsChecked ?? false)
        //    {
        //        SimulateShiftTab();
        //    }
        //    else
        //    {
        //        SimulateWinG();
        //    }
        //    UpdateStatusLabel(isGameBarOpen ? "Overlay opened." : "Overlay closed.");
        //}

        private async void HandleOverlayToggleAsync()
        {
            await Task.Run(() => // Run the key simulation in a background thread
            {
                if (ToggleOverride_Steam.IsChecked ?? false)
                {
                    SimulateShiftTab();
                }
                else
                {
                    SimulateWinG();
                }
            });

            // Ensure that the status update is performed on the UI thread
            Dispatcher.Invoke(() =>
            {
                UpdateStatusLabel(isGameBarOpen ? "Overlay opened." : "Overlay closed.");
            });
        }


        private void SimulateWinG()
        {
            keybd_event(VK_LWIN, 0, KEYEVENTF_EXTENDEDKEY, 0);
            Thread.Sleep(100); 
            keybd_event(VK_G, 0, KEYEVENTF_EXTENDEDKEY, 0);
            Thread.Sleep(50);
            keybd_event(VK_G, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, 0);
        }

        private void SimulateShiftTab()
        {
            keybd_event(VK_SHIFT, 0, KEYEVENTF_EXTENDEDKEY, 0);  
            Thread.Sleep(100);
            keybd_event(VK_TAB, 0, KEYEVENTF_EXTENDEDKEY, 0);    
            Thread.Sleep(50); 
            keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);          
            keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);         
        }

        private void UpdateStatusLabel(string message)
        {
            statuslabel.Text = message;
        }

        private async void ToggleOverride_Checked_1(object sender, RoutedEventArgs e)
        {
            LoadingUi.Visibility = Visibility.Visible; // Show loading GIF
            UpdateStatusLabel("Attempting to stop service and start monitoring...");

            bool serviceStopped = await Task.Run(() => StopService("ArmouryCrateSEService"));
            if (!serviceStopped)
            {
                UpdateStatusLabel("Failed to stop service.");
                LoadingUi.Visibility = Visibility.Hidden; // Hide loading GIF
                ToggleOverride.IsChecked = false; // Reset toggle if service stop fails
                return;
            }

            bool monitoringStarted = await Task.Run(() => StartMonitoringDeviceAsync());
            if (!monitoringStarted)
            {
                UpdateStatusLabel("Failed to start monitoring.");
                LoadingUi.Visibility = Visibility.Hidden; // Hide loading GIF
                ToggleOverride.IsChecked = false; // Reset toggle if monitoring fails
                return;
            }

            UpdateStatusLabel("Monitoring started.");
            LoadingUi.Visibility = Visibility.Hidden; // Hide loading GIF
            ToggleOverride_Steam.IsEnabled = true;
            steamoverlaylabel.Visibility = Visibility.Visible;
            ToggleOverride_Steam.Visibility = Visibility.Visible;
            timer.Start();
        }


        private async void ToggleOverride_Unchecked_1(object sender, RoutedEventArgs e)
        {
            LoadingUi.Visibility = Visibility.Visible;
            timer.Stop();
            UpdateStatusLabel("Monitoring stopped.");
            ToggleOverride_Steam.IsEnabled = false;
            ToggleOverride_Steam.IsChecked = false;
            steamoverlaylabel.Visibility = Visibility.Hidden;
            ToggleOverride_Steam.Visibility = Visibility.Hidden;

            // Restart the service asynchronously to avoid UI freeze
            bool restartSuccessful = await Task.Run(() => RestartService("ArmouryCrateSEService"));
            if (restartSuccessful)
            {
                UpdateStatusLabel("Service restarted successfully.");
                LoadingUi.Visibility = Visibility.Hidden;
            }
            else
            {
                UpdateStatusLabel("Failed to restart service.");
                LoadingUi.Visibility = Visibility.Hidden;
            }
        }


        private void ToggleOverrideSteam_Checked_1(object sender, RoutedEventArgs e)
        {
            UpdateStatusLabel("Steam overlay toggle enabled.");
        }

        private void ToggleOverrideSteam_Unchecked_1(object sender, RoutedEventArgs e)
        {
            
            UpdateStatusLabel("Steam overlay toggle disabled.");
        }

        public class DeviceInfo
        {
            public string FriendlyName { get; set; }
            public string DevicePath { get; set; }

            public override string ToString()
            {
                return $"{FriendlyName} - {DevicePath}";
            }
        }

    }
}
