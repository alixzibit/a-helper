using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;

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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_MINIMIZE = 6;


        public XboxButton()
        {
            InitializeComponent();
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(160)  
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
                    
                    IntPtr hWnd = FindWindow(null, "Armoury Crate"); 
                    if (hWnd != IntPtr.Zero)
                    {
                        ShowWindow(hWnd, SW_MINIMIZE);
                    }

                    process.Kill(); 
                    process.WaitForExit(); 
                    HandleOverlayToggle();
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
