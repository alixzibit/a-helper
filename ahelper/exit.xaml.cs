using System.Windows;

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

        private void button_Copy_Click2(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

    }
}
