using System.Diagnostics;
using System.Management;

namespace ahelper.Helpers
{
    public static class commandutil
    {
        public static string GetCommandLine(this Process process)
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                       $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["CommandLine"]?.ToString() ?? "";
                }
            }
            return string.Empty;
        }
    }
}
