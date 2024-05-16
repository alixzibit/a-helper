using WindowsInput.Native;
using WindowsInput;
using System.Diagnostics;
using System.IO;

namespace ahelper.Helpers
{
    public class UIAutomation
    {

        private static void CheckRundll32Process()
        {
            // Ensure that the rundll32 process is still running, otherwise throw an exception
            if (!Process.GetProcessesByName("rundll32")
                    .Any(p => p.MainModule.FileName.EndsWith("rundll32.exe") && p.GetCommandLine().Contains("DeviceProperties_RunDLL")))
            {
                throw new InvalidOperationException("rundll32 process is not running.");
            }
        }

        //public static void DevicePropertiesNavigator()  // this is the original method (simpler to interpret) which is replaced by the below method
        //{

        //    InputSimulator sim = new InputSimulator();
        //    CheckRundll32Process();
        //    sim.Keyboard.Sleep(300)

        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.RIGHT)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.SPACE)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.SPACE)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.SPACE)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.SPACE)
        //        .Sleep(300);
        //    sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.SPACE)
        //        .Sleep(1000)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300)
        //        .KeyPress(VirtualKeyCode.TAB)
        //        .Sleep(300);
        //    sim.Keyboard.KeyPress(VirtualKeyCode.HOME)
        //        .KeyPress(VirtualKeyCode.DOWN)
        //        .Sleep(300);
        //    sim.Keyboard.KeyPress(VirtualKeyCode.HOME)
        //        .KeyPress(VirtualKeyCode.DOWN)
        //        .Sleep(300);
        //    sim.Keyboard.KeyPress(VirtualKeyCode.HOME)
        //        .Sleep(300);
        //}

        public static void DevicePropertiesNavigator()
        {
            InputSimulator sim = new InputSimulator();

            // Helper function to check the process and throw if not running
            void EnsureRundll32Process()
            {
                if (!Process.GetProcessesByName("rundll32")
                    .Any(p => p.MainModule.FileName.EndsWith("rundll32.exe") && p.GetCommandLine().Contains("DeviceProperties_RunDLL")))
                {
                    throw new InvalidOperationException("rundll32 process is not running.");
                }
            }

            try
            {
                // Initial check before starting the sequence
                EnsureRundll32Process();

                // Execute the series of commands with checks before each critical action
                sim.Keyboard.Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.RIGHT)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300);

                EnsureRundll32Process();  // Check before a critical action

                sim.Keyboard.KeyPress(VirtualKeyCode.SPACE)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300);
                EnsureRundll32Process();
                sim.Keyboard.KeyPress(VirtualKeyCode.SPACE)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300);
                EnsureRundll32Process();
                sim.Keyboard.KeyPress(VirtualKeyCode.SPACE)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300);

                EnsureRundll32Process();  // Another critical check

                sim.Keyboard.KeyPress(VirtualKeyCode.SPACE)
                    .Sleep(300)
                    .ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300);
                EnsureRundll32Process();
                sim.Keyboard.KeyPress(VirtualKeyCode.SPACE)
                    .Sleep(1000)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.TAB)
                    .Sleep(300);

                EnsureRundll32Process();  // Final check before completion

                sim.Keyboard.KeyPress(VirtualKeyCode.HOME)
                    .KeyPress(VirtualKeyCode.DOWN)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.HOME)
                    .KeyPress(VirtualKeyCode.DOWN)
                    .Sleep(300)
                    .KeyPress(VirtualKeyCode.HOME)
                    .Sleep(300);
            }
            catch (InvalidOperationException ex)
            {
               
                Debug.WriteLine(ex.Message);
            }
        }

        //need to test and adjust the below as it does not perform the same as the original method
        //public static void DevicePropertiesNavigator()
        //{
        //    InputSimulator sim = new InputSimulator();
        //    List<VirtualKeyCode> keySequence = new List<VirtualKeyCode>
        //    {
        //        VirtualKeyCode.TAB, VirtualKeyCode.TAB, VirtualKeyCode.TAB, VirtualKeyCode.RIGHT,
        //        VirtualKeyCode.TAB, VirtualKeyCode.TAB, VirtualKeyCode.SPACE, VirtualKeyCode.TAB,
        //        VirtualKeyCode.SPACE, VirtualKeyCode.TAB, VirtualKeyCode.TAB, VirtualKeyCode.TAB,
        //        VirtualKeyCode.SPACE, VirtualKeyCode.TAB, VirtualKeyCode.TAB, VirtualKeyCode.SPACE,
        //        VirtualKeyCode.TAB, VirtualKeyCode.TAB, VirtualKeyCode.SPACE
        //    };

        //    foreach (var key in keySequence)
        //    {
        //        CheckRundll32Process();

        //        sim.Keyboard.Sleep(300).KeyPress(key);
        //    }

        //    // Handle the paste operation separately
        //    PerformPasteOperation(sim);
        //    PerformNavigationToSpecificItem(sim);
        //}

        //private static void PerformNavigationToSpecificItem(InputSimulator sim)
        //{
        //    CheckRundll32Process();

        //    // Navigate using HOME key followed by DOWN key presses
        //    sim.Keyboard.Sleep(300).KeyPress(VirtualKeyCode.HOME);
        //    sim.Keyboard.Sleep(300).KeyPress(VirtualKeyCode.DOWN); // First DOWN
        //    sim.Keyboard.Sleep(300).KeyPress(VirtualKeyCode.HOME);
        //    sim.Keyboard.Sleep(300).KeyPress(VirtualKeyCode.DOWN); // Second DOWN
        //    sim.Keyboard.Sleep(300).KeyPress(VirtualKeyCode.HOME);
        //}


        //private static void PerformPasteOperation(InputSimulator sim)
        //{
        //    CheckRundll32Process();

        //    sim.Keyboard.Sleep(300)
        //        .KeyDown(VirtualKeyCode.CONTROL)
        //        .KeyPress(VirtualKeyCode.VK_V)
        //        .KeyUp(VirtualKeyCode.CONTROL);
        //}

        public static void ExportDeviceList()
        {
            CheckRundll32Process();
            string appRoot = AppDomain.CurrentDomain.BaseDirectory;
            string sysExpPath = Path.Combine(appRoot, "util\\sysexp.exe");
            string outputPath = Path.Combine(appRoot, "gpulist.csv");

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = sysExpPath,
                Arguments = $"/Process rundll32.exe /class \"SysListView32\" /Visible Yes /scomma \"{outputPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process proc = Process.Start(startInfo);
            proc.WaitForExit();
        }

        public static int FindDeviceIndex(string filePath, string deviceName)
        {
            int index = -1;
            int lineCount = 0;

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (lineCount++ == 0) continue; 
                    if (line.Contains(deviceName))
                    {
                        index = lineCount - 1; 
                        break;
                    }
                }
            }

            return index -1;
        }

        public static void NavigateToDeviceInstall(int deviceIndex)
        {
            InputSimulator sim = new InputSimulator();
            CheckRundll32Process();
            for (int i = 0; i < deviceIndex; i++)  
            {
                sim.Keyboard.KeyPress(VirtualKeyCode.DOWN);
                Task.Delay(100).Wait(); 
            }

            sim.Keyboard.KeyPress(VirtualKeyCode.SPACE) 
                .KeyPress(VirtualKeyCode.TAB)
                .Sleep(300)
                .KeyPress(VirtualKeyCode.TAB)
                .Sleep(300)
                .KeyPress(VirtualKeyCode.TAB)
                .Sleep(300);
            sim.Keyboard.KeyPress(VirtualKeyCode.SPACE); 
        }


        public static async void SelectandInstall780m()
        {
            CheckRundll32Process();

            ExportDeviceList();

            string appRoot = AppDomain.CurrentDomain.BaseDirectory;
            string csvPath = Path.Combine(appRoot, "gpulist.csv");

            
            int deviceIndex = FindDeviceIndex(csvPath, "AMD Radeon 780M");

            if (deviceIndex == -1)
            {
                Debug.WriteLine("Device not found");
                return;
            }
            
            
            await Task.Run(() => NavigateToDeviceInstall(deviceIndex));
        }

    }
}
