using ahelper.Helpers;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ahelper.Controls.OptimizeControls
{
    /// <summary>
    /// Interaction logic for ServicesManager.xaml
    /// </summary>
    public partial class ServicesManager : UserControl
    {
        private ServicesManagement servicesManagement;
        public ServicesManager()
        {
            InitializeComponent();
            servicesManagement = new ServicesManagement();
            //LoadServices();
        }

        //public async void LoadServices()
        //{
        //    LoadingUi.Visibility = Visibility.Visible; // Show loading animation

        //    // Load services asynchronously to avoid freezing the UI
        //    var services = await Task.Run(() => servicesManagement.GetAllServices());

        //    // Update the UI after loading services
        //    ServicesDataGrid.ItemsSource = services;
        //    LoadingUi.Visibility = Visibility.Hidden; // Hide loading animation
        //}
        public async Task LoadServicesAsync()
        {
            LoadingUi.Visibility = Visibility.Visible; 
            try
            {
                var services = await Task.Run(() => servicesManagement.GetAllServices());
                ServicesDataGrid.ItemsSource = services;
            }
            finally
            {
                LoadingUi.Visibility = Visibility.Hidden; 
                
            }
        }

      

        public void FocusDataGrid()
        {
            if (ServicesDataGrid.Items.Count > 0)
            {
                ServicesDataGrid.Focus();
                ServicesDataGrid.SelectedIndex = 0; 
                ServicesDataGrid.CurrentCell = new DataGridCellInfo(ServicesDataGrid.Items[0], ServicesDataGrid.Columns[0]);
            }
        }

        public async Task<Dictionary<string, bool>> StopSelectedServicesAsync()
        {
            
            List<string> selectedServices = ServicesDataGrid.Items
                .OfType<ServiceItem>() // This safely filters items to those that can be cast to ServiceItem
                .Where(item => item.IsSelectedForTermination) // Only include items where the checkbox is checked
                .Select(item => item.ServiceName)
                .ToList();

            if (selectedServices.Count > 0)
            {
                Debug.WriteLine($"Multiple services selected: {selectedServices.Count}");
            }

            Dictionary<string, bool> stopResults = new Dictionary<string, bool>();

            foreach (var serviceName in selectedServices)
            {
                using (var service = new ServiceController(serviceName))
                {
                    service.Refresh();  
                    try
                    {
                        if (service.Status == ServiceControllerStatus.Running)
                        {
                            service.Stop();
                            await Task.Delay(250);
                            await Task.Run(() => service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30)));
                            stopResults[serviceName] = true; // Mark as successfully stopped
                        }
                        else
                        {
                            Debug.WriteLine($"Service {serviceName} is not running, current status: {service.Status}");
                            stopResults[serviceName] = false; // Mark as not applicable for stopping
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to stop {serviceName}: {ex.Message}");
                        stopResults[serviceName] = false; // Mark as failed to stop
                    }
                }
            }

            return stopResults;
        }




        //private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        var dataGrid = sender as DataGrid;
        //        if (dataGrid != null && dataGrid.CurrentColumn is DataGridCheckBoxColumn && dataGrid.CurrentItem != null)
        //        {
        //            // Access the current row data as ServiceItem
        //            var serviceItem = dataGrid.CurrentItem as ServiceItem;
        //            if (serviceItem != null)
        //            {
        //                // Toggle the checkbox value for termination
        //                serviceItem.IsSelectedForTermination = !serviceItem.IsSelectedForTermination;
        //                dataGrid.CommitEdit(DataGridEditingUnit.Row, true);  // Commit the edit to update the source
        //                e.Handled = true;  // Prevent the default navigation behavior
        //            }
        //        }
        //    }
        //}
        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // or whatever key the "A" button maps to
            {
                SimulateClickOnCheckbox(sender as DataGrid);
                e.Handled = true; // Prevent further processing to avoid navigating to the next row
            }
        }
        
            //private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
            //{
            //    if (e.Key == Key.Tab || e.Key == Key.Enter)
            //    {
            //        var dataGrid = sender as DataGrid;
            //        if (dataGrid.CurrentColumn is DataGridCheckBoxColumn)
            //        {
            //            // Toggle the checkbox value
            //            // Move to the next checkbox if necessary, or handle as needed
            //            e.Handled = true;
            //        }
            //        else
            //        {
            //            // Skip focusing or handle navigation manually
            //            e.Handled = true;  // Consider managing focus to jump to the next available checkbox
            //        }
            //    }
            //}


            private void SimulateClickOnCheckbox(DataGrid dataGrid)
        {
            if (dataGrid.SelectedItem != null && dataGrid.CurrentColumn is DataGridCheckBoxColumn)
            {
                var cellContent = dataGrid.CurrentColumn.GetCellContent(dataGrid.SelectedItem) as CheckBox;
                if (cellContent != null)
                {
                    cellContent.IsChecked = !cellContent.IsChecked; // Toggle the checkbox state
                }
            }
        }
        private void DebugSelectedServices()
        {
            var selectedServices = ServicesDataGrid.Items
                .OfType<ServiceItem>()  // This safely filters items to those that can be cast to ServiceItem
                .Where(item => item.IsSelectedForTermination)
                .Select(item => item.ServiceName);

            Debug.WriteLine("Selected Services:");
            foreach (var service in selectedServices)
            {
                Debug.WriteLine(service);
            }
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            DebugSelectedServices();
        }
    }
}
