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
using System.Windows.Shapes;

namespace ahelper
{
    /// <summary>
    /// Interaction logic for exit.xaml
    /// </summary>
    public partial class exit : Window
    {
        public exit()
        {
            InitializeComponent();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            
            Application.Current.Shutdown();
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
         
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            }

            this.Close();
        }

    }
}
