using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ahelper.Helpers
{
    public class SysCleanTw
    {
        // Constructor
        private Dictionary<string, long> memoryCleaned;
        public SysCleanTw()
        {
            memoryCleaned = new Dictionary<string, long>();
        }


        public void ResetMemoryCleaned()
        {
            memoryCleaned.Clear();
        }

       
        public string GetFormattedTotalMemoryCleaned()
        {
            long totalBytes = memoryCleaned.Values.Sum();
            double totalMegabytes = totalBytes / 1024.0 / 1024.0;  // Convert bytes to megabytes
            if (totalMegabytes > 1024)
            {
                double totalGigabytes = totalMegabytes / 1024.0;  // Convert megabytes to gigabytes
                return $"{totalGigabytes:F2} GB";
            }
            else
            {
                return $"{totalMegabytes:F2} MB";
            }
        }


        public void CleanAmdGpuCache(bool cleanDx9, bool cleanDx12, bool cleanDx11)
        {
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD");

            if (cleanDx9)
            {
                DeleteFilesInDirectory(Path.Combine(basePath, "DX9Cache"));
            }
            if (cleanDx12)
            {
                DeleteFilesInDirectory(Path.Combine(basePath, "DxcCache"));
            }
            if (cleanDx11)
            {
                DeleteFilesInDirectory(Path.Combine(basePath, "DxCache"));
            }
        }


        public void CleanSystemTempFiles(bool cleanPrefetch, bool cleanLocalTemp, bool cleanWindowsTemp)
        {
            var tempPath = Path.GetTempPath();
            DeleteFilesInDirectory(tempPath);

            if (cleanLocalTemp)
            {
                var localTempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
                DeleteFilesInDirectory(localTempPath);
            }

            if (cleanWindowsTemp)
            {
                var windowsTempPath = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "Temp");
                DeleteFilesInDirectory(windowsTempPath);
            }

            if (cleanPrefetch)
            {
                var prefetchPath = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "Prefetch");
                DeleteFilesInDirectory(prefetchPath);
            }
        }


        public void PurgeSfcCache()
        {
            RunCommand("sfc", "/purgecache");
        }


        public void ResetWinsock(bool b)
        {
            RunCommand("netsh", "winsock reset");
        }


        public void FlushDns(bool b)
        {
            RunCommand("ipconfig", "/flushdns");
        }

        public void ToggleCoreIsolation(bool enable)
        {
            int value = enable ? 1 : 0;  
            string powershellCommand = $"Set-ItemProperty -Path 'HKLM:\\SYSTEM\\CurrentControlSet\\Control\\DeviceGuard\\Scenarios\\HypervisorEnforcedCodeIntegrity' -Name 'Enabled' -Value {value}";

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{powershellCommand}\"",
                    Verb = "runas", 
                    UseShellExecute = true
                };

                Process process = new Process() { StartInfo = startInfo };
                process.Start();
                process.WaitForExit(); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to {(enable ? "enable" : "disable")} Core Isolation: " + ex.Message);
            }
        }

        public bool CheckCoreIsolationEnabled()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\DeviceGuard\\Scenarios\\HypervisorEnforcedCodeIntegrity"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("Enabled");
                        if (value is int intValue)
                        {
                            return intValue == 1;  // Memory Integrity is enabled if the value is 1
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error checking Core Isolation status: " + ex.Message);
            }

            return false;  // Default to disabled if there is an error or the key/value is not found
        }

        private void DeleteFilesInDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                // Deleting files
                foreach (var file in Directory.GetFiles(directoryPath))
                {
                    long fileSize = 0;
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        fileSize = fileInfo.Length; 
                        if (fileInfo.Attributes.HasFlag(FileAttributes.ReadOnly) || fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                        {
                            fileInfo.Attributes = FileAttributes.Normal; 
                        }
                        File.Delete(file);
                        AddToMemoryCleaned(fileSize);  
                        Debug.WriteLine($"Deleted {file}: {fileSize} bytes");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to delete {file}: {ex.Message}. Size: {fileSize} bytes not added.");
                    }
                }

             
                foreach (var dir in Directory.GetDirectories(directoryPath))
                {
                    try
                    {
                        DeleteFilesInDirectory(dir);  
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to access directory {dir}: {ex.Message}");
                    }
                }

                try
                {
                    
                    if (Directory.GetFiles(directoryPath).Length == 0 && Directory.GetDirectories(directoryPath).Length == 0)
                    {
                        Directory.Delete(directoryPath);
                        Debug.WriteLine($"Deleted directory {directoryPath}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to delete directory {directoryPath}: {ex.Message}");
                }
            }
        }


        private void AddToMemoryCleaned(long size)
        {
            if (memoryCleaned.ContainsKey("Total"))
            {
                memoryCleaned["Total"] += size;
            }
            else
            {
                memoryCleaned["Total"] = size;
            }
        }

        // General method to run a command
        private void RunCommand(string command, string arguments)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(command, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = Process.Start(processStartInfo))
            {
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(errors))
                {
                    Debug.WriteLine($"Error: {errors}");
                }
            }
        }
    }
}