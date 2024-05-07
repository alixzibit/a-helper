using ahelper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ahelper.Controls.OptimizeControls
{
    /// <summary>
    /// Interaction logic for SystemClean.xaml
    /// </summary>
    public partial class SystemClean : UserControl
    {
        public SystemClean()
        {
            InitializeComponent();
            InitializeCoreIsoToggleButtonState();
           
        }


        public void ToggleCoreIso_Checked(object sender, RoutedEventArgs e)
        {
            ToggleCoreIso.IsChecked = true;
        }

        public void ToggleCoreIso_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleCoreIso.IsChecked = false;
        }

        private void InitializeCoreIsoToggleButtonState()
        {
            var cleaner = new SysCleanTw();
            bool isEnabled = cleaner.CheckCoreIsolationEnabled();
            ToggleCoreIso.IsChecked = isEnabled;
        }
        private SysCleanTw sysCleanTw = new SysCleanTw();


        public void CleanSystemToggle_Checked(object sender, RoutedEventArgs e)
        {

          
            CleanSystemToggle.IsChecked = true;                                 

           
        }

        public void CleanSystemToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }

        public void Dx9Toggle_Checked(object sender, RoutedEventArgs e)
        {
            sysCleanTw.CleanAmdGpuCache(true, false, false);  
        }

        public void Dx9Toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }

        public void Dx11Toggle_Checked(object sender, RoutedEventArgs e)
        {
            sysCleanTw.CleanAmdGpuCache(false, false, true);  
        }

        public void Dx11Toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }

        public void Dx12Toggle_Checked(object sender, RoutedEventArgs e)
        {
            sysCleanTw.CleanAmdGpuCache(false, true, false);  
        }

        public void Dx12Toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }
        public void ToggleDNS_Checked(object sender, RoutedEventArgs e)
        {
            
            sysCleanTw.FlushDns(true);
        }

        public void ToggleDNS_Unchecked(object sender, RoutedEventArgs e)
        {
            sysCleanTw.FlushDns(false);
        }
        public void ToggleWinsock_Checked(object sender, RoutedEventArgs e)
        {
            sysCleanTw.ResetWinsock(true);  
        }

        public void ToggleWinsock_Unchecked(object sender, RoutedEventArgs e)
        {
            sysCleanTw.ResetWinsock(false);
        }


    }
}
