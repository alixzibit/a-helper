using System.Windows;
using System.Windows.Controls;
using System.ServiceProcess;
using ahelper.Helpers;

namespace ahelper.Controls
{
    public partial class XboxButton : UserControl
    {
        public XboxButton()
        {
            InitializeComponent();
            Loaded += XboxButton_Loaded;  // Check service status when the control is loaded
        }

        private void XboxButton_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateToggleButtonState();
        }

        //private void ToggleOverride_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (!IsServiceRunning())
        //    {
        //        ServiceInstaller.InstallService();
        //    }
        //    UpdateStatusLabel("Service installed and override enabled.");
        //}

        //private void ToggleOverride_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    if (IsServiceRunning())
        //    {
        //        ServiceInstaller.UninstallService();
        //    }
        //    UpdateStatusLabel("Service uninstalled and override disabled.");
        //}

        private void UpdateToggleButtonState()
        {
            ToggleOverride.IsChecked = IsServiceRunning();
        }

        private bool IsServiceRunning()
        {
            try
            {
                using (var sc = new ServiceController("ACKeyRedirector"))
                {
                    return sc.Status == ServiceControllerStatus.Running;
                }
            }
            catch
            {
                return false;
            }
        }

        private void UpdateStatusLabel(string message)
        {
            
                statuslabel.Text = message;
            
        }

        private void ToggleOverride_Checked_1(object sender, RoutedEventArgs e)
        {
            if (!IsServiceRunning())
            {
                ServiceInstaller.InstallService();
            }
            UpdateStatusLabel("Service installed and override enabled.");

        }

        private void ToggleOverride_Unchecked_1(object sender, RoutedEventArgs e)
        {
            if (IsServiceRunning())
            {
                ServiceInstaller.UninstallService();
            }
            UpdateStatusLabel("Service uninstalled and override disabled.");
        }

        private void redirectACCkey_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
