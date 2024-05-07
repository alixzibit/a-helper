using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahelper.Helpers
{
    public class GPUInfo
    {
        public static string GetGpuInfo()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c pnputil /enum-devices /connected /class display",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            string output = "";
            using (Process process = Process.Start(psi))
            {
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }

            // Parse the output to find the device description
            var match = System.Text.RegularExpressions.Regex.Match(output, "Device Description:\\s*(.+)");
            if (match.Success)
            {
                return match.Groups[1].Value.Trim(); // Returns the device description, e.g., "AMD Radeon 780M"
            }

            return "GPU information not found.";
        }

        public static string GetDriverVersionAndAFMF()
        {
            string registryPath = @"SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}\0000";
            string driverVersion = "Driver version not found";
            string afmfStatus = "AFMF not detected"; // Assuming AFMF detection via DrvFrameGenEnabled

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
            {
                if (key != null)
                {
                    // Fetching the driver version
                    object versionValue = key.GetValue("DriverVersion");
                    if (versionValue != null)
                    {
                        driverVersion = versionValue.ToString();
                    }

                    // Checking for AFMF support
                    object afmfValue = key.GetValue("DrvFrameGenEnabled");
                    if (afmfValue != null)
                    {
                        afmfStatus ="AFMF detected";
                    }
                }
            }

            return $"{driverVersion}                      {afmfStatus}";
        }
    }
}
