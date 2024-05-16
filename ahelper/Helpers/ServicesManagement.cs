using System.ServiceProcess;
using System.ComponentModel;

namespace ahelper.Helpers
{
    public class ServicesManagement
    {
        private List<string> predefinedServicesToTerminate;
       

        public ServicesManagement()
        {
            // List of service names that should be checked by default for termination
            // and also prioritized in the list
            predefinedServicesToTerminate = new List<string>
            {
                "GlideXService", "GlideXRemoteService", "GlideXNearService", "GlideXServiceExt",
                "wuauserv", "CertPropSvc", "ClickToRunSvc", "edgeupdate", "edgeupdatem", "WinDefend",
                "WwanSvc", "ScDeviceEnum", "SCardSvr", "WSearch", "Spooler", "Sense",
                "SEMgrSvc", "", "vmicguestinterface", "vmicheartbeat", "vmickvpexchange", "vmicompute",
                "vmicrdv", "vmicshutdown", "vmictimesync", "vmicvmsession", "vmicvss"
            };
        }
      
        public List<ServiceItem> GetAllServices()
        {
            List<ServiceItem> services = new List<ServiceItem>();
            ServiceController[] allServices = ServiceController.GetServices();

            // Filter and convert only running services to ServiceItem objects
            var serviceItems = allServices
                .Where(sc => sc.Status == ServiceControllerStatus.Running)  // Filter to include only running services
                .Select(sc => new ServiceItem
                {
                    ServiceName = sc.ServiceName,
                    Description = GetServiceDescription(sc.ServiceName),
                    IsSelectedForTermination = predefinedServicesToTerminate.Contains(sc.ServiceName)
                }).ToList();

            // Sort services, prioritizing predefined ones
            var sortedServices = serviceItems
                .OrderByDescending(s => predefinedServicesToTerminate.Contains(s.ServiceName))
                .ThenBy(s => s.ServiceName)  // Optionally, sort alphabetically as a secondary criterion
                .ToList();

            return sortedServices;
        }




        private string GetServiceDescription(string serviceName)
        {
            // Use WMI to get more detailed info like the description of the service
            string description = "";
            try
            {
                System.Management.ManagementObject wmiService;
                wmiService = new System.Management.ManagementObject("Win32_Service.Name='" + serviceName + "'");
                wmiService.Get();
                description = wmiService["Description"]?.ToString() ?? "No Description Available";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error getting service description: " + ex.Message);
            }
            return description;
        }
        

    }



    //public class ServiceItem
    //{
    //    public string ServiceName { get; set; }
    //    public string Description { get; set; }
    //    public bool IsSelectedForTermination { get; set; }
    //}

    public class ServiceItem : INotifyPropertyChanged
    {
        private bool _isSelectedForTermination;
        public string ServiceName { get; set; }
        public string Description { get; set; }

        public bool IsSelectedForTermination
        {
            get => _isSelectedForTermination;
            set
            {
                if (_isSelectedForTermination != value)
                {
                    _isSelectedForTermination = value;
                    OnPropertyChanged(nameof(IsSelectedForTermination));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
