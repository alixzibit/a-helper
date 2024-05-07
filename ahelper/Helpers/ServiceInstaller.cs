//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;

//namespace ahelper.Helpers
//{
//    public class ServiceInstaller
//    {
//        private static string appRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
//        private static string serviceInstallFolder = Path.Combine(appRoot, "serviceInstall");
//        private static string installUtilPath = Path.Combine(serviceInstallFolder, "InstallUtil.exe");
//        private static string servicePath = Path.Combine(serviceInstallFolder, "ACKeyRedirector.exe");

//        public static void InstallService()
//        {
//            RunInstallUtil("/i"); // Install the service
//        }

//        public static void UninstallService()
//        {
//            RunInstallUtil("/u"); // Uninstall the service
//        }

//        private static void RunInstallUtil(string argument)
//        {
//            try
//            {
//                ProcessStartInfo processStartInfo = new ProcessStartInfo
//                {
//                    FileName = installUtilPath,
//                    Arguments = $"{argument} \"{servicePath}\"",
//                    Verb = "runas", // Run as administrator
//                    UseShellExecute = false
//                };

//                Process process = Process.Start(processStartInfo);
//                process.WaitForExit();

//                MessageBox.Show("Service operation completed.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Failed to execute service operation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }
//    }
//}
