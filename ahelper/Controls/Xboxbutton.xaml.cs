using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ahelper.Controls
{
    public partial class XboxButton : UserControl
    {
        private DispatcherTimer timer;
        private bool isGameBarOpen = false; 

        private const byte VK_LWIN = 0x5B;
        private const byte VK_G = 0x47;
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;
        private const byte VK_SHIFT = 0x10;
        private const byte VK_TAB = 0x09;

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);



        public XboxButton()
        {
            InitializeComponent();
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)  
            };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (ToggleOverride.IsChecked == true)
            {
                MonitorAndHandleProcess();
            }
        }

        private void MonitorAndHandleProcess()
        {
            var processes = Process.GetProcessesByName("ArmouryCrateSE");
            if (!processes.Any())
            {
                UpdateStatusLabel("Armoury Crate is not running.");
                return;
            }

            foreach (var process in processes)
            {
                try
                {
                    // Directly attempt to kill the process without trying to find and minimize its window
                    process.Kill();
                    //Process.Start(new ProcessStartInfo
                    //{
                    //    FileName = "cmd.exe",
                    //    Arguments = "/c taskkill /f /im ArmouryCrateSE.exe",
                    //    CreateNoWindow = true,
                    //    UseShellExecute = false
                    //});

                    process.WaitForExit(); // Ensures that the process has finished exiting
                    HandleOverlayToggle(); // Proceed to handle the overlay toggle after the process is closed
                }
                catch (Exception ex)
                {
                    UpdateStatusLabel($"Failed to handle process: {ex.Message}");
                }
            }
        }



        private void HandleOverlayToggle()
        {
            if (ToggleOverride_Steam.IsChecked ?? false)
            {
                if (!isGameBarOpen)
                {
                    SimulateShiftTab(); 
                    isGameBarOpen = true;
                }
                else
                {
                    isGameBarOpen = false;
                }
            }
            else
            {
                if (!isGameBarOpen)
                {
                    SimulateWinG(); 
                    isGameBarOpen = true;
                }
                else
                {
                    isGameBarOpen = false;
                }
            }
            UpdateStatusLabel(isGameBarOpen ? "Overlay opened." : "Overlay closed.");
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

        private void ToggleOverride_Checked_1(object sender, RoutedEventArgs e)
        {
            timer.Start();
            UpdateStatusLabel("Monitoring started.");
            ToggleOverride_Steam.IsEnabled = true;
            steamoverlaylabel.Visibility=Visibility.Visible;
            ToggleOverride_Steam.Visibility = Visibility.Visible;
        }

        private void ToggleOverride_Unchecked_1(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            UpdateStatusLabel("Monitoring stopped.");
            ToggleOverride_Steam.IsEnabled = false;
            ToggleOverride_Steam.IsChecked = false;
            steamoverlaylabel.Visibility = Visibility.Hidden;
            ToggleOverride_Steam.Visibility = Visibility.Hidden;
        }

        private void ToggleOverrideSteam_Checked_1(object sender, RoutedEventArgs e)
        {
            UpdateStatusLabel("Steam overlay toggle enabled.");
        }

        private void ToggleOverrideSteam_Unchecked_1(object sender, RoutedEventArgs e)
        {
            
            UpdateStatusLabel("Steam overlay toggle disabled.");
        }


    }
}
