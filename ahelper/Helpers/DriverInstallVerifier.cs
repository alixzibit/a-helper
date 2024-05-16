using System.Management;
using System.Diagnostics;
using System.IO;

namespace ahelper.Helpers
{
    public class DriverInstallVerifier
    {

        public async Task<bool> CheckDriverInstallationAsync(string deviceId, DateTime installTime)
        {
            // Ensure installTime is in UTC
            installTime = installTime.ToUniversalTime();

            // Correct the time format and query syntax for WMI
            string startTime = installTime.AddSeconds(-60).ToString("yyyyMMddHHmmss.fffffff");
            string endTime = installTime.AddSeconds(60).ToString("yyyyMMddHHmmss.fffffff");

            // Define the queries with precise time formatting
            string successQuery = $"SELECT * FROM Win32_NTLogEvent WHERE Logfile = 'System' " +
                                  $"AND SourceName = 'Microsoft-Windows-UserPnP' " +
                                  $"AND EventCode = '20003' " +
                                  $"AND TimeGenerated >= '{startTime}' " +
                                  $"AND TimeGenerated <= '{endTime}' " +
                                  $"AND Message LIKE '%{deviceId.Replace("\\", "\\\\")}%'";

            string errorQuery = $"SELECT * FROM Win32_NTLogEvent WHERE Logfile = 'System' " +
                                $"AND SourceName = 'Microsoft-Windows-Kernel-PnP' " +
                                $"AND EventCode = '411' " +
                                $"AND TimeGenerated >= '{startTime}' " +
                                $"AND TimeGenerated <= '{endTime}' " +
                                $"AND Message LIKE '%{deviceId.Replace("\\", "\\\\")}%'";

            bool success = false;
            bool errorOccurred = false;

            // Check for a successful install event
            using (ManagementObjectSearcher successSearcher = new ManagementObjectSearcher(successQuery))
            {
                success = successSearcher.Get().Count > 0;
            }

            // Check for an error event
            using (ManagementObjectSearcher errorSearcher = new ManagementObjectSearcher(errorQuery))
            {
                errorOccurred = errorSearcher.Get().Count > 0;
            }

            // Return true if there was a success event and no error events
            return success && !errorOccurred;
        }



        public bool CheckDriverInstallationSuccess()
        {
            string appRoot = AppDomain.CurrentDomain.BaseDirectory;
            string sysExpPath = Path.Combine(appRoot, "sysexp.exe");
            string output2Path = Path.Combine(appRoot, "deviceproperties.csv");
            ;
            // Setup the SysExporter process to capture the output from the Static class
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = sysExpPath,
                Arguments = $"/Process rundll32.exe /class \"Static\" /Visible Yes /scomma \"{output2Path}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process sysExpProcess = new Process() { StartInfo = startInfo };
            sysExpProcess.Start();
            sysExpProcess.WaitForExit();

            // Read the output file to check for the installation success message
            if (File.Exists(output2Path))
            {
                string content = File.ReadAllText(output2Path);
                return content.Contains("Windows has finished installing the drivers for this device");
            }

            return false;
        }

    }
}
