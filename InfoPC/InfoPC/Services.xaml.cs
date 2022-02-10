using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Timer = System.Timers.Timer;


namespace InfoPC
{
    /// <summary>
    /// Interaction logic for Services.xaml
    /// </summary>
    public partial class Services : Window
    {
        private readonly Timer _timer;

        public List<string> DefaultServices = new List<string>()
        {
            "FgStSecurityCenterServerService",
            "FgStSearchSvc",
            "FgStInvestigationCenterServerService",
            "FgStIcapServerService",
            "FgStUsersAuthServerService",
            "FgStMailProcessingServerService",
            "FgStEPAControlServerService",
            "FgStEPAServerService",
            "FgStHealthMonitorServerService",
            "FgStRecognitionServer",
            "FgStSpeechServer",
            "FgStStorageServer",
            "FgStReportsServerService"
        };

        public List<ServiceControllerVM> ServiceControllers { get; set; } = new List<ServiceControllerVM>();
        //{
        // new ServiceControllerVM() {ServiceController = new ServiceController("FgStSecurityCenterServerService")},
        // new ServiceControllerVM() {ServiceController = new ServiceController("FgStSearchSvc")},
        // new ServiceControllerVM()
        //     {ServiceController = new ServiceController("FgStInvestigationCenterServerService")},
        // new ServiceControllerVM() {ServiceController = new ServiceController("FgStIcapServerService")},
        // new ServiceControllerVM() {ServiceController = new ServiceController("FgStUsersAuthServerService")},
        // new ServiceControllerVM() {ServiceController = new ServiceController("FgStMailProcessingServerService")},
        // new ServiceControllerVM() {ServiceController = new ServiceController("FgStEPAControlServerService")},
        // new ServiceControllerVM() {ServiceController = new ServiceController("FgStHealthMonitorServerService")},
        // new ServiceControllerVM() {ServiceController = new ServiceController("FgStRecognitionServer")},
        // new ServiceControllerVM() {ServiceController = new ServiceController("FgStSpeechServer")},
        // new ServiceControllerVM() {ServiceController = new ServiceController("FgStStorageServer")},
        // new ServiceControllerVM() {ServiceController = new ServiceController("FgStReportsServerService")}
        //};


        private const string SettingsFileName = "AdditionalServices.txt";

        public Services()
        {
            var services = ServiceController.GetServices().Where(x => x.ServiceName.IndexOf("fgst", StringComparison.OrdinalIgnoreCase) != -1).ToList();
            if (!File.Exists(SettingsFileName))
            {
                File.WriteAllLines(SettingsFileName, new string[] { string.Empty });
                Thread.Sleep(200);
            }

            var readAllLines = File.ReadAllLines(SettingsFileName).ToList();
            ServiceControllers.AddRange(services.Select(x => new ServiceControllerVM() { ServiceController = x }));
            readAllLines.Where(x => !string.IsNullOrWhiteSpace(x) && !services.Select(xx => xx.ServiceName).Contains(x)).ToList().ForEach(x =>
            {
                try
                {
                    var serviceController = new ServiceController(x);
                    var serviceControllerStatus = serviceController.Status;
                    var serviceControllerVm = new ServiceControllerVM() { ServiceController = serviceController };
                    ServiceControllers.Add(serviceControllerVm);
                }
                catch
                {
                    //
                }
            });
            ServiceControllers = ServiceControllers.OrderBy(x => x.ServiceController.ServiceName).ToList();
            InitializeComponent();
            StartCommand = new AsyncRelayCommand(Start);
            StopCommand = new AsyncRelayCommand(Stop);
            RestartCommand = new AsyncRelayCommand(Restart);
            _timer = new System.Timers.Timer();
            _timer.Interval = 200;
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
            DataContext = this;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            ServiceControllers.ForEach(x =>
            {
                x.ServiceController.Refresh();
                x.OnPropertyChanged("Status");
                x.OnPropertyChanged("IsStopped");
            });
        }

        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand RestartCommand { get; set; }

        private void StartAll(object sender, RoutedEventArgs e)
        {
            ServiceControllers.Where(x => x.Status == ServiceControllerStatus.Stopped).ToList().ForEach(x => Task.Run(x.ServiceController.Start));
        }

        private void StopAll(object sender, RoutedEventArgs e)
        {
            ServiceControllers.Where(x => x.Status == ServiceControllerStatus.Running).ToList().ForEach(x => Task.Run(x.ServiceController.Stop));
        }

        private void RestartAll(object sender, RoutedEventArgs e)
        {
            ServiceControllers.ForEach(x => Task.Run(async () =>
            {
                var count = 0;
                while (x.ServiceController.Status != ServiceControllerStatus.Stopped)
                {
                    try
                    {
                        if (x.ServiceController.CanStop)
                        {
                            x?.ServiceController.Stop();
                        }
                        await Task.Delay(500);
                        count++;
                        if (count == 2000)
                        {
                            break;
                        }
                    }
                    catch
                    {
                        //
                    }

                }

                count = 0;
                while (x.ServiceController.Status == ServiceControllerStatus.Stopped)
                {
                    x.ServiceController.Start();
                    await Task.Delay(500);
                    count++;
                    if (count == 2000)
                    {
                        break;
                    }
                }

            }));
        }

        private Task Start(object o)
        {
            return Task.Run(() =>
            {
                var service = o as ServiceControllerVM;
                if (service?.ServiceController.Status == ServiceControllerStatus.Stopped)
                    service?.ServiceController.Start();
            });
        }

        private Task Stop(object o)
        {
            return Task.Run(() =>
            {
                var service = o as ServiceControllerVM;
                if (service?.ServiceController.CanStop ?? false)
                    service?.ServiceController.Stop();
            });
        }

        private Task Restart(object o)
        {
            return Task.Run(async () =>
            {
                var service = o as ServiceControllerVM;


                var count = 0;
                while (service.ServiceController != null &&
                       service.ServiceController.Status != ServiceControllerStatus.Stopped)
                {
                    if (service.ServiceController.CanStop)
                    {
                        service?.ServiceController.Stop();
                    }
                    await Task.Delay(100);
                    count++;
                    if (count == 200)
                    {
                        break;
                    }
                }

                service?.ServiceController.Start();
            });
        }
    }
}
