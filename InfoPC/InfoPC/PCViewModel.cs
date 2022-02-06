using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Windows.Input;

namespace InfoPC
{
    public class PCViewModel : BaseViewModel
    {

        private readonly string _serverLogsFiles = @"C:\ProgramData\Falcongaze SecureTower\Logs";
        private readonly string _consoleLogsFiles = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\Falcongaze SecureTower";
        private readonly string _agentHostLogsFiles = @"C:\ProgramData\Falcongaze SecureTower\EPA";
        private readonly string _agentCssLogsFiles = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Falcongaze SecureTower";
        private ObservableCollection<string> _ipv4Adress;
        private ObservableCollection<string> _ipv6Adress;
        private ObservableCollection<CheckBoxAdapterItem> _nameAdapter = new ObservableCollection<CheckBoxAdapterItem>();
        private ObservableCollection<FreeDiskSpaceViewModel> _freeDiskSpace = new ObservableCollection<FreeDiskSpaceViewModel>();
        private string _pcName;
        private string _userName;
        private string _domainName;
        private string _productVersion;
        private string _buildVersionOS;
        private bool _isChecked;
        private static Timer _timer;
        public bool IsProduct { get; set; } = true;
        public ObservableCollection<FreeDiskSpaceViewModel> FreeDiskSpace
        {
            get => _freeDiskSpace;
            set
            {
                _freeDiskSpace = value;
                OnPropertyChanged();
            }
        }
        public string PcName
        {
            get => _pcName;
            set
            {
                _pcName = value;
                OnPropertyChanged();
            }
        }
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }
        public string DomainName
        {
            get => _domainName;
            set
            {
                _domainName = value;
                OnPropertyChanged();
            }
        }
        public string ProductVersion
        {
            get => _productVersion;
            set
            {
                _productVersion = value;
                OnPropertyChanged();
            }
        }
        public string BuildVersionOS
        {
            get => _buildVersionOS;
            set
            {
                _buildVersionOS = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> Ipv4Adress
        {
            get => _ipv4Adress;
            set
            {
                _ipv4Adress = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> Ipv6Adress
        {
            get => _ipv6Adress;
            set
            {
                _ipv6Adress = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<CheckBoxAdapterItem> NameAdapter
        {
            get => _nameAdapter;
            set
            {
                _nameAdapter = value;
                OnPropertyChanged();
            }
        }
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public PCViewModel()
        {
            UpdateInfo();
            _timer = new Timer(Callback, null, 1000 * 5, Timeout.Infinite);
            CopyToClipboardCommand = new RelayCommand(CopyToClipboardExecute);
            CloseWindowsCommand = new RelayCommand(CloseWindowExecute);
            ChangeStatusEthenetCommand = new RelayCommand(ChangeStatusEthenetExecute);
            CopyLogsCommand = new RelayCommand(CopyLogsExecute);

        }

        public ICommand CopyToClipboardCommand { get; set; }
        public ICommand CloseWindowsCommand { get; set; }
        public ICommand ChangeStatusEthenetCommand { get; set; }
        public ICommand CopyLogsCommand { get; set; }
        private void GetFreeSpace()
        {
            var allDrives = DriveInfo.GetDrives();
            Application.Current.Dispatcher.Invoke(delegate
            {
                FreeDiskSpace.Clear();
                foreach (var drive in allDrives)
                {
                    if (drive.IsReady)
                    {
                        FreeDiskSpace.Add(new FreeDiskSpaceViewModel(drive.Name, drive.AvailableFreeSpace / Math.Pow(1024, 3)));
                    }

                }
            });
        }
        private void GetUserName()
        {
            UserName = Environment.UserName;
        }
        private void GetPCName()
        {
            PcName = Environment.MachineName;
        }
        private void GetDomainName()
        {
            DomainName = Environment.UserDomainName;
        }
        private void GetIPv4Adress()
        {
            Ipv4Adress = new ObservableCollection<string> (Dns.GetHostEntry(Dns.GetHostName()).AddressList.
                         Where(x => x.AddressFamily == AddressFamily.InterNetwork).
                         Select(x => x.ToString()).
                         ToList());
        }
        private void GetIPv6Adress()
        {
            Ipv6Adress = new ObservableCollection<string>(Dns.GetHostEntry(Dns.GetHostName()).AddressList.
                         Where(x => x.AddressFamily == AddressFamily.InterNetworkV6).
                         Select(x => x.ToString()).
                         ToList());
        }
        private void Callback(object state)
        {
            UpdateInfo();
            _timer.Change(1000 * 10, Timeout.Infinite);
        }
        private void CopyToClipboardExecute(object arg)
        {
            if (arg is ObservableCollection<string> stringArg)
            {
                Clipboard.SetDataObject(string.Join(Environment.NewLine, stringArg.ToArray()));
            }
            else
            {
                Clipboard.SetDataObject(arg.ToString());
            }
        }
        private void CloseWindowExecute(object arg)
        {
            Application.Current.Shutdown();
        }
        private void CopyLogsExecute(object arg)
        {
            Directory.CreateDirectory(@"C:\Logs");
            string[] serverLogsFiles = Directory.GetFiles(_serverLogsFiles, "*.log", SearchOption.AllDirectories);
            if (serverLogsFiles.Length > 0)
            {
                foreach (var filename in serverLogsFiles)
                {
                    var fileName = Path.GetFileName(filename);
                    var destFile = Path.Combine(@"C:\Logs", fileName);
                    File.Copy(filename, destFile, true);
                }
            }
            string[] consoleLogsFiles = Directory.GetFiles(_consoleLogsFiles, "*.log");
            if (serverLogsFiles.Length > 0)
            {
                foreach (var filename in consoleLogsFiles)
                {
                    var fileName = Path.GetFileName(filename);
                    var destFile = Path.Combine(@"C:\Logs", fileName);
                    File.Copy(filename, destFile, true);
                }
            }
            Directory.CreateDirectory(@"C:\AgentLogs");
            string[] agentHostLogsFiles = Directory.GetFiles(_agentHostLogsFiles, "*.log");
            if (agentHostLogsFiles.Length > 0)
            {
                foreach (var filename in agentHostLogsFiles)
                {
                    var fileName = Path.GetFileName(filename);
                    var destFile = Path.Combine(@"C:\AgentLogs", fileName);
                    File.Copy(filename, destFile, true);
                }
            }
            string[] agentCssLogsFiles = Directory.GetFiles(_agentCssLogsFiles, "*.log");
            if (agentCssLogsFiles.Length > 0)
            {
                foreach (var filename in agentCssLogsFiles)
                {
                    var fileName = Path.GetFileName(filename);
                    var destFile = Path.Combine(@"C:\AgentLogs", fileName);
                    File.Copy(filename, destFile, true);
                }
            }
            string startPath = @"C:\Logs";
            string zipPath = @"C:\result.zip";
            ZipFile.CreateFromDirectory(startPath, zipPath);
        }
        private void ChangeStatusEthenetExecute(object arg)
        {
            foreach (ManagementObject item in GetAllAdapter().Get())
            {
                if (item["NetConnectionId"].Equals(arg.ToString()) && (bool)item.Properties["NetEnabled"].Value)
                {

                    item.InvokeMethod("Disable", null);

                }
                if (item["NetConnectionId"].Equals(arg.ToString()) && !(bool)item.Properties["NetEnabled"].Value)
                {
                    item.InvokeMethod("Enable", null);
                }
            }
        }
        private void GetVersionNumber()
        {
            ProductVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\FalconGaze\SecureTower", "ProductVersion", "")?.ToString();
            if (ProductVersion == null)
            {
                IsProduct = false;
            }
        }
        private void GetBuildVersionOS()
        {
            var productName = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "")?.ToString();
            var displayVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", "")?.ToString();
            var currentBild = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", "")?.ToString();
            BuildVersionOS = $"{productName}: {displayVersion} ({currentBild })";
        }
        private void GetAdapterName()
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                NameAdapter.Clear();
                foreach (var item in GetAllAdapter().Get())
                {
                    IsChecked = (bool)item.Properties["NetEnabled"].Value;
                    NameAdapter.Add(new CheckBoxAdapterItem(IsChecked, item["NetConnectionId"].ToString()));
                }
            });
        }
        private ManagementObjectSearcher GetAllAdapter()
        {
            return new ManagementObjectSearcher(new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL"));
        }
        private void UpdateInfo()
        {
            GetFreeSpace();
            GetUserName();
            GetPCName();
            GetDomainName();
            GetIPv4Adress();
            GetIPv6Adress();
            GetAdapterName();
            GetVersionNumber();
            GetBuildVersionOS();
        }

    }
}

