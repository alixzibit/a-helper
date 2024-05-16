using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;

namespace ahelper.Helpers
{
    public class WifiBtSwitch
    {

        public bool IsWifiEnabled()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId='Wi-Fi'");
                foreach (ManagementObject obj in searcher.Get())
                {
                    // NetEnabled returns true if the network adapter is enabled
                    return (bool)(obj["NetEnabled"] ?? false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error checking WiFi status: " + ex.Message);
            }
            return false;
        }

        public bool IsBluetoothEnabled()
        {
            bool isBluetoothEnabled = false;  // Default to false unless a device with Status "OK" is found
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%Bluetooth%'");
                foreach (ManagementObject obj in searcher.Get())
                {
                    var status = obj["Status"]?.ToString();
                    if (status == "OK")
                    {
                        isBluetoothEnabled = true;  // Set to true if any Bluetooth device has Status "OK"
                        break;  // Stop searching once an enabled device is found
                    }
                    else if (status == "Error")
                    {
                        continue;  // Skip and check the next device
                    }
                    // If status is neither "OK" nor "Error", assume disabled and continue checking
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error checking Bluetooth status: " + ex.Message);
            }
            return isBluetoothEnabled;
        }
    

    public void ToggleWifi(bool enable)
        {
            try
            {
                string methodName = enable ? "Enable" : "Disable";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId='Wi-Fi'");
                foreach (ManagementObject obj in searcher.Get())
                {
                    obj.InvokeMethod(methodName, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error toggling WiFi: " + ex.Message);
            }
        }

        public void ToggleBluetooth(bool enable)
        {
            bool operationSuccessful = false;
            try
            {
                string methodName = enable ? "Enable" : "Disable";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%Bluetooth%' AND DeviceID LIKE 'USB\\%'");

                foreach (ManagementObject obj in searcher.Get())
                {
                    obj.InvokeMethod(methodName, null);
                    operationSuccessful = true;  // Assume success if no exceptions are thrown
                    Debug.WriteLine($"Bluetooth {methodName} command successfully issued to {obj["Name"]}.");
                }

                if (!operationSuccessful)
                {
                    Debug.WriteLine("No USB Bluetooth devices found or operation unsuccessful, attempting pnputil fallback.");
                    ApplyPnputil(enable);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error toggling Bluetooth via WMI: " + ex.Message);
                ApplyPnputil(enable);  // Fallback to pnputil if WMI operation fails
            }
        }


        private void ApplyPnputil(bool enable)
        {
            string output = RunPnputilCommand("/enum-devices /connected /class Bluetooth");
            string instanceId = ParseInstanceId(output);
            if (!string.IsNullOrEmpty(instanceId))
            {
                string command = enable ? "/enable-device " : "/disable-device ";
                RunPnputilCommand(command + instanceId);
            }
        }

        private string RunPnputilCommand(string arguments)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "pnputil.exe";
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error running pnputil command: " + ex.Message);
                return string.Empty;
            }
        }

        private string ParseInstanceId(string pnputilOutput)
        {
            Regex regex = new Regex(@"Instance ID:\s+(.+)\r?\n", RegexOptions.IgnoreCase);
            Match match = regex.Match(pnputilOutput);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            return string.Empty;
        }

        public string CheckBluetoothStatus()
        {
            string output = RunPnputilCommand("/enum-devices /connected /class Bluetooth");
            return ParseBluetoothStatus(output);
        }

        private string ParseBluetoothStatus(string pnputilOutput)
        {
            Regex regex = new Regex(@"Status:\s+(.+)\r?\n", RegexOptions.IgnoreCase);
            Match match = regex.Match(pnputilOutput);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
            return "Unknown";
        }
    }
}
