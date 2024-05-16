using System.Windows;

namespace ahelper
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Opt_dialog : Window
    {
        public Opt_dialog()
        {
            InitializeComponent();
        }

        public void UpdateStatus(string status)
        {
            statusTextBlock.Text += "\n" + status; 
                
                
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}