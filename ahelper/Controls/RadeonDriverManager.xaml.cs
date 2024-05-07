using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using ahelper.Helpers;
using WindowsInput;
using WindowsInput.Native;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;
using Path = System.IO.Path;
using SharpDX.XInput;
using System.Management;
using System.Threading;



namespace ahelper.Controls
{
    /// <summary>
    /// Interaction logic for RadeonDriverManager.xaml
    /// </summary>


    public partial class RadeonDriverManager : UserControl
    {
        public RadeonDriverManager()
        {
            InitializeComponent();
            //MainWindow.GamepadButtonPressed += OnGamepadButtonPressed;
            this.Dispatcher.Invoke(() => { DisplayGpuInfo(); });

        }




        // TODO: use pnputil silently to backup AMD GPU driver for button  <Button x:Name="backupdrv"

        private void DisplayGpuInfo()
        {
            string gpuInfo = GPUInfo.GetGpuInfo();
            string driverAndAfmfInfo = GPUInfo.GetDriverVersionAndAFMF();
            gpuinfo.Content = $"{gpuInfo}        {driverAndAfmfInfo}";
        }

        //private void OnGamepadButtonPressed(object sender, GamepadButtonEventArgs e)
        //{
        //    if (e.Button == GamepadButtonFlags.Y)
        //    {
        //        Dispatcher.Invoke(() =>
        //        {
        //            // Cancel any ongoing operations
        //            CancelInstallation();
        //        });
        //    }
        //}



        private async void installdrv_Click(object sender, RoutedEventArgs e)
        {

            loading_ui.Visibility = Visibility.Visible;
            MessageBox.Show(
                messageBoxText:
                "Ensure to place the setup file in the application root driver_setup folder otherwise application will try to find any previous driver from C:\\AMD path",
                "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            var result = MessageBox.Show("Do you want to visit the AMD website (7840u) to download the latest driver?",
                "Download Driver", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Process.Start(
                    new ProcessStartInfo(
                            "https://www.amd.com/en/support/downloads/drivers.html/processors/ryzen/ryzen-7000-series/amd-ryzen-7-7840u.html")
                        { UseShellExecute = true });
                return;
            }

            if (result == MessageBoxResult.Cancel)
            {
                loading_ui.Visibility = Visibility.Hidden;
                return;
            }

            // Find the driver executable from app root
            string appRootPath = AppDomain.CurrentDomain.BaseDirectory;
            string driverFileName = Directory.EnumerateFiles(appRootPath, "driver_setup\\whql-amd-software-adrenalin*.exe")
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(driverFileName))
            {
                // Start silent installation
                Process installationProcess = Process.Start(driverFileName, "-INSTALL");
                Dispatcher.InvokeAsync(() => { statuslabel.Text = "Setup files are being extracted."; });
                await Task.Delay(300);
                installationProcess.WaitForExit();

                // Check if AMD folder is created
                if (Directory.Exists(@"C:\AMD\AMD-Software-Installer\Packages\Drivers\Display\WT6A_INF"))
                {
                    Dispatcher.InvokeAsync(() => { statuslabel.Text = "Installation files are in place."; });
                }
                else
                {
                    Dispatcher.InvokeAsync(() => { statuslabel.Text = "Installation did not proceed as expected."; });
                    return;
                }
            }
            else
            {

                if (Directory.Exists(@"C:\AMD\AMD-Software-Installer\Packages\Drivers\Display\WT6A_INF"))
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        statuslabel.Text =
                            "Driver executable not found, but C:\\AMD path contains driver - proceeding to install.";
                    });
                }
                else
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        statuslabel.Text = "Driver executable not found and no installation files present.";
                    });
                    return;
                }
            }

            txtOutput.Visibility = Visibility.Visible;
            InputSimulator sim1 = new InputSimulator();

            txtOutput.Focus();
            await Task.Delay(100);
            txtOutput.Text = @"C:\AMD\AMD-Software-Installer\Packages\Drivers\Display\WT6A_INF";
            await Task.Delay(100);
            sim1.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);

            await Task.Delay(100);

            sim1.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C);

            //PCI\\VEN_10DE&DEV_2204&SUBSYS_87B51043&REV_A1\\4&3292EEB7&0&0008\ 3090 dev pc
            // PCI\\VEN_1002&DEV_15BF&SUBSYS_17F31043&REV_04\\4&98C338A&0&0041\ z1x ally
            DriverInstallVerifier verifier = new DriverInstallVerifier();
           

            if (!await OpenDeviceProperties())
            {
                loading_ui.Visibility = Visibility.Hidden; // Hide the loading UI since processing stops
                txtOutput.Visibility = Visibility.Hidden;
                statuslabel.Text = "Driver installation stopped due to device not found";
                return; // Stop further execution because the device properties could not be opened
            }
            Dispatcher.InvokeAsync(() =>
            {
                statuslabel2.Text = "<== Automated UI interaction is in progress - PRESS (B) 2-3 times TO CANCEL";
            });
            txtOutput.Visibility = Visibility.Hidden;
            await Task.Delay(1000);
            UIAutomation.DevicePropertiesNavigator();
            await Task.Delay(500);
            UIAutomation.SelectandInstall780m();
            await Task.Delay(500);
            devPropProcess.WaitForExit();
            DateTime installTime = DateTime.UtcNow;
            string deviceId = "PCI\\VEN_1002&DEV_15BF&SUBSYS_17F31043&REV_04\\4&98C338A&0&0041";
            bool isInstallSuccessful = await verifier.CheckDriverInstallationAsync(deviceId, installTime);
            Dispatcher.Invoke(() =>
            {
                statuslabel.Text = isInstallSuccessful
                    ? "Driver installation successful."
                    : "Driver installation failed.";
                loading_ui.Visibility = Visibility.Hidden;
                statuslabel2.Text = "";
                DisplayGpuInfo();
            });
        }

        private Process devPropProcess;


        private async Task<bool> OpenDeviceProperties()
        {
            string deviceId = "PCI\\VEN_1002&DEV_15BF&SUBSYS_17F31043&REV_04\\4&98C338A&0&0041";

            bool isPresent = await IsDeviceIdPresentAsync(deviceId);
            if (!isPresent)
            {
                MessageBox.Show("No compatible AMD Z1x GPU Device ID found.", "Device Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                Process devPropProcess = Process.Start("rundll32.exe", $"devmgr.dll,DeviceProperties_RunDLL /DeviceID \"{deviceId}\"");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open device properties: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }



        private async Task<bool> IsDeviceIdPresentAsync(string deviceId)
        {
            string command = "/c pnputil /enum-devices /connected /class Display";
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                await process.WaitForExitAsync();
                string output = await process.StandardOutput.ReadToEndAsync();
                return CheckDeviceIdInOutput(output, deviceId);
            }
        }

        private bool CheckDeviceIdInOutput(string output, string targetDeviceId)
        {
            var lines = output.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            bool deviceFound = false;

            foreach (string line in lines)
            {
                if (line.StartsWith("Instance ID:", StringComparison.OrdinalIgnoreCase))
                {
            
                    string instanceId = line.Substring("Instance ID:".Length).Trim();
                    if (string.Equals(instanceId, targetDeviceId, StringComparison.OrdinalIgnoreCase))
                    {
                        deviceFound = true;
                        break;
                    }
                }
            }

            return deviceFound;
        }



        private void KillDevicePropertiesProcess()
        {
         
            var rundllProcesses = Process.GetProcessesByName("rundll32");
            foreach (var process in rundllProcesses)
            {
                try
                {
                    if (process.MainModule.FileName.EndsWith("rundll32.exe") &&
                        process.GetCommandLine().Contains("DeviceProperties_RunDLL"))
                    {
                        process.Kill(); 
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error killing process: {ex.Message}");
                }
            }

            Dispatcher.Invoke(() => statuslabel.Text = "Device installation was cancelled by user.");
        }

        public void CancelInstallation()
        {
            KillDevicePropertiesProcess();
            statuslabel.Text = "Installation has been cancelled by the user.";
            loading_ui.Visibility = Visibility.Hidden;
        }

        private async Task BackupDriverAsync()
        {
            string instanceId = "PCI\\VEN_1002&DEV_15BF&SUBSYS_17F31043&REV_04\\4&98C338A&0&0041";
            string backupRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "driver_backup", DateTime.Today.ToString("yyyyMMdd"));
            Directory.CreateDirectory(backupRoot); 

            string driverName = await GetDriverNameAsync(instanceId);
            if (string.IsNullOrEmpty(driverName))
            {
                MessageBox.Show("Driver not found.", "Backup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string formattedPath = $"\"{backupRoot}\"";
            string backupCommand = $"/export-driver {driverName} {formattedPath}";
            await RunPnputilCommand(backupCommand, "Driver backup completed successfully.");
        }

        private async Task<string> GetDriverNameAsync(string instanceId)
        {
            ProcessStartInfo psi = new ProcessStartInfo("pnputil.exe", "/enum-devices /connected /class Display")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                await process.WaitForExitAsync();
                string output = await process.StandardOutput.ReadToEndAsync();
                return ParseDriverName(output, instanceId);
            }
        }

        private string ParseDriverName(string pnputilOutput, string instanceId)
        {
            var lines = pnputilOutput.Split(new[] { "\r\n", "\r" }, StringSplitOptions.None); // Split lines correctly handling different newline conventions
            bool isRelevantDevice = false;

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim(); 

                
                if (line.StartsWith("Instance ID:", StringComparison.OrdinalIgnoreCase))
                {
                    string currentId = line.Substring("Instance ID:".Length).Trim(); 
                    
                    var normalizedCurrentId = new string(currentId.Where(char.IsLetterOrDigit).ToArray());
                    var normalizedInstanceId = new string(instanceId.Where(char.IsLetterOrDigit).ToArray());

                    if (normalizedCurrentId.Equals(normalizedInstanceId, StringComparison.OrdinalIgnoreCase))
                    {
                        isRelevantDevice = true; 
                    }
                    else
                    {
                        isRelevantDevice = false; 
                    }
                }
                else if (isRelevantDevice && line.StartsWith("Driver Name:", StringComparison.OrdinalIgnoreCase))
                {
                    return line.Substring("Driver Name:".Length).Trim(); 
                }
            }

            return null; 
        }


        private async Task RunPnputilCommand(string arguments, string successMessage)
        {
            ProcessStartInfo psi = new ProcessStartInfo("pnputil.exe", arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                await process.WaitForExitAsync();

                // Optionally read output and errors
                string output = await process.StandardOutput.ReadToEndAsync();
                string errors = await process.StandardError.ReadToEndAsync();

                if (process.ExitCode == 0)
                {
                    MessageBox.Show(successMessage, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to backup driver. Error: {errors}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void backupdrv_Click(object sender, RoutedEventArgs e)
        {
            BackupDriverAsync();

        }

        private void restoredrv_Click(object sender, RoutedEventArgs e)
        {
            
            MessageBox.Show("This feature is not yet implemented.", "Feature Not Implemented", MessageBoxButton.OK, MessageBoxImage.Information);
           
        }
    }

}
